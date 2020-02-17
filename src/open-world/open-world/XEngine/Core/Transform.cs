using GlmNet;

namespace XEngine.Core
{
	public struct Transform
	{
		public vec3 position;
		public vec3 rotation;
		public vec3 scale;

		public Transform(vec3 position, vec3 rotation, vec3 scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}
	}
}
