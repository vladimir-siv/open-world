namespace XEngine.Core
{
	using XEngine.Shading;

	public class Skybox
	{
		public Color SkyColor { get; }

		public Skybox() : this(Color.DeepSky) { }
		public Skybox(Color skycolor) { SkyColor = skycolor; }
	}
}
