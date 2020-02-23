using System.Collections.Generic;
using System.Drawing;
using SharpGL.SceneGraph.Assets;

namespace XEngine.Resources
{
	public static partial class Resource
	{
		private static Dictionary<string, Texture> LoadedTextures = new Dictionary<string, Texture>();

		public static Texture LoadTexture(string textureName)
		{
			var keyTextureName = textureName.ToLower();
			if (LoadedTextures.TryGetValue(keyTextureName, out var found)) return found;

			using (var stream = ManifestResourceManager.LoadFromResources($"{textureName}.bmp"))
			{
				using (var bmp = new Bitmap(stream))
				{
					var texture = new Texture();
					texture.Create(XEngineContext.Graphics, bmp);
					LoadedTextures.Add(keyTextureName, texture);
					return texture;
				}
			}
		}

		internal static void ReleaseAllTextures()
		{
			foreach (var texture in LoadedTextures) texture.Value.Destroy(XEngineContext.Graphics);
			LoadedTextures.Clear();
		}
	}
}
