using System;

namespace XEngine.Shapes
{
	using XEngine.Data;
	using XEngine.Extensions;

	public struct ShapeData : IDisposable
	{
		public vertex[] Vertices { get; private set; }
		public ushort[] Indices { get; private set; }

		public float[] SerializeData(VertexAttribute attributes)
		{
			if (attributes == VertexAttribute.NONE) throw new ArgumentException("Cannot serialize shape data with no selected attributes.");

			var attribcount = Math.Min(((uint)attributes).BitCount(), vertex.AttribCount);
			var vertexsize = vertex.AttribSize * attribcount;
			var dataLength = Vertices.Length * vertexsize;
			var data = new float[dataLength];

			for (int i = 0; i < Vertices.Length; ++i)
			{
				var vertex = Vertices[i];
				var c = 0;

				if (attributes.HasFlag(VertexAttribute.POSITION))
				{
					data[i * vertexsize + c++] = vertex.position.x;
					data[i * vertexsize + c++] = vertex.position.y;
					data[i * vertexsize + c++] = vertex.position.z;
				}

				if (attributes.HasFlag(VertexAttribute.COLOR))
				{
					data[i * vertexsize + c++] = vertex.color.x;
					data[i * vertexsize + c++] = vertex.color.y;
					data[i * vertexsize + c++] = vertex.color.z;
				}

				if (attributes.HasFlag(VertexAttribute.NORMAL))
				{
					data[i * vertexsize + c++] = vertex.normal.x;
					data[i * vertexsize + c++] = vertex.normal.y;
					data[i * vertexsize + c++] = vertex.normal.z;
				}
			}

			return data;
		}

		public ShapeData(vertex[] vertices, ushort[] indices = null)
		{
			Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));

			if (indices == null)
			{
				indices = new ushort[Vertices.Length];

				for (ushort i = 0; i < indices.Length; ++i)
				{
					indices[i] = i;
				}
			}
			
			Indices = indices;
		}

		public void Dispose()
		{
			Vertices = null;
			Indices = null;
		}
	}
}
