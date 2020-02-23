using System;
using GlmNet;

namespace XEngine.Terrains
{
	using XEngine.Shading;

	public class Terrain
	{
		public static Terrain GenerateFlat(vec2 size, uint granularity, uint tiles)
		{
			var terrain = new Terrain(size, granularity, tiles);
			terrain.Generate();
			terrain.Shape = new GeometricShape(new ShapeData(terrain.Vertices, terrain.Indices));
			return terrain;
		}

		public vec2 Size { get; }
		public uint Granularity { get; }
		public uint Tiles { get; }

		private vertex[] Vertices = null;
		private ushort[] Indices = null;

		public GeometricShape Shape { get; private set; }

		private Terrain(vec2 size, uint granularity, uint tiles)
		{
			if (size.x <= 0 || size.y <= 0) throw new ArgumentException("Size must be positive across both dimensions.");
			if (size.x != size.y) throw new ArgumentException("Terrain must be square size (both dimensions equal).");
			if (granularity == 0) throw new ArgumentException("Granularity must be positive.");
			if (tiles == 0) throw new ArgumentException("Tiles must be positive.");

			Size = size;
			Granularity = granularity;
			Tiles = tiles;
		}

		private void Generate()
		{
			var vert_count = Granularity + 1;

			Vertices = new vertex[vert_count * vert_count];
			Indices = new ushort[Granularity * Granularity * 6];

			var color = new vec3(+0.0f, +0.0f, +0.0f);
			var normal = new vec3(+0.0f, +1.0f, +0.0f);
			var delta_xz = Size.x / Granularity;
			var delta_uv = (float)Tiles / Granularity;

			uint index(uint x, uint z) => x + z * vert_count;

			for (var z = 0u; z < vert_count; ++z)
			{
				for (var x = 0u; x < vert_count; ++x)
				{
					Vertices[index(x, z)] = new vertex
					(
						new vec3(x * delta_xz, 0.0f, z * delta_xz),
						color,
						normal,
						new vec2(x * delta_uv, z * delta_uv)
					);
				}
			}

			for (var z = 0u; z < Granularity; ++z)
			{
				for (var x = 0u; x < Granularity; ++x)
				{
					Indices[z * Granularity + x + 0u] = (ushort)index(x, z);
					Indices[z * Granularity + x + 1u] = (ushort)index(x, z + 1u);
					Indices[z * Granularity + x + 2u] = (ushort)index(x + 1u, z);
					Indices[z * Granularity + x + 3u] = (ushort)index(x + 1u, z);
					Indices[z * Granularity + x + 4u] = (ushort)index(x, z + 1u);
					Indices[z * Granularity + x + 5u] = (ushort)index(x + 1u, z + 1u);
				}
			}
		}
	}
}
