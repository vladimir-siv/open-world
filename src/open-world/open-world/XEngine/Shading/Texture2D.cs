using System;
using System.Drawing;
using System.Drawing.Imaging;

using SharpGL;

namespace XEngine.Shading
{
	using XEngine.Resources;

	public class Texture2D : Texture
	{
		public static Texture FindBMP(string name) => Find($"{name}.bmp");
		public static Texture FindPNG(string name) => Find($"{name}.png");
		public static Texture Find(string name)
		{
			var name_key = name.ToLower();

			if (XEngineContext.Textures.TryGetValue(name_key, out var found))
			{
				return found;
			}

			var bitmap = name.EndsWith(".bmp")
				?
				Resource.LoadTexture(name.Remove(name.Length - 4))
				:
				Resource.LoadCustomTexture(name)
			;
			
			var texture = new Texture2D();
			texture.Image = bitmap;
			
			bitmap.Dispose();

			XEngineContext.Textures.Add(name_key, texture);
			return texture;
		}

		protected override uint TextureType => OpenGL.GL_TEXTURE_2D;

		public int Width { get; private set; } = 0;
		public int Height { get; private set; } = 0;

		private Texture2D() { }

		private Bitmap Image
		{
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));

				var gl = XEngineContext.Graphics;

				Width = value.Width;
				Height = value.Height;

				Activate();
				//gl.BindTexture(OpenGL.GL_TEXTURE_2D, TextureId);

				var bitmapData = value.LockBits
				(
					new Rectangle(0, 0, value.Width, value.Height),
					ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb
				);

				gl.TexImage2D
				(
					OpenGL.GL_TEXTURE_2D,
					0,
					OpenGL.GL_RGBA,
					Width,
					Height,
					0,
					OpenGL.GL_BGRA,
					OpenGL.GL_UNSIGNED_BYTE,
					bitmapData.Scan0
				);

				value.UnlockBits(bitmapData);

				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
			}
		}
	}
}
