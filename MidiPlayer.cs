using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxNicc2000
{
	internal static class MidiPlayer
	{
		internal static int[] ChannelPitches;
		internal static int[] ChannelNotes;
		internal static int[] ChannelSamples;
		internal static int[] ChannelDrums;
		internal static int[] ChannelOffsets;
		internal static int[] ChannelVolume;
		internal static long[] ChannelDurations;
		internal static long[] ChannelTimers;
		internal static int Reverb;
		internal static int Chorus;

		internal static void Play()
		{
			ChannelPitches = new int[4];
			ChannelNotes = new int[4];
			ChannelSamples = new int[4];
			ChannelDrums = new int[4];
			ChannelOffsets = new int[4];
			ChannelVolume = new int[4];
			ChannelDurations = new long[4];
			ChannelTimers = new long[4];
			Reverb = 127;
			Chorus = 127;

			Midi.Enable();

			for (var channel = 0; channel < 4; channel++)
			{
				Midi.ControlChange(channel, 123, 0);
				Midi.ProgramChange(channel, Midi.Patches.GrandPiano);
				Midi.ControlChange(channel, Midi.Controls.Reverb, Reverb);
				Midi.ControlChange(channel, Midi.Controls.Tremolo, 127);
				Midi.ControlChange(channel, Midi.Controls.Chorus, Chorus);
				Midi.ControlChange(channel, Midi.Controls.Detune, 127);
				Midi.ControlChange(channel, Midi.Controls.Phaser, 127);

				//Midi.ControlChange(channel, Midi.Controls.Volume, ChannelVolumes[channel]);
			}

			Midi.ControlChange(9, 123, 0);
			Midi.ControlChange(9, Midi.Controls.Reverb, Reverb);
			//Midi.ControlChange(9, Midi.Controls.Tremolo, 127);
			Midi.ControlChange(9, Midi.Controls.Chorus, Chorus);
			//Midi.ControlChange(9, Midi.Controls.Detune, 127);
			//Midi.ControlChange(9, Midi.Controls.Phaser, 127);

			Midi.ControlChange(9, Midi.Controls.Volume, 127);

		}

		internal static void Stop()
		{
			Midi.Disable();
		}

		internal static void Update()
		{
			for (var channel = 0; channel < 4; channel++)
			{
				if (ModPlayer.ChannelSamples[channel] != ChannelSamples[channel])
					Sample(channel);

				if (ModPlayer.ChannelVolume[channel] != ChannelVolume[channel])
				{
					if (ChannelDrums[channel] == 0)
					{
						if (ChannelVolume[channel] == 0)
						{
							if (ChannelNotes[channel] != 0)
								Midi.NoteOff(channel, ChannelNotes[channel], 0);
						}

						Midi.ControlChange(channel, Midi.Controls.Volume, (int)((ModPlayer.ChannelVolume[channel] / 64.0f) * 127.0f));
					}

					ChannelVolume[channel] = ModPlayer.ChannelVolume[channel];
				}

				if (ModPlayer.ChannelTriggers[channel])
				{
					if (ChannelNotes[channel] != 0)
						Midi.NoteOff(channel, ChannelNotes[channel], 0);

					if (ChannelDrums[channel] != 0)
					{
						Midi.NoteOn(9, ChannelDrums[channel], (int)((ModPlayer.ChannelVolume[channel] / 64.0f) * 127.0f));
						Midi.NoteOff(9, ChannelDrums[channel], 0);
					}
					else
					{
						Note(channel);
						Midi.NoteOn(channel, ChannelNotes[channel], 127);
					}
				}
			}
		}

		private static void Sample(int channel)
		{
			if (ChannelDrums[channel] == 0 &&
				ChannelNotes[channel] != 0)
			{
				Midi.NoteOff(channel, ChannelNotes[channel], 0);
				ChannelNotes[channel] = 0;
			}

			ChannelDrums[channel] = 0;

			switch (ModPlayer.ChannelSamples[channel])
			{
				case 0x01:
					ChannelDrums[channel] = Midi.Drums.SnareDrum;
					break;

				case 0x02:
					ChannelDrums[channel] = Midi.Drums.BassDrum;
					break;

				case 0x03:
					ChannelDrums[channel] = Midi.Drums.SnareDrum2;
					break;

				case 0x04:
					Midi.ProgramChange(channel, Midi.Patches.BassLead);
					ChannelOffsets[channel] = 12;
					break;

				case 0x05:
					Midi.ProgramChange(channel, Midi.Patches.DistortionGuitar);
					ChannelOffsets[channel] = 14;
					break;

				case 0x06:
					Midi.ProgramChange(channel, Midi.Patches.VoiceLead);
					ChannelOffsets[channel] = 24;
					break;

				case 0x07:
					Midi.ProgramChange(channel, Midi.Patches.OrchestraHit);
					ChannelOffsets[channel] = 33;
					break;

				case 0x08:
					Midi.ProgramChange(channel, Midi.Patches.CharangLead);
					ChannelOffsets[channel] = 24;
					break;

				case 0x09:
					Midi.ProgramChange(channel, Midi.Patches.Gunshot);
					ChannelOffsets[channel] = 12;
					break;

				case 0x0A:
					Midi.ProgramChange(channel, Midi.Patches.FifthsLead);
					ChannelOffsets[channel] = 48;
					break;

				case 0x0B:
					Midi.ProgramChange(channel, Midi.Patches.FifthsLead);
					ChannelOffsets[channel] = 48;
					break;

				case 0x0C:
					Midi.ProgramChange(channel, Midi.Patches.SynthBass2);
					ChannelOffsets[channel] = 12;
					break;

				case 0x0D:
					Midi.ProgramChange(channel, Midi.Patches.SynthBass2);
					ChannelOffsets[channel] = 12;
					break;

				case 0x0E:
					Midi.ProgramChange(channel, Midi.Patches.Applause);
					ChannelOffsets[channel] = 12;
					break;

				case 0x0F:
					Midi.ProgramChange(channel, Midi.Patches.Violin);
					ChannelOffsets[channel] = 24;
					break;

				case 0x10:
					Midi.ProgramChange(channel, Midi.Patches.Atmosphere);
					ChannelOffsets[channel] = 24;
					break;

				case 0x11:
					Midi.ProgramChange(channel, Midi.Patches.SynthBass2);
					ChannelOffsets[channel] = 0;
					break;

				case 0x12:
					Midi.ProgramChange(channel, Midi.Patches.SynthBass);
					ChannelOffsets[channel] = 0;
					break;

				case 0x13:
					Midi.ProgramChange(channel, Midi.Patches.BassLead);
					ChannelOffsets[channel] = 0;
					break;

				case 0x14:
					Midi.ProgramChange(channel, Midi.Patches.SynthBass2);
					ChannelOffsets[channel] = 12;
					break;

				case 0x15:
					Midi.ProgramChange(channel, Midi.Patches.SynthBass);
					ChannelOffsets[channel] = 12;
					break;

				case 0x16:
					Midi.ProgramChange(channel, Midi.Patches.BassLead);
					ChannelOffsets[channel] = 12;
					break;

				case 0x17:
					Midi.ProgramChange(channel, Midi.Patches.BassLead);
					ChannelOffsets[channel] = 12;
					break;

				case 0x18:
					Midi.ProgramChange(channel, Midi.Patches.CharangLead);
					ChannelOffsets[channel] = 12;
					break;

				case 0x19:
					Midi.ProgramChange(channel, Midi.Patches.ChiffLead);
					ChannelOffsets[channel] = 12;
					break;

				case 0x1A:
					Midi.ProgramChange(channel, Midi.Patches.CalliopeLead);
					ChannelOffsets[channel] = 12;
					break;

				default:
					System.Diagnostics.Debug.WriteLine("Channel " + channel + " Unknown Sample " + ModPlayer.ChannelSamples[channel].ToString("X2"));
					Midi.ProgramChange(channel, Midi.Patches.SquareLead);
					ChannelOffsets[channel] = 24;
					break;
			}

			ChannelSamples[channel] = ModPlayer.ChannelSamples[channel];
		}

		private static void Note(int channel)
		{
			var pitch = ModPlayer.ChannelPitches[channel];

			var closest = Enumerable.Range(0, ModSong.NotePitches.Length).OrderBy(x => Math.Abs(ModSong.NotePitches[x] - pitch)).First();

			ChannelNotes[channel] = closest + ChannelOffsets[channel];
		}
	}
}
