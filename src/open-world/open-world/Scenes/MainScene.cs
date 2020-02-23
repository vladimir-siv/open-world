﻿using GlmNet;
using XEngine.Core;
using XEngine.Lighting;
using XEngine.Resources;
using XEngine.Shading;
using XEngine.Shapes;

namespace open_world
{
	[GenerateScene("OpenWorld.MainScene", isMain: true)]
	public class MainScene : Scene
	{
		private GameObject Player;
		private GameObject Crate;
		private GameObject Light;
		private GameObject Ground;
		
		private AmbientLight AmbientLight = AmbientLight.Bright;
		private PointLight PointLight = new PointLight(-15.0f, 40.0f, 30.0f);

		private Color PlayerColor = new Color(232.0f / 255.0f, 176.0f / 255.0f, 141.0f / 255.0f);
		private Color GroundColor = new Color(0.39f, 0.95f, 0.43f);
		
		protected override void Init()
		{
			var cube = new Cube { KeepAlive = true };

			Player = new GameObject("Player");
			Player.mesh = new Mesh();
			Player.mesh.shape = cube.Use(VertexAttribute.POSITION | VertexAttribute.NORMAL);
			Player.material = new Material(Shader.Find("phong"));
			Player.material.Set("ambient_light_color", AmbientLight.color, true);
			Player.material.Set("ambient_light_power", AmbientLight.power);
			Player.material.Set("light_source_position", PointLight.position);
			Player.material.Set("light_source_color", PointLight.color, true);
			Player.material.Set("light_source_power", PointLight.power);
			Player.material.Set("material_color", PlayerColor, true);
			Player.AttachBehaviour(new PlayerController {  });
			Player.transform.position = new vec3(+0.0f, +5.0f, +0.0f);

			Crate = new GameObject("Crate");
			Crate.mesh = new Mesh();
			Crate.mesh.shape = cube.Use(VertexAttribute.POSITION | VertexAttribute.UV);
			Crate.material = new Material(Shader.Find("unlit_texture"));
			Crate.material.texture = Resource.LoadTexture("crate");
			Crate.transform.position = new vec3(+0.0f, +0.5f, +0.0f);

			Light = new GameObject("Light");
			Light.mesh = new Mesh();
			Light.mesh.shape = cube.Use(VertexAttribute.POSITION | VertexAttribute.COLOR);
			Light.material = new Material(Shader.Find("basic"));
			Light.transform.position = PointLight.position;

			Ground = new GameObject("Ground");
			Ground.mesh = new Mesh();
			Ground.mesh.shape = new Plane() { Attributes = VertexAttribute.POSITION | VertexAttribute.NORMAL };
			Ground.material = new Material(Shader.Find("phong"));
			Ground.material.Set("ambient_light_color", AmbientLight.color, true);
			Ground.material.Set("ambient_light_power", AmbientLight.power);
			Ground.material.Set("light_source_position", PointLight.position);
			Ground.material.Set("light_source_color", PointLight.color, true);
			Ground.material.Set("light_source_power", PointLight.power);
			Ground.material.Set("material_color", GroundColor, true);
			Ground.transform.scale = new vec3(5.0f, 5.0f, 5.0f);

			cube.Dispose(force: true);

			MainCamera.Following = Player;
			MainCamera.LocalPosition = new vec3(+0.0f, +2.5f, +5.0f);
			MainCamera.LocalRotation = new vec3(-20.0f, +0.0f, +0.0f);
		}
	}
}
