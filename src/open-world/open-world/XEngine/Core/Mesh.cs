using System;
using System.Threading.Tasks;
using SharpGL;

namespace XEngine.Core
{
	using XEngine.Models;
	using XEngine.Shading;

	public sealed class Mesh : IDisposable
	{
		private static Mesh CurrentBound = null;

		private uint[] ArrayIds = null;
		private uint[] BufferIds = null;

		public uint VertexArrayId => ArrayIds[0];
		public uint VertexBufferId => BufferIds[0];
		public uint IndexBufferId => BufferIds[1];

		private GeometricShape _shape = null;
		public GeometricShape shape
		{
			get
			{
				return _shape;
			}
			set
			{
				if (_shape == value) return;
				_shape = value;
				Generate();
				_shape.Dispose();
			}
		}
		
		private uint shared = 0u;

		public bool KeepAlive { get; set; } = false;
		public bool Disposed { get; private set; } = true;
		public bool Active => this == CurrentBound;
		
		public async Task LoadModel(string name, VertexAttribute attributes = VertexAttribute.ALL)
		{
			var model = await Model.Load(name);
			model.Attributes = attributes;
			shape = model;
		}

		public void Activate()
		{
			if (ArrayIds == null) throw new InvalidOperationException("Mesh disposed.");
			if (this == CurrentBound) return;
			var gl = XEngineContext.Graphics;
			gl.BindVertexArray(VertexArrayId);
			CurrentBound = this;
		}

		private void Generate()
		{
			if (shape == null) throw new InvalidOperationException("Shape not provided.");

			var gl = XEngineContext.Graphics;

			if (ArrayIds == null)
			{
				ArrayIds = new uint[1];
				BufferIds = new uint[2];

				gl.GenVertexArrays(ArrayIds.Length, ArrayIds);
				gl.BindVertexArray(ArrayIds[0]);
				gl.GenBuffers(BufferIds.Length, BufferIds);
			}
			else gl.BindVertexArray(ArrayIds[0]);

			gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, BufferIds[0]);
			gl.BufferData(OpenGL.GL_ARRAY_BUFFER, shape.Data, OpenGL.GL_STATIC_DRAW);

			gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, BufferIds[1]);
			gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, shape.Indices, OpenGL.GL_STATIC_DRAW);

			for (var i = 0u; i < shape.AttribCount; ++i)
			{
				gl.VertexAttribPointer(i, shape.GetAttribSize(i), shape.GetAttribType(i), shape.ShouldAttribNormalize(i), shape.GetAttribStride(i), shape.GetAttribOffset(i));
				gl.EnableVertexAttribArray(i);
			}

			Disposed = false;

			if (CurrentBound != null && CurrentBound != this && !CurrentBound.Disposed)
			{
				gl.BindVertexArray(CurrentBound.VertexArrayId);
			}
		}

		internal void Register()
		{
			++shared;
		}
		internal void Release()
		{
			if (shared == 0u) return;
			if (--shared == 0u && !KeepAlive) Dispose();
		}

		public void Dispose()
		{
			if (ArrayIds == null) throw new InvalidOperationException("Already disposed.");

			var gl = XEngineContext.Graphics;
			gl.DeleteBuffers(BufferIds.Length, BufferIds);
			gl.DeleteVertexArrays(ArrayIds.Length, ArrayIds);

			ArrayIds = null;
			BufferIds = null;

			_shape = null;

			shared = 0u;
			KeepAlive = false;
			Disposed = true;
		}
	}
}
