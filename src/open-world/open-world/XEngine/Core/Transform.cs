using GlmNet;

namespace XEngine.Core
{
	using XEngine.Common;

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

		public SpaceUnits WorldSpaceUnits
		{
			get
			{
				var transform4d = quaternion.euler(rotation);

				return new SpaceUnits
				(
					(transform4d * vector4.forward).to_vec3(),
					(transform4d * vector4.right).to_vec3(),
					(transform4d * vector4.up).to_vec3()
				);
			}
		}

		public static Transform operator +(Transform t1, Transform t2)
		{
			return new Transform(t1.position + t2.position, t1.rotation + t2.rotation, new vec3(t1.scale.x * t2.scale.x, t1.scale.y * t2.scale.y, t1.scale.z * t2.scale.z));
		}
	}

	public struct SpaceUnits
	{
		public vec3 forward;
		public vec3 right;
		public vec3 up;

		public SpaceUnits(vec3 forward, vec3 right, vec3 up)
		{
			this.forward = forward;
			this.right = right;
			this.up = up;
		}
	}
}
