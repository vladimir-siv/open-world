using SharpGL;
using GlmNet;

namespace XEngine.Lighting
{
	using XEngine.Core;
	using XEngine.Shading;
	using XEngine.Common;

	public class Skybox
	{
		private class SkyboxShape : GeometricShape
		{
			public static GeometricShape Geometry { get; } = new SkyboxShape();

			private SkyboxShape() : base
			(
				new ShapeData
				(
					ShapeData.Positions3f
					(
						-0.5f, +0.5f, -0.5f,
						-0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,
						+0.5f, +0.5f, -0.5f,
						-0.5f, +0.5f, -0.5f,

						-0.5f, -0.5f, +0.5f,
						-0.5f, -0.5f, -0.5f,
						-0.5f, +0.5f, -0.5f,
						-0.5f, +0.5f, -0.5f,
						-0.5f, +0.5f, +0.5f,
						-0.5f, -0.5f, +0.5f,

						+0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,

						-0.5f, -0.5f, +0.5f,
						-0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, -0.5f, +0.5f,
						-0.5f, -0.5f, +0.5f,

						-0.5f, +0.5f, -0.5f,
						+0.5f, +0.5f, -0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						-0.5f, +0.5f, +0.5f,
						-0.5f, +0.5f, -0.5f,

						-0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f, +0.5f,
						+0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f, +0.5f,
						+0.5f, -0.5f, +0.5f
					)
				)
			)
			{
				Attributes = VertexAttribute.POSITION;
				KeepAlive = true;
			}
		}

		private static Mesh _mesh = null;
		internal static Mesh mesh
		{
			get
			{
				return _mesh;
			}
			set
			{
				if (value == _mesh) return;
				_mesh?.Dispose();
				_mesh = value;
			}
		}

		public static Skybox Black { get; } = new Skybox(Color.Black);
		public static Skybox Default { get; } = new Skybox(Color.DeepSky);

		public static Skybox Find(string name) => Find(name, Color.Gray);
		public static Skybox Find(string name, Color skycolor)
		{
			var skybox = new Skybox(skycolor);
			skybox.texture = CubeMap.Find($"Skyboxes/{name}");
			mesh = mesh ?? new Mesh() { shape = SkyboxShape.Geometry };
			return skybox;
		}

		public Color SkyColor { get; }
		public float RotationSpeed { get; set; } = 1.0f;

		private Texture texture = null;

		private float rotation = 0.0f;
		private mat4 transform = mat4.identity();
		private float[] transform_cache = new float[16];

		private Skybox(Color skycolor) { SkyColor = skycolor; }

		public void Draw(float scale)
		{
			if (texture == null) return;

			rotation += RotationSpeed * Time.DeltaTime / 1000.0f;

			var gl = XEngineContext.Graphics;
			var shader = XEngineContext.SkyboxShader;

			var camera = SceneManager.CurrentScene.MainCamera;
			transform = camera.WorldToView.copy_to(transform);
			transform = quaternion.euler(transform, 0.0f, rotation, 0.0f);
			transform_cache = transform.serialize(transform_cache);
			transform_cache[12] = transform_cache[13] = transform_cache[14] = 0.0f;

			shader.Use();
			texture.Activate();
			gl.UniformMatrix4(shader.Project, 1, false, camera.ViewToProjectData);
			gl.UniformMatrix4(shader.View, 1, false, transform_cache);
			shader.SetScalar("scale", scale);
			shader.SetVec4("sky_color", SkyColor.r, SkyColor.g, SkyColor.b, SkyColor.a);

			mesh.Activate();
			gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, SkyboxShape.Geometry.VertexCount);
		}
	}
}
