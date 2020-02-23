using System;
using GlmNet;

namespace XEngine.Terrains
{
	using XEngine.Shading;

	public class Terrain
	{
		public static Terrain GenerateFlat(vec2 size, uint granularity, uint tiles)
		{
			if (size.x != size.y) throw new ArgumentException("Both dimensions in size must be equal.");
			return GenerateFlat(size.x, granularity, tiles);
		}
		public static Terrain GenerateFlat(float length, uint granularity, uint tiles)
		{
			var terrain = new Terrain(length, granularity, tiles);
			terrain.Generate();
			terrain.Shape = new GeometricShape(new ShapeData(terrain.Vertices, terrain.Indices));
			return terrain;
		}

		public float Length { get; }
		public uint Granularity { get; }
		public uint Tiles { get; }

		private vertex[] Vertices = null;
		private int[] Indices = null;

		public GeometricShape Shape { get; private set; }

		private Terrain(float length, uint granularity, uint tiles)
		{
			if (length <= 0.0f) throw new ArgumentException("Length must be positive.");
			if (granularity == 0) throw new ArgumentException("Granularity must be positive.");
			if (tiles == 0) throw new ArgumentException("Tiles must be positive.");

			Length = length;
			Granularity = granularity;
			Tiles = tiles;
		}

		private void Generate()
		{
			var vert_count = Granularity + 1;

			Vertices = new vertex[vert_count * vert_count];
			Indices = new int[Granularity * Granularity * 6];

			var color = new vec3(+0.0f, +0.0f, +0.0f);
			var normal = new vec3(+0.0f, +1.0f, +0.0f);
			var delta_xz = Length / Granularity;
			var delta_uv = (float)Tiles / Granularity;

			int index(uint x, uint z) => (int)(x + z * vert_count);

			for (var z = 0u; z < vert_count; ++z)
			{
				for (var x = 0u; x < vert_count; ++x)
				{
					Vertices[index(x, z)] = new vertex
					(
						new vec3(x * delta_xz - Length / 2.0f, 0.0f, z * delta_xz - Length / 2.0f),
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
					var i = z * Granularity + x;
					Indices[i * 6u + 0u] = index(x, z);
					Indices[i * 6u + 1u] = index(x, z + 1u);
					Indices[i * 6u + 2u] = index(x + 1u, z);
					Indices[i * 6u + 3u] = index(x + 1u, z);
					Indices[i * 6u + 4u] = index(x, z + 1u);
					Indices[i * 6u + 5u] = index(x + 1u, z + 1u);
				}
			}
		}
	}
}
