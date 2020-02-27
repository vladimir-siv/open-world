namespace XEngine.Terrains
{
	public abstract class HeightMap
	{
		public abstract uint Length { get; }

		public abstract float GetHeight(float x, float z);
	}
}
