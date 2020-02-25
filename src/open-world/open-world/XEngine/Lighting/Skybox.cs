using System;
using System.Drawing;
using System.Drawing.Imaging;

using SharpGL;
using GlmNet;

namespace XEngine.Lighting
{
	using XEngine.Core;
	using XEngine.Shading;
	using XEngine.Common;

	public class Skybox : IDisposable
	{
		private static readonly uint[] Temp = new uint[1];

		private struct TextureData : IDisposable
		{
			public static string Side(uint i)
			{
				switch (i)
				{
					case 0u: return "right";
					case 1u: return "left";
					case 2u: return "top";
					case 3u: return "bottom";
					case 4u: return "back";
					case 5u: return "front";
					default: return null;
				}
			}

			private readonly Bitmap bitmap;
			private readonly BitmapData data;

			public int Width => bitmap.Width;
			public int Height => bitmap.Height;
			public IntPtr Data => data.Scan0;

			public TextureData(Bitmap bitmap)
			{
				this.bitmap = bitmap;
				this.data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			}

			public void Dispose()
			{
				bitmap.UnlockBits(data);
				bitmap.Dispose();
			}
		}
		private class SkyboxShape : GeometricShape
		{
			public static GeometricShape Geometry { get; } = new SkyboxShape();

			private SkyboxShape() : base
			(
				new ShapeData
				(
					ShapeData.Positions3f
					(
						-0.5f, +0.5f, -0.5f,
						-0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,
						+0.5f, +0.5f, -0.5f,
						-0.5f, +0.5f, -0.5f,

						-0.5f, -0.5f, +0.5f,
						-0.5f, -0.5f, -0.5f,
						-0.5f, +0.5f, -0.5f,
						-0.5f, +0.5f, -0.5f,
						-0.5f, +0.5f, +0.5f,
						-0.5f, -0.5f, +0.5f,

						+0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,

						-0.5f, -0.5f, +0.5f,
						-0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, -0.5f, +0.5f,
						-0.5f, -0.5f, +0.5f,

						-0.5f, +0.5f, -0.5f,
						+0.5f, +0.5f, -0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						-0.5f, +0.5f, +0.5f,
						-0.5f, +0.5f, -0.5f,

						-0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f, +0.5f,
						+0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f, +0.5f,
						+0.5f, -0.5f, +0.5f
					)
				)
			)
			{
				Attributes = VertexAttribute.POSITION;
				KeepAlive = true;
			}
		}

		private static TextureData LoadTexture(string name, string side) => new TextureData(ManifestResourceManager.LoadAsBitmap($"Textures/Skyboxes/{name}/{side}.png"));

		public static Skybox Black { get; } = new Skybox(Color.Black);
		public static Skybox Default { get; } = new Skybox(Color.DeepSky);

		public static Skybox Find(string name) => Find(name, Color.Gray);
		public static Skybox Find(string name, Color skycolor)
		{
			var name_key = name.ToLower();

			if (XEngineContext.Skyboxes.TryGetValue(name_key, out var found)) return found;

			var gl = XEngineContext.Graphics;
			gl.GenTextures(1, Temp);

			var skybox = new Skybox(skycolor)
			{
				plain = false,
				id = Temp[0]
			};

			gl.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, skybox.id);

			for (var i = 0u; i < 6; ++i)
			{
				try
				{
					var texture = LoadTexture(name, TextureData.Side(i));

					gl.TexImage2D
					(
						OpenGL.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i,
						0,
						OpenGL.GL_RGBA,
						texture.Width,
						texture.Height,
						0,
						OpenGL.GL_BGRA,
						OpenGL.GL_UNSIGNED_BYTE,
						texture.Data
					);

					texture.Dispose();
				}
				catch (Exception ex)
				{
					gl.DeleteTextures(1, Temp);
					throw ex;
				}
			}

			gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

			XEngineContext.Skyboxes.Add(name_key, skybox);

			mesh = mesh ?? new Mesh() { shape = SkyboxShape.Geometry };

			return skybox;
		}

		private static Mesh _mesh = null;
		internal static Mesh mesh
		{
			get
			{
				return _mesh;
			}
			set
			{
				if (value == _mesh) return;
				_mesh?.Dispose();
				_mesh = value;
			}
		}

		private bool plain = true;
		private uint id = 0;

		private float rotation = 0.0f;
		private mat4 transform = mat4.identity();
		private float[] transform_cache = new float[16];

		public float RotationSpeed { get; set; } = 1.0f;

		public Color SkyColor { get; }

		private Skybox(Color skycolor) { SkyColor = skycolor; }

		public void Draw(float scale)
		{
			if (plain) return;
			var gl = XEngineContext.Graphics;
			var shader = XEngineContext.SkyboxShader;

			rotation += RotationSpeed * Time.DeltaTime / 1000.0f;

			var camera = SceneManager.CurrentScene.MainCamera;
			transform = camera.WorldToView.copy_to(transform);
			transform = quaternion.euler(transform, 0.0f, rotation, 0.0f);
			transform_cache = transform.serialize(transform_cache);
			transform_cache[12] = transform_cache[13] = transform_cache[14] = 0.0f;

			shader.Use();
			gl.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, id);
			gl.UniformMatrix4(shader.Project, 1, false, camera.ViewToProjectData);
			gl.UniformMatrix4(shader.View, 1, false, transform_cache);
			shader.SetScalar("scale", scale);
			shader.SetVec4("sky_color", SkyColor.r, SkyColor.g, SkyColor.b, SkyColor.a);

			mesh.Activate();
			gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, SkyboxShape.Geometry.VertexCount);
		}

		public void Dispose()
		{
			if (plain) return;
			Temp[0] = id;
			var gl = XEngineContext.Graphics;
			gl.DeleteTextures(1, Temp);
			id = 0;
		}
	}
}
