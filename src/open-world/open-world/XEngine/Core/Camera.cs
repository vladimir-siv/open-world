using System;
using GlmNet;

namespace XEngine.Core
{
	using XEngine.Common;

	public sealed class Camera
	{
		public GameObject Following { get; private set; } = null;

		public float FieldOfView { get; private set; } = 60.0f;
		public float AspectRatio { get; private set; } = 1.0f;
		public float NearClipPlane { get; private set; } = +0.1f;
		public float FarClipPlane { get; private set; } = +100.0f;

		public vec3 ViewDirection { get; private set; } = vector3.forward;
		public vec3 StrafeDirection { get; private set; } = vector3.right;
		public vec3 FlyDirection { get; private set; } = vector3.up;

		public vec3 LocalPosition { get; set; } = vector3.zero;
		public vec3 Position { get; private set; } = vector3.zero;
		public vec3 LocalRotation { get; set; } = vector3.zero;
		public vec3 Rotation { get; private set; } = vector3.zero;

		public mat4 WorldToView => glm.lookAt(Position, Position + ViewDirection, FlyDirection);
		public mat4 ViewToProject { get; private set; }

		public Camera()
		{
			SetProjection(FieldOfView, AspectRatio, NearClipPlane, FarClipPlane);
		}

		public void SetFieldOfView(float fov) => SetProjection(fov, AspectRatio, NearClipPlane, FarClipPlane);
		public void SetAspectRatio(float aspect) => SetProjection(FieldOfView, aspect, NearClipPlane, FarClipPlane);
		public void SetNearClipPlane(float near) => SetProjection(FieldOfView, AspectRatio, near, FarClipPlane);
		public void SetFarClipPlane(float far) => SetProjection(FieldOfView, AspectRatio, NearClipPlane, far);
		public void SetClipPlane(float near, float far) => SetProjection(FieldOfView, AspectRatio, near, far);
		public void SetProjection(float fov, float aspect, float near, float far)
		{
			if (fov <= 0.0f) throw new ArgumentException("FieldOfView must be a positive float.");
			if (aspect <= 0.0f) throw new ArgumentException("AspectRatio must be a positive float.");
			if (near <= 0.0f) throw new ArgumentException("NearClipPlane must be a positive float.");
			if (far <= 0.0f) throw new ArgumentException("FarClipPlane must be a positive float.");

			var change = false;

			if (fov != FieldOfView)
			{
				FieldOfView = fov;
				change = true;
			}

			if (aspect != AspectRatio)
			{
				AspectRatio = aspect;
				change = true;
			}

			if (near != NearClipPlane)
			{
				NearClipPlane = near;
				change = true;
			}

			if (far != FarClipPlane)
			{
				FarClipPlane = far;
				change = true;
			}

			if (change) ViewToProject = glm.perspective(fov * (float)Math.PI / 180.0f, aspect, near, far);
		}

		public void Follow(GameObject gameObject) => Follow(gameObject, LocalPosition, LocalRotation);
		public void Follow(GameObject gameObject, vec3 localPosition, vec3 localRotation)
		{
			if (Following == gameObject) return;
			Following = gameObject;
			LocalPosition = localPosition;
			LocalRotation = localRotation;
		}

		public void Adjust()
		{
			var parentRotate = Following?.transform.rotation ?? vector3.zero;
			var parentTranslate = Following?.transform.position ?? vector3.zero;

			var transform4d = mat4.identity();
			transform4d = glm.translate(transform4d, parentTranslate);
			transform4d = quaternion.euler(transform4d, parentRotate);
			transform4d = glm.translate(transform4d, LocalPosition);
			transform4d = quaternion.euler(transform4d, LocalRotation);

			var rotate4d = quaternion.euler(LocalRotation + parentRotate);

			var zero4d = new vec4(0.0f, 0.0f, 0.0f, 1.0f);
			Position = (transform4d * zero4d).to_vec3();
			Rotation = (rotate4d * zero4d).to_vec3();

			ViewDirection = (rotate4d * vector4.forward).to_vec3();
			StrafeDirection = (rotate4d * vector4.right).to_vec3();
			FlyDirection = (rotate4d * vector4.up).to_vec3();
		}
	}
}
