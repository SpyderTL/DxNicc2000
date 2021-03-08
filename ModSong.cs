using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxNicc2000
{
	internal static class ModSong
	{
		internal static string Title;
		internal static int[] Sequence;
		internal static Pattern[] Patterns;
		internal static int[] SampleVolumes;

		internal struct Pattern
		{
			internal Division[] Divisions;
		}

		internal struct Division
		{
			internal Channel[] Channels;
		}

		internal struct Channel
		{
			internal uint Sample;
			internal uint Parameter;
			internal uint Effect;
		}

		internal static readonly int[] NotePitches = new int[]
		{
			1712,1616,1525,1440,1357,1281,1209,1141,1077,1017, 961, 907,
			856, 808, 762, 720, 678, 640, 604, 570, 538, 508, 480, 453,
			428, 404, 381, 360, 339, 320, 302, 285, 269, 254, 240, 226,
			214, 202, 190, 180, 170, 160, 151, 143, 135, 127, 120, 113,
			107, 101,  95,  90,  85,  80,  76,  71,  67,  64,  60,  57
		};
	}
}
