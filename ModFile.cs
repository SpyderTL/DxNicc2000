using System;

namespace DxNicc2000
{
	internal class ModFile
	{
		internal static byte[] Data;

		internal static void Load()
		{
			Data = Properties.Resources.chcknbnk;
		}
	}
}