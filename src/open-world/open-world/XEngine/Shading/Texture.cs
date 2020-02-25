using System;

using SharpGL;

namespace XEngine.Shading
{
	public abstract class Texture : IDisposable
	{
		private readonly uint[] glTextureArray = new uint[1] { 0u };

		public uint TextureId => glTextureArray[0];

		protected abstract uint TextureType { get; }

		protected Texture()
		{
			var gl = XEngineContext.Graphics;
			gl.GenTextures(1, glTextureArray);
		}

		public void Activate(uint index = 0u)
		{
			var gl = XEngineContext.Graphics;
			gl.ActiveTexture(OpenGL.GL_TEXTURE0 + index);
			gl.BindTexture(TextureType, glTextureArray[0]);
		}

		public virtual void Dispose()
		{
			if (TextureId == 0) throw new InvalidOperationException("Already disposed."); ;
			var gl = XEngineContext.Graphics;
			gl.DeleteTextures(1, glTextureArray);
			glTextureArray[0] = 0u;
		}
	}
}
