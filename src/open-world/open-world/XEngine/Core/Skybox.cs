namespace XEngine.Core
{
	using XEngine.Shading;

	public class Skybox
	{
		public Color SkyColor { get; }

		public Skybox() : this(Color.Black) { }
		public Skybox(Color skycolor) { SkyColor = skycolor; }
	}
}
