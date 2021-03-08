using System;

namespace DxNicc2000
{
	internal class SceneFile
	{
		internal static byte[] Data;

		internal static void Load()
		{
			Data = Properties.Resources.scene1;
		}
	}
}