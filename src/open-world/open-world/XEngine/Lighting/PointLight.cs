using GlmNet;

namespace XEngine.Lighting
{
	using XEngine.Shading;

	public struct PointLight
	{
		public vec3 position;
		public Color color;
		public float power;

		public PointLight(float x, float y, float z) : this(new vec3(x, y, z)) { }
		public PointLight(vec3 position) : this(position, new Color(1.0f, 1.0f, 1.0f), 60.0f) { }
		public PointLight(vec3 position, Color color) : this(position, color, 60.0f) { }
		public PointLight(vec3 position, Color color, float power)
		{
			this.position = position;
			this.color = color;
			this.power = power;
		}
	}
}
