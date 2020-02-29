using System;
using GlmNet;
using XEngine.Core;
using XEngine.Terrains;
using XEngine.Lighting;
using XEngine.Rendering;
using XEngine.Shading;
using XEngine.Shapes;

namespace open_world
{
	[GenerateScene("OpenWorld.MainScene", isMain: true)]
	public class MainScene : Scene
	{
		private WaterFrameBuffers WFB = null;
		private GameObject Water = null;

		private void CreateWater()
		{
			WFB = new WaterFrameBuffers();

			var water_terrain = Terrain.GenerateFlat(1000.0f, 50u, 1000u);

			Water = GameObject.CreateUnlinked("Water");
			Water.mesh = new Mesh();
			Water.mesh.shape = water_terrain.Shape.Use(VertexAttribute.POSITION | VertexAttribute.UV);
			Water.material = new Material(Shader.Find("water"));
			Water.material.Set("water_color", Color.FromBytes(66, 135, 245));
			Water.material.Set("wave_strength", 0.02f);
			Water.material.Set("wave_speed", 0.03f);
			Water.material.Set("wave_timestamp", 0.0f);
			Water.material.Set("reflection", WFB.ReflectionFBO.TextureAttachment);
			Water.material.Set("refraction", WFB.RefractionFBO.TextureAttachment);
			Water.material.Set("dudv", Texture2D.FindPNG("Maps/water_dudv"));
			Water.material.MarkDynamic();
		}

		protected override void Init()
		{
			Add(LightSource.Sun);

			Sky.Cycle.Add(Skybox.Find("Daylight", Color.FromBytes(216, 229, 235)));
			//Sky.Cycle.Add(Skybox.Find("Cloudy", Color.FromBytes(145, 180, 194)));
			Sky.Cycle.Add(Skybox.Find("Night", Color.Black));

			//using (var heightmap = new TextureHeightMap("Ground/heightmap.png")) terrain = Terrain.Generate(500.0f, 50u, heightmap);
			var terrain = Terrain.Generate(500.0f, 50u, new ProceduralHeightMap(100u, 8357));

			var Crate = new Prefab("Crate");
			Crate.mesh = new Mesh();
			Crate.mesh.shape = new Cube() { Attributes = VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV };
			Crate.material = new Material(Shader.Find("phong_texture"));
			Crate.material.Set("dampening", 10.0f);
			Crate.material.Set("reflectivity", 0.1f);
			Crate.material.Set("use_simulated_light", false);
			Crate.material.Set("material_texture", Texture2D.FindBMP("Objects/crate"));

			var Tree = new Prefab("Tree");
			Tree.mesh = new Mesh();
			Tree.mesh.LoadModel("tree", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV).Wait();
			Tree.material = new Material(Shader.Find("phong_texture"));
			Tree.material.Set("dampening", 10.0f);
			Tree.material.Set("reflectivity", 0.1f);
			Tree.material.Set("use_simulated_light", false);
			Tree.material.Set("material_texture", Texture2D.FindPNG("Plants/tree"));
			Tree.material.CullFace = false;

			var Cherry = new Prefab("Cherry");
			Cherry.mesh = new Mesh();
			Cherry.mesh.LoadModel("cherry", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV).Wait();
			Cherry.material = new Material(Shader.Find("phong_texture"));
			Cherry.material.Set("dampening", 10.0f);
			Cherry.material.Set("reflectivity", 0.1f);
			Cherry.material.Set("use_simulated_light", false);
			Cherry.material.Set("material_texture", Texture2D.FindPNG("Plants/cherry"));
			Cherry.material.CullFace = false;

			var Fern = new Prefab("Fern");
			Fern.mesh = new Mesh();
			Fern.mesh.LoadModel("fern", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV).Wait();
			Fern.material = new Material(Shader.Find("phong_texture"));
			Fern.material.Set("dampening", 10.0f);
			Fern.material.Set("reflectivity", 0.1f);
			Fern.material.Set("use_simulated_light", false);
			Fern.material.Set("material_texture", Texture2D.FindPNG("Plants/atlas_fern"), 0u, 4u);
			Fern.material.CullFace = false;

			var Boulder = new Prefab("Boulder");
			Boulder.mesh = new Mesh();
			Boulder.mesh.LoadModel("boulder", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV).Wait();
			Boulder.material = new Material(Shader.Find("phong_texture"));
			Boulder.material.Set("dampening", 10.0f);
			Boulder.material.Set("reflectivity", 0.1f);
			Boulder.material.Set("use_simulated_light", false);
			Boulder.material.Set("material_texture", Texture2D.FindPNG("Objects/boulder"));

			var Barrel = new Prefab("Barrel");
			Barrel.mesh = new Mesh();
			Barrel.mesh.LoadModel("barrel", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV).Wait();
			Barrel.material = new Material(Shader.Find("phong_texture"));
			Barrel.material.Set("dampening", 10.0f);
			Barrel.material.Set("reflectivity", 0.1f);
			Barrel.material.Set("use_simulated_light", false);
			Barrel.material.Set("material_texture", Texture2D.FindPNG("Objects/barrel"));

			var Lantern = new Prefab("Lantern");
			Lantern.mesh = new Mesh();
			Lantern.mesh.LoadModel("lantern", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV).Wait();
			Lantern.material = new Material(Shader.Find("phong_texture_light_source"));
			Lantern.material.Set("dampening", 10.0f);
			Lantern.material.Set("reflectivity", 0.5f);
			Lantern.material.Set("use_simulated_light", false);
			Lantern.material.Set("material_texture", Texture2D.FindPNG("Objects/lantern"));
			Lantern.material.Set("brightness_map", Texture2D.FindPNG("Maps/lantern"));

			var Lamp = new Prefab("Lamp");
			Lamp.mesh = new Mesh();
			Lamp.mesh.LoadModel("lamp", VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV).Wait();
			Lamp.material = new Material(Shader.Find("phong_texture_light_source"));
			Lamp.material.Set("dampening", 10.0f);
			Lamp.material.Set("reflectivity", 0.5f);
			Lamp.material.Set("use_simulated_light", false);
			Lamp.material.Set("material_texture", Texture2D.FindPNG("Objects/lamp"));
			Lamp.material.Set("brightness_map", Texture2D.FindPNG("Objects/lamp"));

			var Lighting = new GameObject("Lighting");
			Lighting.AttachBehaviour(new LightingController { });

			var Player = new GameObject("Player");
			Player.AttachBehaviour(new PlayerController {  });
			Player.transform.position = new vec3(+0.0f, +10.0f, +0.0f);

			var Ground = new GameObject("Ground");
			Ground.mesh = new Mesh();
			Ground.mesh.shape = terrain.Shape.Use(VertexAttribute.POSITION | VertexAttribute.NORMAL | VertexAttribute.UV);
			Ground.material = new Material(Shader.Find("phong_terrain"));
			Ground.material.Set("dampening", 10.0f);
			Ground.material.Set("reflectivity", 0.1f);
			Ground.material.Set("tiles", terrain.Tiles);
			Ground.material.Set("main_texture", Texture2D.FindPNG("Ground/ground_mud"));
			Ground.material.Set("r_texture", Texture2D.FindPNG("Ground/ground_flowery"));
			Ground.material.Set("g_texture", Texture2D.FindPNG("Ground/ground_grass"));
			Ground.material.Set("b_texture", Texture2D.FindPNG("Ground/ground_path"));
			Ground.material.Set("terrain_map", Texture2D.FindPNG("Ground/terrain"));

			var leos = new[]
			{
				new vec3(+66.65484f, +20.54087f, +14.08892f),
				new vec3(-70.22906f, +18.10966f, +24.74698f),
				new vec3(-49.59372f, +24.86368f, -92.01048f),
				new vec3(+2.849623f, +18.29142f, +44.42444f),
				new vec3(-51.86799f, +23.97442f, +127.3762f),
			};

			var lanternDelta = new vec3(0.6468048f, 12.84496f, -5.352559f);
			var lampDelta = new vec3(0.0f, 11.55653f, 0.0f);

			for (var i = 0; i < leos.Length; ++i)
			{
				leos[i].y = terrain.CalculateLocalHeight(leos[i].x, leos[i].z);
				
				if (i % 2 == 0)
				{
					Lantern.Instantiate(leos[i]);
					Add(LightSource.Point(leos[i] + lanternDelta, Color.White, 10.0f));
				}
				else
				{
					Lamp.Instantiate(leos[i]);
					Add(LightSource.Point(leos[i] + lampDelta, Color.White, 10.0f));
				}
			}
			
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
				XEngine.Common.vector3.one,
				XEngine.Common.vector3.one,
				500,
				"crate0",
				"tree0",
				"cherry0",
				"fern0", "fern1", "fern2", "fern3",
				"boulder0",
				"barrel0"
			);
			//*/

