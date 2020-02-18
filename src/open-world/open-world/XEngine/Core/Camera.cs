using System;
using GlmNet;

namespace XEngine.Core
{
	public sealed class Camera
	{
		private static readonly vec3 IdentY = new vec3(0.0f, 1.0f, 0.0f);
		private vec3 StrafeDirection = new vec3(1.0f, 0.0f, 0.0f);

		public vec3 Position { get; set; } = new vec3(0.0f, 0.0f, 0.0f);

		public float RotationSpeed { get; set; } = 0.005f;
		public float MovementSpeed { get; set; } = 1.0f;

		public float FieldOfView { get; private set; } = 60.0f;
		public float AspectRatio { get; private set; } = 1.0f;
		public float NearClipPlane { get; private set; } = +0.1f;
		public float FarClipPlane { get; private set; } = +100.0f;

		public vec3 ViewDirection { get; private set; } = new vec3(0.0f, 0.0f, -1.0f);
		public mat4 WorldToView => glm.lookAt(Position, Position + ViewDirection, IdentY);
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

		public void Rotate(vec2 delta)
		{
			StrafeDirection = glm.cross(ViewDirection, IdentY);
			var rotx = glm.rotate(-delta.x * RotationSpeed, IdentY);
			var roty = glm.rotate(-delta.y * RotationSpeed, StrafeDirection);
			ViewDirection = (rotx * roty).to_mat3() * ViewDirection;
		}

		public void Move(vec3 delta)
		{
			Position += MovementSpeed * StrafeDirection * delta.x;
			Position += MovementSpeed * IdentY * delta.y;
			Position += MovementSpeed * ViewDirection * -delta.z;
		}

		public void LookAt(vec3 point)
		{
			ViewDirection = glm.normalize(-1f * Position + point);
			StrafeDirection = glm.cross(ViewDirection, IdentY);
		}
	}
}
