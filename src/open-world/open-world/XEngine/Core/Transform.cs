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

		public static Transform operator +(Transform t1, Transform t2)
		{
			return new Transform(t1.position + t2.position, t1.rotation + t2.rotation, new vec3(t1.scale.x * t2.scale.x, t1.scale.y * t2.scale.y, t1.scale.z * t2.scale.z));
		}
	}
}
