using System;
using SharpGL;

namespace XEngine.Shapes
{
	using XEngine.Data;
	using XEngine.Extensions;

	public class GeometricShape
	{
		protected ShapeData ShapeData { get; private set; }

		public uint AttribCount { get; private set; } = vertex.AttribCount;
		private void UpdateAttribCount() => AttribCount = Math.Min(((uint)Attributes).BitCount(), vertex.AttribCount);
		private VertexAttribute _Attributes = VertexAttribute.ALL;
		public VertexAttribute Attributes
		{
			get { return _Attributes; }
			set { _Attributes = value; UpdateAttribCount(); }
		}

		public vertex[] Vertices => ShapeData.Vertices;
		public ushort[] Indices => ShapeData.Indices;
		public float[] Data => ShapeData.SerializeData(Attributes);

		internal GeometricShape(ShapeData shapeData) { ShapeData = shapeData; }
		public virtual uint OpenGLShapeType => OpenGL.GL_TRIANGLES;

		protected uint GLAttribCount => AttribCount == 1u ? 0u : AttribCount;

		public virtual int GetAttribSize(uint index) => (int)vertex.AttribSize;
		public virtual uint GetAttribType(uint index) => OpenGL.GL_FLOAT;
		public virtual bool ShouldAttribNormalize(uint index) => false;
		public virtual int GetAttribStride(uint index) => (int)(vertex.AttribSize * GLAttribCount * sizeof(float));
		public virtual IntPtr GetAttribOffset(uint index) => new IntPtr(index * vertex.AttribSize * sizeof(float));
	}
}
