using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DxNicc2000
{
	public static class Midi
	{
		[DllImport("winmm.dll")]
		static extern uint midiOutOpen(out IntPtr lphMidiOut, uint uDeviceID, IntPtr dwCallback, IntPtr dwInstance, uint dwFlags);

		[DllImport("winmm.dll")]
		static extern uint midiOutShortMsg(IntPtr hMidiOut, uint dwMsg);

		[DllImport("winmm.dll")]
		static extern uint midiOutClose(IntPtr hMidiOut);

		private delegate void MidiCallBack(IntPtr handle, uint msg, uint instance, uint param1, uint param2);

		private static IntPtr Handle;

		public static void Enable()
		{
			if (Handle == IntPtr.Zero)
			{
				//var result = midiOutOpen(out Handle, 0xFFFFFFFF, IntPtr.Zero, IntPtr.Zero, 0);
				var result = midiOutOpen(out Handle, 0, IntPtr.Zero, IntPtr.Zero, 0);

				//System.Diagnostics.Debug.WriteLine(result);
			}
		}

		public static void NoteOn(int channel, int note, int velocity)
		{
			if (Handle != IntPtr.Zero)
			{
				var result = midiOutShortMsg(Handle, 0x90u | (uint)channel | ((uint)note << 8) | ((uint)velocity << 16));

				//System.Diagnostics.Debug.WriteLine(result);
			}
		}

		public static void NoteOff(int channel, int note, int velocity)
		{
			if (Handle != IntPtr.Zero)
			{
				var result = midiOutShortMsg(Handle, 0x80u | (uint)channel | ((uint)note << 8) | ((uint)velocity << 16));

				//System.Diagnostics.Debug.WriteLine(result);
			}
		}

		public static void ProgramChange(int channel, int patch)
		{
			if (Handle != IntPtr.Zero)
			{
				var result = midiOutShortMsg(Handle, 0xC0u | (uint)channel | ((uint)patch << 8));

				//System.Diagnostics.Debug.WriteLine(result);
			}
		}

		public static void ControlChange(int channel, int control, int value)
		{
			if (Handle != IntPtr.Zero)
			{
				var result = midiOutShortMsg(Handle, 0xB0u | (uint)channel | ((uint)control << 8) | ((uint)value << 16));

				//System.Diagnostics.Debug.WriteLine(result);
			}
		}

		public static void PitchBendChange(int channel, int value)
		{
			if (Handle != IntPtr.Zero)
			{
				var value1 = value & 0x7f;
				var value2 = (value >> 7) & 0x7f;

				var result = midiOutShortMsg(Handle, 0xE0u | (uint)channel | ((uint)value1 << 8) | ((uint)value2 << 16));

				//System.Diagnostics.Debug.WriteLine(result);
			}
		}

		public static void Disable()
		{
			if (Handle != IntPtr.Zero)
			{
				var result = midiOutClose(Handle);

				Handle = IntPtr.Zero;

				//System.Diagnostics.Debug.WriteLine(result);
			}
		}

		public static class Controls
		{
			public const int Bank = 0x00;
			public const int Modulation = 0x01;
			public const int Breath = 0x02;
			public const int Foot = 0x04;
			public const int Portamento = 0x05;
			public const int Volume = 0x07;
			public const int Balance = 0x07;
			public const int Pan = 0x0A;
			public const int Expression = 0x0B;
			public const int SustainEnable = 0x40;
			public const int PortamentoEnable = 0x041;
			public const int SostenutoEnable = 0x42;
			public const int SoftPedalEnable = 0x43;
			public const int LegatoPedalEnable = 0x044;
			public const int HoldEnable = 0x45;
			public const int PortamentoControl = 0x54;
			public const int Reverb = 0x5B;
			public const int Tremolo = 0x5C;
			public const int Chorus = 0x5D;
			public const int Detune = 0x5E;
			public const int Phaser = 0x5F;
		}

		public static class Patches
		{
			public const int GrandPiano = 0;
			public const int DrawbarOrgan = 16;
			public const int PercussiveOrgan = 17;
			public const int RockOrgan = 18;
			public const int ChurchOrgan = 19;
			public const int ReedOrgan = 20;
			public const int AcousticBass = 32;
			public const int FingerBass = 33;
			public const int PickBass = 34;
			public const int FretlessBass = 35;
			public const int SlapBass = 36;
			public const int SlapBass2 = 37;
			public const int SynthBass = 38;
			public const int SynthBass2 = 39;
			public const int Contrabass = 43;
			public const int TremoloStrings = 44;
			public const int PizzicatoStrings = 45;
			public const int Harp = 46;
			public const int Strings = 48;
			public const int Strings2 = 49;
			public const int SynthStrings = 50;
			public const int SynthStrings2 = 51;
			public const int Trumpet = 56;
			public const int Trombone = 57;
			public const int Tuba = 58;
			public const int MutedTrumpet = 59;
			public const int FrenchHorn = 60;
			public const int BrassSection = 61;
			public const int SynthBrass = 62;
			public const int SynthBrass2 = 63;
			public const int Flute = 73;
			public const int SquareLead = 80;
			public const int SawtoothLead = 81;
			public const int CalliopeLead = 82;
			public const int ChiffLead = 83;
			public const int CharangLead = 84;
			public const int VoiceLead = 85;
			public const int FifthsLead = 86;
			public const int BassLead = 87;
			public const int NewAgePad = 88;
			public const int WarmPad = 89;
			public const int PolysynthPad = 90;
			public const int ChoirPad = 91;
			public const int BowedPad = 92;
			public const int MetallicPad = 93;
			public const int HaloPad = 94;
			public const int SweepPad = 95;
			public const int Soundtrack = 97;
			public const int Timpani = 47;
			public const int Gunshot = 127;
			public const int OrchestraHit = 55;
			public const int Seashore = 122;
			public const int Applause = 126;
			public const int Atmosphere = 99;
			public const int Violin = 40;
			public const int OverdrivenGuitar = 29;
			public const int DistortionGuitar = 30;
		}

		public static class Drums
		{
			public const int BassDrum = 35;
			public const int BassDrum2 = 36;
			public const int SnareDrum = 38;
			public const int SnareDrum2 = 40;
			public const int HiHat = 42;
			public const int OpenHiHat = 46;
			public const int HandClap = 39;
			public const int SplashCymbal = 55;
			public const int CrashCymbal = 49;
			public const int CrashCymbal2 = 57;
		}
	}
}
