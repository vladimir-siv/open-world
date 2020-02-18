using System;
using GlmNet;

namespace XEngine.Extensions
{
	public static class CommonMath
	{
		public static uint BitCount(this uint v)
		{
			v -= ((v >> 1) & 0x55555555);
			v = (v & 0x33333333) + ((v >> 2) & 0x33333333);
			return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
		}

		public static float ToRad(this float v)
		{
			return v * (float)Math.PI / 180.0f;
		}

		public static float ToDeg(this float v)
		{
			return v * 180.0f / (float)Math.PI;
		}
	}

	public static class vec
	{
		public static readonly vec3 forward		= new vec3(+0.0f, +0.0f, -1.0f);
		public static readonly vec3 backward	= new vec3(+0.0f, +0.0f, +1.0f);
		public static readonly vec3 right = new vec3(+1.0f, +0.0f, +0.0f);
		public static readonly vec3 left		= new vec3(-1.0f, +0.0f, +0.0f);
		public static readonly vec3 up			= new vec3(+0.0f, +1.0f, +0.0f);
		public static readonly vec3 down		= new vec3(+0.0f, -1.0f, +0.0f);
	}
}
