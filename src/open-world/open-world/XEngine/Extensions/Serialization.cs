﻿using System;
using GlmNet;

namespace XEngine.Extensions
{
	using XEngine.Shading;

	public static class Serialization
	{
		public static float[] Serialize(this Color[] colors, bool includeAlpha = true)
		{
			if (colors == null) throw new ArgumentNullException(nameof(colors));

			if (includeAlpha)
			{
				var serialized = new float[colors.Length * 4];

				for (var i = 0; i < colors.Length; ++i)
				{
					serialized[i * 4 + 0] = colors[i].r;
					serialized[i * 4 + 1] = colors[i].g;
					serialized[i * 4 + 2] = colors[i].b;
					serialized[i * 4 + 3] = colors[i].a;
				}

				return serialized;
			}
			else
			{
				var serialized = new float[colors.Length * 3];

				for (var i = 0; i < colors.Length; ++i)
				{
					serialized[i * 4 + 0] = colors[i].r;
					serialized[i * 4 + 1] = colors[i].g;
					serialized[i * 4 + 2] = colors[i].b;
				}

				return serialized;
			}
		}
		public static float[] Serialize(this vec2[] values)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));

			var serialized = new float[values.Length * 2];

			for (var i = 0; i < values.Length; ++i)
			{
				serialized[i * 2 + 0] = values[i].x;
				serialized[i * 2 + 1] = values[i].y;
			}

			return serialized;
		}
		public static float[] Serialize(this vec3[] values)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));

			var serialized = new float[values.Length * 3];

			for (var i = 0; i < values.Length; ++i)
			{
				serialized[i * 3 + 0] = values[i].x;
				serialized[i * 3 + 1] = values[i].y;
				serialized[i * 3 + 2] = values[i].z;
			}

			return serialized;
		}
		public static float[] Serialize(this vec4[] values)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));

			var serialized = new float[values.Length * 4];

			for (var i = 0; i < values.Length; ++i)
			{
				serialized[i * 4 + 0] = values[i].x;
				serialized[i * 4 + 1] = values[i].y;
				serialized[i * 4 + 2] = values[i].z;
				serialized[i * 4 + 3] = values[i].w;
			}

			return serialized;
		}
		public static float[] Serialize(this mat2[] values)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));

			var serialized = new float[values.Length * 4];

			for (var i = 0; i < values.Length; ++i)
				for (var c = 0; c < 4; ++c) 
					serialized[i * 4 + c] = values[i][c / 2, c % 2];

			return serialized;
		}
		public static float[] Serialize(this mat3[] values)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));

			var serialized = new float[values.Length * 9];

			for (var i = 0; i < values.Length; ++i)
				for (var c = 0; c < 9; ++c)
					serialized[i * 9 + c] = values[i][c / 3, c % 3];

			return serialized;
		}
		public static float[] Serialize(this mat4[] values)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));

			var serialized = new float[values.Length * 16];

			for (var i = 0; i < values.Length; ++i)
				for (var c = 0; c < 16; ++c)
					serialized[i * 16 + c] = values[i][c / 4, c % 4];

			return serialized;
		}
	}
}
