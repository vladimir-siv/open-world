using GlmNet;

using XEngine.Core;
using XEngine.Lighting;
using XEngine.Shading;
using XEngine.Shapes;

namespace open_world
{
	[GenerateScene("OpenWorld.TestScene", isMain: true)]
	public class TestScene : Scene
	{
		private GameObject Model;
		private GameObject User;
		private GameObject Light;
		private GameObject Ground;

		private Color ModelColor = new Color(232.0f / 255.0f, 176.0f / 255.0f, 141.0f / 255.0f);
		private AmbientLight AmbientLight = AmbientLight.Bright;
		private PointLight PointLight = new PointLight(-15.0f, 40.0f, 30.0f);

		protected override void Init()
		{
			Model = new GameObject("Model");
			Model.mesh = new Mesh();
			Model.mesh.LoadModel("male_head", VertexAttribute.POSITION | VertexAttribute.NORMAL).Wait();
			Model.mesh.material = new Material(Shader.Find("phong"));
			Model.mesh.material.Set("material_color", ModelColor.rgb);
			Model.mesh.material.Set("ambient_light_color", AmbientLight.color.rgb);
			Model.mesh.material.Set("ambient_light_power", AmbientLight.power);
			Model.mesh.material.Set("light_source_position", PointLight.position);
			Model.mesh.material.Set("light_source_color", PointLight.color.rgb);
			Model.mesh.material.Set("light_source_power", PointLight.power);
			
			User = new GameObject("User");
			User.mesh = new Mesh();
			User.mesh.shape = new Cube();
			User.mesh.material = new Material(Shader.Find("basic"));
			User.transform.position = new vec3(-30.0f, 20.0f, 30.0f);
			User.transform.rotation = new vec3(-25.0f, -45.0f, 0.0f);
			User.AttachBehavior(new UserController { Model = Model });

			Light = new GameObject("Light");
			Light.mesh = new Mesh();
			Light.mesh.shape = new Cube();
			Light.mesh.material = new Material(Shader.Find("basic"));
			Light.transform.position = PointLight.position;

			Ground = new GameObject("Ground");
			Ground.mesh = new Mesh();
			Ground.mesh.shape = new Plane();
			Ground.mesh.material = new Material(Shader.Find("unlit"));
			Ground.mesh.material.Set("material_color", new Color(1.0f, 1.0f, 1.0f));
			Ground.transform.position = new vec3(0.0f, -20.0f, 0.0f);
			Ground.transform.scale = new vec3(2.0f, 2.0f, 2.0f);

			MainCamera.Following = User;

			// [Uncomment for testing]
			//MainCamera.LocalPosition = new vec3(-4.0f, +4.0f, +10.0f);
			//Model.parent = User;
			//Model.transform.position = new vec3(+0.0f, -4.0f, -40.0f);
		}
	}
}
