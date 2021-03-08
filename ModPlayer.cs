using System;
using System.Linq;

namespace DxNicc2000
{
	internal static class ModPlayer
	{
		internal static bool Stopped;
		internal static int Position;
		internal static int Division;
		internal static long Speed;
		internal static long Timer;
		internal static long Last;
		internal static bool[] ChannelTriggers;
		internal static int[] ChannelPitches;
		internal static int[] ChannelSamples;
		internal static int[] ChannelVolume;

		internal static void Play()
		{
			Position = 0;
			Speed = 140;
			Timer = 0;
			Last = Environment.TickCount;

			ChannelTriggers = new bool[4];
			ChannelPitches = new int[4];
			ChannelSamples = new int[4];
			ChannelVolume = Enumerable.Repeat(64, 4).ToArray();

			Stopped = false;
		}

		internal static void Update()
		{
			if (Stopped)
				return;

			var current = Environment.TickCount;
			var elapsed = current - Last;
			Last = current;

			Timer -= elapsed;

			for (var channel2 = 0; channel2 < 4; channel2++)
				ChannelTriggers[channel2] = false;

			if (Timer <= 0)
			{
				var nextPosition = Position;
				var nextDivision = Division + 1;

				if (nextDivision == 64)
				{
					nextPosition += 1;
					nextDivision = 0;

					System.Diagnostics.Debug.WriteLine((nextPosition + 1).ToString());
				}

				for (var channel = 0; channel < 4; channel++)
				{
					var value = ModSong.Patterns[ModSong.Sequence[Position]].Divisions[Division].Channels[channel];

					if (value.Sample != 0)
					{
						ChannelTriggers[channel] = true;
						ChannelSamples[channel] = (int)value.Sample;
						ChannelVolume[channel] = ModSong.SampleVolumes[value.Sample];
					}
					
					if (value.Effect != 0)
					{
						var type = value.Effect >> 8;
						var parameter = (value.Effect >> 4) & 0x0F;
						var parameter2 = value.Effect & 0x0F;

						switch (type)
						{
							case 0x0A:
								if (parameter != 0)
								{
									ChannelVolume[channel] += (int)parameter;

									if (ChannelVolume[channel] > 64)
										ChannelVolume[channel] = 64;
								}
								else
								{
									ChannelVolume[channel] -= (int)parameter2;

									if (ChannelVolume[channel] < 0)
										ChannelVolume[channel] = 0;
								}
								break;

							case 0x0C:
								ChannelVolume[channel] = (int)((parameter << 4) | parameter2);
								break;

							case 0x0D:
								nextPosition = Position + 1;
								nextDivision = (int)((parameter * 10) + parameter2);

								System.Diagnostics.Debug.WriteLine((nextPosition + 1).ToString());

								break;

							case 0x0E:
								switch (parameter)
								{
									case 0x0A:
										ChannelVolume[channel] += (int)parameter2;

										if (ChannelVolume[channel] > 64)
											ChannelVolume[channel] = 64;
										break;

									case 0x0B:
										ChannelVolume[channel] -= (int)parameter2;

										if (ChannelVolume[channel] < 0)
											ChannelVolume[channel] = 0;
										break;
								}
								break;
						}
					}

					if (value.Parameter != 0)
					{
						ChannelPitches[channel] = (int)value.Parameter;
					}
				}

				Position = nextPosition;
				Division = nextDivision;

				if (Position >= ModSong.Sequence.Length)
				{
					ChannelVolume[0] = 0;
					ChannelVolume[1] = 0;
					ChannelVolume[2] = 0;
					ChannelVolume[3] = 0;

					Stopped = true;
				}
				else
					Timer += Speed;
			}
		}
	}
}