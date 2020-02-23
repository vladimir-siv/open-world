using GlmNet;
using XEngine.Core;
using XEngine.Lighting;
using XEngine.Resources;
using XEngine.Terrains;
using XEngine.Shading;
using XEngine.Shapes;

namespace open_world
{
	[GenerateScene("OpenWorld.MainScene", isMain: true)]
	public class MainScene : Scene
	{
		protected override void Init()
		{
			var AmbientLight = new AmbientLight(Color.White, 0.25f);
			var PointLight = new PointLight(-15.0f, 40.0f, 30.0f);

			var PlayerColor = new Color(232.0f / 255.0f, 176.0f / 255.0f, 141.0f / 255.0f);
			
			var cube = new Cube { KeepAlive = true };
			var terrain = Terrain.GenerateFlat(50.0f, 10u, 5u);

			var Player = new GameObject("Player");
			Player.mesh = new Mesh();
			Player.mesh.shape = cube.Use(VertexAttribute.POSITION | VertexAttribute.NORMAL);
			Player.material = new Material(Shader.Find("phong"));
			Player.material.Set("ambient_light_color", AmbientLight.color, true);
			Player.material.Set("ambient_light_power", AmbientLight.power);
			Player.material.Set("light_source_position", PointLight.position);
			Player.material.Set("light_source_color", PointLight.color, true);
			Player.material.Set("light_source_power", PointLight.power);
			Player.material.Set("dampening", 10.0f);
			Player.material.Set("reflectivity", 1.0f);
			Player.material.Set("material_color", PlayerColor, true);
			Player.AttachBehaviour(new PlayerController {  });
			Player.transform.position = new vec3(+0.0f, +5.0f, +0.0f);

			var Crate = new GameObject("Crate");
			Crate.mesh = new Mesh();
			Crate.mesh.shape = cube.Use(VertexAttribute.POSITION | VertexAttribute.UV);
			Crate.material = new Material(Shader.Find("unlit_texture"));
			Crate.material.texture = Resource.LoadTexture("crate");
			Crate.transform.position = new vec3(+0.0f, +0.5f, +0.0f);

			var Light = new GameObject("Light");
			Light.mesh = new Mesh();
			Light.mesh.shape = cube.Use(VertexAttribute.POSITION | VertexAttribute.COLOR);
			Light.material = new Material(Shader.Find("basic"));
			Light.transform.position = PointLight.position;

			var Ground = new GameObject("Ground");
			Ground.mesh = new Mesh();
			Ground.mesh.shape = terrain.Shape.Use(VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV);
			Ground.material = new Material(Shader.Find("phong_texture"));
			Ground.material.Set("ambient_light_color", AmbientLight.color, true);
			Ground.material.Set("ambient_light_power", AmbientLight.power);
			Ground.material.Set("light_source_position", PointLight.position);
			Ground.material.Set("light_source_color", PointLight.color, true);
			Ground.material.Set("light_source_power", PointLight.power);
			Ground.material.Set("dampening", 10.0f);
			Ground.material.Set("reflectivity", 0.1f);
			Ground.material.Set("use_simulated_light", false);
			Ground.material.texture = Resource.LoadPNGTexture("grass_ground");

			var Grass = new GameObject("Grass");
			Grass.mesh = new Mesh();
			Grass.mesh.LoadModel("grass", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV);
			Grass.material = new Material(Shader.Find("phong_texture"));
			Grass.material.Set("ambient_light_color", AmbientLight.color, true);
			Grass.material.Set("ambient_light_power", AmbientLight.power);
			Grass.material.Set("light_source_position", PointLight.position);
			Grass.material.Set("light_source_color", PointLight.color, true);
			Grass.material.Set("light_source_power", PointLight.power);
			Grass.material.Set("dampening", 10.0f);
			Grass.material.Set("reflectivity", 1.0f);
			Grass.material.Set("use_simulated_light", true);
			Grass.material.texture = Resource.LoadPNGTexture("grass");
			Grass.material.CullFace = false;
			Grass.transform.position = new vec3(-1.0f, +0.0f, +0.0f);

			var Fern = new GameObject("Fern");
			Fern.mesh = new Mesh();
			Fern.mesh.LoadModel("fern", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV);
			Fern.material = new Material(Shader.Find("phong_texture"));
			Fern.material.Set("ambient_light_color", AmbientLight.color, true);
			Fern.material.Set("ambient_light_power", AmbientLight.power);
			Fern.material.Set("light_source_position", PointLight.position);
			Fern.material.Set("light_source_color", PointLight.color, true);
			Fern.material.Set("light_source_power", PointLight.power);
			Fern.material.Set("dampening", 10.0f);
			Fern.material.Set("reflectivity", 1.0f);
			Fern.material.Set("use_simulated_light", true);
			Fern.material.texture = Resource.LoadPNGTexture("fern");
			Fern.material.CullFace = false;
			Fern.transform.position = new vec3(+5.0f, +0.0f, +2.5f);

			cube.Dispose(force: true);

			MainCamera.Following = Player;
			MainCamera.LocalPosition = new vec3(+0.0f, +2.5f, +5.0f);
			MainCamera.LocalRotation = new vec3(-20.0f, +0.0f, +0.0f);
		}
	}
}
