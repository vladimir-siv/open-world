using System;
using GlmNet;

namespace XEngine.Core
{
	using XEngine.Common;

	public sealed class Camera
	{
		public GameObject Following { get; set; } = null;

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

		public mat4 WorldToView { get; private set; }
		public mat4 ViewToProject { get; private set; }

		public float[] WorldToViewData { get; private set; } = new float[16];
		public float[] ViewToProjectData { get; private set; } = new float[16];

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

			if (change)
			{
				ViewToProject = glm.perspective(fov * (float)Math.PI / 180.0f, aspect, near, far);
				ViewToProject.serialize(ViewToProjectData);
			}
		}

		public void Adjust()
		{
			var transform = Following?.transform_model.clone() ?? mat4.identity();
			var rotate = Following?.rotate_model.clone() ?? mat4.identity();

			transform = glm.translate(transform, LocalPosition);
			transform = quaternion.euler(transform, LocalRotation);
			rotate = quaternion.euler(rotate, LocalRotation);

			Position = (transform * vector4.neutral).to_vec3();
			Rotation = (rotate * vector4.neutral).to_vec3();

			ViewDirection = (rotate * vector4.forward).to_vec3();
			StrafeDirection = (rotate * vector4.right).to_vec3();
			FlyDirection = (rotate * vector4.up).to_vec3();

			WorldToView = glm.lookAt(Position, Position + ViewDirection, FlyDirection);
			WorldToView.serialize(WorldToViewData);
		}
	}
}
