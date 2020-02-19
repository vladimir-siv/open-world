using GlmNet;
using SharpGL;

using XEngine;
using XEngine.Core;
using XEngine.Data;
using XEngine.Shapes;
using XEngine.Shading;
using XEngine.Common;

namespace open_world.Scenes
{
	using Scripts;

	[GenerateScene("OpenWorld.TestScene", isMain: true)]
	public class TestScene : Scene
	{
		private GameObject Model;
		private GameObject User;

		protected override void Init()
		{
			var gl = XEngineContext.Graphics;
			gl.Enable(OpenGL.GL_DEPTH_TEST);
			gl.Enable(OpenGL.GL_CULL_FACE);
			gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

			Model = new GameObject("Model");
			Model.mesh = new Mesh();
			Model.mesh.LoadModel("male_head", VertexAttribute.POSITION | VertexAttribute.NORMAL).Wait();
			Model.mesh.material = new Material(Shader.Find("phong"));
			Model.mesh.material.Set("material_color", new vec3(232 / 255f, 176 / 255f, 141 / 255f));
			Model.mesh.material.Set("ambient_light_color", new vec3(1.0f, 1.0f, 1.0f));
			Model.mesh.material.Set("ambient_light_power", 0.25f);
			Model.mesh.material.Set("light_source_position", new vec3(-15.0f, 40.0f, 30.0f));
			Model.mesh.material.Set("light_source_color", new vec3(1.0f, 1.0f, 1.0f));
			Model.mesh.material.Set("light_source_power", 60.0f);
			
			User = new GameObject("UserController");
			User.mesh = new Mesh();
			User.mesh.shape = new Cube();
			User.mesh.shape.Attributes = VertexAttribute.POSITION | VertexAttribute.COLOR;
			User.mesh.shape.Release(); // [optimization]
			User.mesh.material = new Material(Shader.Find("unlit"));

			User.AttachBehavior(new UserController { Model = Model });
			User.transform.position = new vec3(-30.0f, 20.0f, 30.0f);
			User.transform.rotation = new vec3(-25.0f, -45.0f, 0.0f);

			MainCamera.Follow(User, vector3.zero, vector3.zero);
			//MainCamera.LocalPosition = new vec3(-4.0f, 4.0f, 10.0f);
		}

		protected override void Draw()
		{
			var gl = XEngineContext.Graphics;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);
			gl.Viewport(0, 0, XEngineContext.GLControl.Width, XEngineContext.GLControl.Height);
			
			Model.Sync();
			User.Sync();

			MainCamera.Adjust();

			Model.Draw();
			User.Draw();
		}
	}
}
