using System.Drawing;
using SharpGL.SceneGraph.Assets;

namespace XEngine.Resources
{
	public static partial class Resource
	{
		public static Texture LoadTexture(string textureName)
		{
			var keyTextureName = textureName.ToLower();
			if (XEngineContext.Textures.TryGetValue(keyTextureName, out var found)) return found;
			
			using (var stream = ManifestResourceManager.LoadFromResources($"Textures/{textureName}.bmp"))
			{
				using (var bmp = new Bitmap(stream))
				{
					var texture = new Texture();
					texture.Create(XEngineContext.Graphics, bmp);
					XEngineContext.Textures.Add(keyTextureName, texture);
					return texture;
				}
			}
		}

		public static Texture LoadPNGTexture(string textureName)
		{
			var keyTextureName = textureName.ToLower();
			if (XEngineContext.Textures.TryGetValue(keyTextureName, out var found)) return found;

			using (var stream = ManifestResourceManager.LoadFromResources($"Textures/{textureName}.png"))
			{
				using (var img = Image.FromStream(stream))
				{
					using (var bmp = new Bitmap(img))
					{
						var texture = new Texture();
						texture.Create(XEngineContext.Graphics, bmp);
						XEngineContext.Textures.Add(keyTextureName, texture);
						return texture;
					}
				}
			}
		}
	}
}
