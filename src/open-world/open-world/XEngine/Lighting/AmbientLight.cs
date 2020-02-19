namespace XEngine.Lighting
{
	using XEngine.Shading;

	public struct AmbientLight
	{
		public static readonly AmbientLight Bright = new AmbientLight(new Color(1.0f, 1.0f, 1.0f), 0.25f);

		public Color color;
		public float power;

		public AmbientLight(Color color, float power)
		{
			this.color = color;
			this.power = power;
		}
	}
}
