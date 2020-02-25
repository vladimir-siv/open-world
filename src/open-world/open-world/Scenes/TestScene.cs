using GlmNet;
using XEngine.Core;
using XEngine.Lighting;
using XEngine.Shading;
using XEngine.Shapes;

namespace open_world
{
	[GenerateScene("OpenWorld.TestScene")]
	public class TestScene : Scene
	{
		protected override void Init()
		{
			Skybox = Skybox.Black;
			FogDensity = 0.0f;

			var light = LightSource.Point(new vec3(-15.0f, 40.0f, 30.0f));
			var model_color = new Color(232.0f / 255.0f, 176.0f / 255.0f, 141.0f / 255.0f);

			var cube = new Mesh { shape = new Cube() { Attributes = VertexAttribute.POSITION | VertexAttribute.COLOR }, };
			var basic = new Material(Shader.Find("basic"));

			var Model = new GameObject("Model");
			Model.mesh = new Mesh();
			Model.mesh.LoadModel("male_head", VertexAttribute.POSITION | VertexAttribute.NORMAL).Wait();
			Model.material = new Material(Shader.Find("phong"));
			Model.material.Set("dampening", 10.0f);
			Model.material.Set("reflectivity", 1.0f);
			Model.material.Set("material_color", model_color, true);

			var User = new GameObject("User");
			User.mesh = cube;
			User.material = basic;
			User.transform.position = new vec3(-30.0f, 20.0f, 30.0f);
			User.transform.rotation = new vec3(-25.0f, -45.0f, 0.0f);
			User.AttachBehaviour(new UserController { Model = Model });

			var Light = new GameObject("Light");
			Light.mesh = cube;
			Light.material = basic;
			Light.transform.position = light.position;

			var Ground = new GameObject("Ground");
			Ground.mesh = new Mesh();
			Ground.mesh.shape = new Plane() { Attributes = VertexAttribute.POSITION | VertexAttribute.UV };
			Ground.material = new Material(Shader.Find("unlit_texture"));
			Ground.material.Set("material_texture", Texture2D.FindBMP("Objects/crate"));
			Ground.transform.position = new vec3(0.0f, -20.0f, 0.0f);
			Ground.transform.scale = new vec3(2.0f, 2.0f, 2.0f);

			AddLight(light);
			MainCamera.Following = User;
		}
	}
}
