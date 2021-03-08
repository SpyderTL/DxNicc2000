using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace DxNicc2000
{
	internal class SceneReader
	{
		internal static int Position;

		internal static bool ClearScreen;
		internal static ushort[] Palette = new ushort[16];
		internal static Polygon[] Polygons;
		internal static Point[] Verteces;
		internal static bool Done;

		internal static bool Read()
		{
			if(Done || Position >= SceneFile.Data.Length)
				return false;

			var flags = ReadByte();

			ClearScreen = (flags & 0x01) != 0;
			var palette = (flags & 0x02) != 0;
			var indexed = (flags & 0x04) != 0;

			if (palette)
			{
				var mask = ReadUShort();

				for (var x = 0; x < 16; x++)
				{
					if ((mask & 0x8000) != 0)
					{
						var color = ReadUShort();

						Palette[x] = color;
					}

					mask <<= 1;
				}
			}

			var polygons = new List<Polygon>();

			if (indexed)
			{
				int count = ReadByte();

				Verteces = Enumerable.Range(0, count).Select(x => new Point(ReadByte(), ReadByte())).ToArray();

				while (true)
				{
					var type = ReadByte();

					if (type == 0xFF)
						break;
					else if (type == 0xFE)
					{
						var block = Position >> 16;
						block++;
						Position = block << 16;

						break;
					}
					else if (type == 0xFD)
					{
						Done = true;
						break;
					}

					var color = type >> 4;
					count = type & 0x0F;

					var indeces = Enumerable.Range(0, count).Select(x => ReadByte()).ToArray();

					var polygon = new Polygon { Color = color, Indeces = indeces };

					polygons.Add(polygon);
				}
			}
			else
			{
				Verteces = null;

				while (true)
				{
					var type = ReadByte();

					if (type == 0xFF)
						break;
					else if (type == 0xFE)
					{
						var block = Position >> 16;
						block++;
						Position = block << 16;

						break;
					}
					else if (type == 0xFD)
					{
						Done = true;
						break;
					}

					var color = type >> 4;
					var count = type & 0x0F;

					var verteces = Enumerable.Range(0, count).Select(x => new Point(ReadByte(), ReadByte())).ToArray();

					var polygon = new Polygon { Color = color, Verteces = verteces };

					polygons.Add(polygon);
				}
			}

			Polygons = polygons.ToArray();

			return true;
		}

		private static byte ReadByte()
		{
			return SceneFile.Data[Position++];
		}

		private static ushort ReadUShort()
		{
			return (ushort)((SceneFile.Data[Position++] << 8) | SceneFile.Data[Position++]);
		}

		internal struct Polygon
		{
			internal int Color;
			internal Point[] Verteces;
			internal byte[] Indeces;
		}
	}
}