using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;

namespace DxNicc2000
{
	internal class ProgramWindow
	{
		internal static Form Form;

		internal static void Show()
		{
			ModFile.Load();
			ModReader.Read();
			ModPlayer.Play();
			MidiPlayer.Play();

			SceneFile.Load();
			SceneReader.Position = 0;

			Form = new Form
			{
				StartPosition = FormStartPosition.CenterScreen,
				Size = new System.Drawing.Size(1024, 768),
				Text = "DX-NICCC-2000"
			};

			var size = Form.ClientSize;

			var d3d = new Direct3D();

			var device = new Device(d3d, 0, DeviceType.Hardware, Form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(size.Width, size.Height) { PresentationInterval = PresentInterval.One });

			var effect = Effect.FromString(device, @"
float4x4 worldViewProjection;

struct VS_IN
{
	float4 position : POSITION;
	float4 color : COLOR0;
};

struct PS_IN
{
	float4 position : POSITION;
	float4 color : COLOR0;
};

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.position = mul(input.position,worldViewProjection);
	output.color=input.color;

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	return input.color;
}

technique Main { 
 	pass P0 { 
 		VertexShader = compile vs_2_0 VS(); 
         PixelShader  = compile ps_2_0 PS();
	}
}", ShaderFlags.None);

			var vertexElements = new[] {
				new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};

			var vertexDeclaration = new VertexDeclaration(device, vertexElements);

			var frame = 0;
			var start = DateTime.Now;

			RenderLoop.Run(Form, () =>
			{
				ModPlayer.Update();
				MidiPlayer.Update();

				//Debug.WriteLine(ModPlayer.Position.ToString() + ":" + ModPlayer.Division);

				//for (var channel = 0; channel < 4; channel++)
				//{
				//	if (ModPlayer.ChannelTriggers[channel])
				//		Debug.WriteLine(channel.ToString() + ": " + ModPlayer.ChannelSamples[channel] + ": " + ModPlayer.ChannelPitches[channel]);
				//}

				if (size != Form.ClientSize)
				{
					size = Form.ClientSize;

					if (size.Width != 0 && size.Height != 0)
						device.Reset(new PresentParameters(size.Width, size.Height) { PresentationInterval = PresentInterval.One });
				}

				frame++;

				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new RawColorBGRA(0, 0, 0, 255), 1.0f, 0);
				device.BeginScene();

				//effect.Technique = effect.GetTechnique(0);
				effect.Begin(FX.DoNotSaveState);

				effect.BeginPass(0);

				// Update Camera
				var projection = Matrix.OrthoOffCenterLH(0, 255, 199, 0, 1, 100);
				var view = Matrix.Identity;
				var world = Matrix.Identity;

				var worldViewProjection = world * view * projection;

				effect.SetValue("worldViewProjection", worldViewProjection);

				// Draw Scene
				device.SetRenderState(RenderState.CullMode, Cull.None);
				device.VertexDeclaration = vertexDeclaration;

				if((frame % 4) == 1)
					SceneReader.Read();

				if (SceneReader.Verteces == null)
				{
					// Draw Polygons
					foreach (var polygon in SceneReader.Polygons)
					{
						var verteces = polygon.Verteces.Select(x => Vertex(x, polygon.Color, SceneReader.Palette)).ToArray();

						device.DrawUserPrimitives(PrimitiveType.TriangleFan, verteces.Length - 2, verteces);
					}
				}
				else
				{
					// Draw Indexed Polygons
					foreach (var polygon in SceneReader.Polygons)
					{
						var verteces = polygon.Indeces.Select(x => Vertex(x, polygon.Color, SceneReader.Palette, SceneReader.Verteces)).ToArray();

						device.DrawUserPrimitives(PrimitiveType.TriangleFan, verteces.Length - 2, verteces);
					}
				}

				effect.EndPass();
				effect.End();

				device.EndScene();
				device.Present();
			});

			Debug.WriteLine(frame / (DateTime.Now - start).TotalSeconds);
		}

		private static SceneVertex Vertex(Point point, int color, ushort[] palette)
		{
			return new SceneVertex
			{
				Position = new Vector4(point.X, point.Y, 10, 1),
				Color = new Vector4(
					((palette[color] >> 8) & 0x07) / 7.0f,
					((palette[color] >> 4) & 0x07) / 7.0f,
					((palette[color] >> 0) & 0x07) / 7.0f,
					1.0f)
			};
		}

		private static SceneVertex Vertex(int point, int color, ushort[] palette, Point[] verteces)
		{
			return new SceneVertex
			{
				Position = new Vector4(verteces[point].X, verteces[point].Y, 10, 1),
				Color = new Vector4(
					((palette[color] >> 8) & 0x07) / 7.0f,
					((palette[color] >> 4) & 0x07) / 7.0f,
					((palette[color] >> 0) & 0x07) / 7.0f,
					1.0f)
			};
		}

		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		private struct SceneVertex
		{
			public Vector4 Position;
			public Vector4 Color;
		}
	}
}