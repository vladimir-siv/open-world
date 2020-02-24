using System;
using System.Drawing;

using GlmNet;

namespace XEngine.Terrains
{
	using XEngine.Shading;
	using XEngine.Common;

	public class Terrain
	{
		public static Terrain GenerateFlat(float length, uint tiles, uint granularity = 1u)
		{
			var terrain = new Terrain(length, tiles, granularity);
			terrain.Generate();
			terrain.Shape = new GeometricShape(new ShapeData(terrain.Vertices, terrain.Indices));
			return terrain;
		}
		public static Terrain Generate(float length, uint tiles, Bitmap heightmap, float heightfactor = 25.0f)
		{
			if (heightmap == null) throw new ArgumentNullException(nameof(heightmap));
			if (heightmap.Width != heightmap.Height) throw new ArgumentException("Height map must be of equal dimensions.");
			var terrain = new Terrain(length, tiles, (uint)heightmap.Width - 1u);
			terrain.Generate(heightmap, heightfactor);
			terrain.Shape = new GeometricShape(new ShapeData(terrain.Vertices, terrain.Indices));
			return terrain;
		}

		public float Length { get; }
		public uint Tiles { get; }
		public uint Granularity { get; }

		private vertex[] Vertices = null;
		private int[] Indices = null;

		public GeometricShape Shape { get; private set; }

		private Terrain(float length, uint tiles, uint granularity)
		{
			if (length <= 0.0f) throw new ArgumentException("Length must be positive.");
			if (granularity == 0) throw new ArgumentException("Granularity must be positive.");
			if (tiles == 0) throw new ArgumentException("Tiles must be positive.");

			Length = length;
			Granularity = granularity;
			Tiles = tiles;
		}

		private void Generate(Bitmap heightmap = null, float heightfactor = 25.0f)
		{
			var vert_count = Granularity + 1u;
			float h(uint x, uint z) => heightmap?.GetPixel((int)x, (int)z).GetBrightness() - 0.5f ?? 0.0f;
			int index(uint x, uint z) => (int)(x + z * vert_count);

			Vertices = new vertex[vert_count * vert_count];
			Indices = new int[Granularity * Granularity * 6];

			var color = new vec3(+0.0f, +0.0f, +0.0f);
			var delta_xz = Length / Granularity;
			var delta_uv = (float)Tiles / Granularity;

			for (var z = 0u; z < vert_count; ++z)
			{
				for (var x = 0u; x < vert_count; ++x)
				{
					Vertices[index(x, z)] = new vertex
					(
						new vec3(x * delta_xz - Length / 2.0f, h(x, z) * heightfactor, z * delta_xz - Length / 2.0f),
						color,
						vector3.up,
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

			if (heightmap == null) return;

			for (var z = 0u; z < vert_count; ++z)
			{
				for (var x = 0u; x < vert_count; ++x)
				{
					var li = (int)x - 1;
					var ri = (int)x + 1;
					var di = (int)z - 1;
					var ui = (int)z + 1;

					var l = (li < 0) ? 0.0f : Vertices[index((uint)li, z)].position.y;
					var r = (ri >= vert_count) ? 0.0f : Vertices[index((uint)ri, z)].position.y;
					var d = (di < 0) ? 0.0f : Vertices[index(x, (uint)di)].position.y;
					var u = (ui >= vert_count) ? 0.0f : Vertices[index(x, (uint)ui)].position.y;

					Vertices[index(x, z)].normal = new vec3(l - r, 2.0f, d - u).normalize();
				}
			}
		}
	}
}
