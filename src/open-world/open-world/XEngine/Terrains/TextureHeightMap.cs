using System;
using System.Drawing;

namespace XEngine.Terrains
{
	using XEngine.Resources;

	public class TextureHeightMap : HeightMap, IDisposable
	{
		private Bitmap Texture = null;
		public float HeightFactor { get; private set; }
		public bool Negative { get; private set; }
		public override uint Length => (uint)Texture.Width;

		public TextureHeightMap(string textureName, float heightfactor = 20.0f, bool negative = true)
		{
			Texture = Resource.LoadCustomTexture(textureName);
			HeightFactor = heightfactor;
			Negative = negative;

			if (Texture.Width != Texture.Height)
			{
				Texture.Dispose();
				throw new FormatException("The texture must be of the same width and height.");
			}
		}

		public override float GetHeight(float x, float z)
		{
			var h = Texture.GetPixel((int)x, (int)z).GetBrightness();
			if (Negative) h = (h - 0.5f) * 2.0f;
			return h * HeightFactor;
		}

		public void Dispose()
		{
			Texture.Dispose();
			Texture = null;
		}
	}
}
