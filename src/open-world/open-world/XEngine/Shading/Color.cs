using GlmNet;

namespace XEngine.Shading
{
	public struct Color
	{
		public vec4 vectorized;

		public float r
		{
			get => vectorized.x;
			set => vectorized.x = value;
		}
		public float g
		{
			get => vectorized.y;
			set => vectorized.y = value;
		}
		public float b
		{
			get => vectorized.z;
			set => vectorized.z = value;
		}
		public float a
		{
			get => vectorized.w;
			set => vectorized.w = value;
		}

		public Color(vec3 rgb) : this(rgb.x, rgb.y, rgb.z) { }
		public Color(float r, float g, float b) : this(r, g, b, 1.0f) { }
		public Color(float r, float g, float b, float a) : this(new vec4(r, g, b, a)) { }
		public Color(vec4 rgba) { vectorized = rgba; }
	}
}