			using (var map = new Map("Maps/field"))
			{
				while (map.Read(out var descriptor))
				{
					var atlas_index = Convert.ToUInt32(descriptor.name[descriptor.name.Length - 1] - '0');
					var transform = descriptor.transform;

					switch (descriptor.name.Remove(descriptor.name.Length - 1))
					{
						case "crate":
							transform.position.y += 1.5f;
							transform.scale = new vec3(3.0f, 3.0f, 3.0f);
							Crate.Instantiate(transform);
							break;
						case "tree":
							transform.scale = new vec3(+5.0f, +5.0f, +5.0f);
							Tree.Instantiate(transform);
							break;
						case "cherry":
							transform.scale = new vec3(+5.0f, +5.0f, +5.0f);
							Cherry.Instantiate(transform);
							break;
						case "fern":
							var obj = Fern.Instantiate(transform);
							obj.properties = new ShaderProperties();
							obj.properties.SetAtlasIndex("material_texture", atlas_index);
							break;
						case "boulder":
							Boulder.Instantiate(transform);
							break;
						case "barrel":
							transform.scale = new vec3(+10.0f, +10.0f, +10.0f);
							Barrel.Instantiate(transform);
							break;
						default: break;
					}
				}
			}

			CreateWater();

			MainCamera.Following = Player;
		}

		protected override void Draw()
		{
			Water.material.Set("wave_timestamp", ElapsedTime / 1000.0f);

			DrawCalls = 3u;
			Prepare();
			SyncScene();
			ClipDistance = true;

			ClipPlane = new vec4(0.0f, +1.0f, 0.0f, 0.0f);
			WFB.ReflectionFBO.Bind();
			MainCamera.Reflect();
			UpdateCamera();
			DrawScene();

			ClipPlane = new vec4(0.0f, -1.0f, 0.0f, 0.0f);
			WFB.RefractionFBO.Bind();
			MainCamera.ReflectBack();
			UpdateCamera();
			DrawScene();

			ClipDistance = false;
			FrameBuffer.BindRenderingWindow();
			Viewport();
			DrawScene();

			Water?.Sync();
			Water?.Draw();
		}

		protected override void Exit()
		{
			WFB?.Dispose();
			WFB = null;

			Water?.Dispose();
			Water = null;
		}
	}
}
