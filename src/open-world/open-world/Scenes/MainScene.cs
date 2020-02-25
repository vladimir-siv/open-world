using System;
using GlmNet;
using XEngine;
using XEngine.Core;
using XEngine.Resources;
using XEngine.Terrains;
using XEngine.Lighting;
using XEngine.Shading;
using XEngine.Shapes;

namespace open_world
{
	[GenerateScene("OpenWorld.MainScene", isMain: true)]
	public class MainScene : Scene
	{
		protected override void Init()
		{
			Skybox = Skybox.Find("Cloudy", Color.FromBytes(145, 180, 194));

			var terrain = (Terrain)null;
			using (var heightmap = ManifestResourceManager.LoadAsBitmap("Textures/Ground/heightmap.png")) terrain = Terrain.Generate(500.0f, 50u, heightmap, 20.0f);
			
			var Crate = new Prefab("Crate");
			Crate.mesh = new Mesh();
			Crate.mesh.shape = new Cube() { Attributes = VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV };
			Crate.material = new Material(Shader.Find("phong_texture"));
			Crate.material.Set("dampening", 10.0f);
			Crate.material.Set("reflectivity", 0.1f);
			Crate.material.Set("use_simulated_light", false);
			Crate.material.Set("material_texture", Resource.LoadTexture("Objects/crate"));

			var Pine = new Prefab("Pine");
			Pine.mesh = new Mesh();
			Pine.mesh.LoadModel("pine", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV);
			Pine.material = new Material(Shader.Find("phong_texture"));
			Pine.material.Set("dampening", 10.0f);
			Pine.material.Set("reflectivity", 0.1f);
			Pine.material.Set("use_simulated_light", false);
			Pine.material.Set("material_texture", Resource.LoadPNGTexture("Plants/pine"), 0u, 9u);
			Pine.material.CullFace = false;

			var Fern = new Prefab("Fern");
			Fern.mesh = new Mesh();
			Fern.mesh.LoadModel("fern", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV);
			Fern.material = new Material(Shader.Find("phong_texture"));
			Fern.material.Set("dampening", 10.0f);
			Fern.material.Set("reflectivity", 0.1f);
			Fern.material.Set("use_simulated_light", false);
			Fern.material.Set("material_texture", Resource.LoadPNGTexture("Plants/atlas_fern"), 0u, 4u);
			Fern.material.CullFace = false;

			var Player = new GameObject("Player");
			Player.AttachBehaviour(new PlayerController {  });
			Player.transform.position = new vec3(+0.0f, +5.0f, +0.0f);

			var Ground = new GameObject("Ground");
			Ground.mesh = new Mesh();
			Ground.mesh.shape = terrain.Shape.Use(VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV);
			Ground.material = new Material(Shader.Find("phong_terrain"));
			Ground.material.Set("dampening", 10.0f);
			Ground.material.Set("reflectivity", 0.1f);
			Ground.material.Set("tiles", terrain.Tiles);
			Ground.material.Set("main_texture", Resource.LoadPNGTexture("Ground/ground_mud"));
			Ground.material.Set("r_texture", Resource.LoadPNGTexture("Ground/ground_flowery"));
			Ground.material.Set("g_texture", Resource.LoadPNGTexture("Ground/ground_grass"));
			Ground.material.Set("b_texture", Resource.LoadPNGTexture("Ground/ground_path"));
			Ground.material.Set("terrain_map", Resource.LoadPNGTexture("Ground/terrain"));

			using (var map = new Map("Maps/field"))
			{
				while (map.Read(out var descriptor))
				{
					var atlas_index = Convert.ToUInt32(descriptor.name[descriptor.name.Length - 1] - '0');
					var obj = (GameObject)null;

					switch (descriptor.name.Remove(descriptor.name.Length - 1))
					{
						case "crate": obj = Crate.Instantiate(descriptor.transform); break;
						case "pine": obj = Pine.Instantiate(descriptor.transform); break;
						case "fern": obj = Fern.Instantiate(descriptor.transform); break;
						default: break;
					}

					obj.properties = new ShaderProperties();
					obj.properties.SetAtlasIndex("material_texture", atlas_index);
				}
			}

			MainCamera.Following = Player;

			/*
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
				500,
				"crate0",
				"pine0",
				"fern0", "fern1", "fern2", "fern3"
			);
			//*/
		}
	}
}
