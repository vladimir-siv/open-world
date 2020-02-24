﻿using GlmNet;
using XEngine;
using XEngine.Core;
using XEngine.Lighting;
using XEngine.Resources;
using XEngine.Terrains;
using XEngine.Shading;
using XEngine.Shapes;
using XEngine.Common;

namespace open_world
{
	[GenerateScene("OpenWorld.MainScene", isMain: true)]
	public class MainScene : Scene
	{
		protected override void Init()
		{
			const bool GEN_NEW_TERRAIN = false;

			Skybox = new Skybox();

			var AmbientLight = new AmbientLight(Color.White, 0.25f);
			var PointLight = new PointLight(-15.0f, 40.0f, 30.0f);

			var cube = new Cube { KeepAlive = true };
			var terrain = (Terrain)null;
			using (var heightmap = ManifestResourceManager.LoadAsBitmap("heightmap.png")) terrain = Terrain.Generate(250.0f, 25u, heightmap);
			
			var Crate = new Prefab("Crate");
			Crate.mesh = new Mesh();
			Crate.mesh.shape = cube.Use(VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV);
			Crate.material = new Material(Shader.Find("phong_texture"));
			Crate.material.Set("material_texture", Resource.LoadTexture("crate"));

			var Grass = new Prefab("Grass");
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
			Grass.material.Set("material_texture", Resource.LoadPNGTexture("grass"));
			Grass.material.CullFace = false;

			var Fern = new Prefab("Fern");
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
			Fern.material.Set("material_texture", Resource.LoadPNGTexture("fern"));
			Fern.material.CullFace = false;

			var Player = new GameObject("Player");
			Player.AttachBehaviour(new PlayerController {  });
			Player.transform.position = new vec3(+0.0f, +5.0f, +0.0f);

			var Light = new GameObject("Light");
			Light.mesh = new Mesh();
			Light.mesh.shape = cube.Use(VertexAttribute.POSITION | VertexAttribute.COLOR);
			Light.material = new Material(Shader.Find("basic"));
			Light.transform.position = PointLight.position;

			var Ground = new GameObject("Ground");
			Ground.mesh = new Mesh();
			Ground.mesh.shape = terrain.Shape.Use(VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV);
			Ground.material = new Material(Shader.Find("phong_terrain"));
			Ground.material.Set("ambient_light_color", AmbientLight.color, true);
			Ground.material.Set("ambient_light_power", AmbientLight.power);
			Ground.material.Set("light_source_position", PointLight.position);
			Ground.material.Set("light_source_color", PointLight.color, true);
			Ground.material.Set("light_source_power", PointLight.power);
			Ground.material.Set("dampening", 10.0f);
			Ground.material.Set("reflectivity", 0.1f);
			Ground.material.Set("tiles", terrain.Tiles);
			Ground.material.Set("main_texture", Resource.LoadPNGTexture("ground_mud"));
			Ground.material.Set("r_texture", Resource.LoadPNGTexture("ground_flowery"));
			Ground.material.Set("g_texture", Resource.LoadPNGTexture("ground_grass"));
			Ground.material.Set("b_texture", Resource.LoadPNGTexture("ground_path"));
			Ground.material.Set("terrain_map", Resource.LoadPNGTexture("terrain"));

			if (GEN_NEW_TERRAIN)
			{
				Map.Generate
				(
					"field",
					terrain,
					Ground.transform.position,
					new vec3(-terrain.Length * 0.45f, 0.0f, -terrain.Length * 0.45f),
					new vec3(+terrain.Length * 0.45f, 0.0f, +terrain.Length * 0.45f),
					new vec3(0.0f, 0.0f, 0.0f),
					new vec3(0.0f, 360.0f, 0.0f),
					vector3.one,
					vector3.one,
					250,
					"crate", "grass", "fern"
				);
			}

			using (var map = new Map("field"))
			{
				while (map.Read(out var descriptor))
				{
					switch (descriptor.name)
					{
						case "crate": Crate.Instantiate(descriptor.transform); break;
						case "grass": Grass.Instantiate(descriptor.transform); break;
						case "fern": Fern.Instantiate(descriptor.transform); break;
						default: break;
					}
				}
			}

			cube.Dispose(force: true);

			MainCamera.Following = Player;
		}
	}
}
