using System;
using System.Globalization;
using System.IO;

using GlmNet;

using XEngine;
using XEngine.Core;
using XEngine.Terrains;
using XEngine.Common;

namespace open_world
{
	public sealed class Map : IDisposable
	{
		public static void Generate
		(
			string mapName,
			Terrain terrain,
			vec3 terrain_position,
			vec3 pos_from, vec3 pos_to,
			vec3 rot_from, vec3 rot_to,
			vec3 scl_from, vec3 scl_to,
			int object_count,
			params string[] object_names
		)
		{
			if (mapName.Contains("/") || mapName.Contains("\\")) throw new FormatException("File name cannot have subdirectories.");
			if (terrain == null) throw new ArgumentNullException(nameof(terrain));
			if (object_names == null) throw new ArgumentNullException(nameof(object_names));
			if (object_names.Length == 0) throw new ArgumentException("There must be at least one object name.");

			var path = Path.Combine(Environment.CurrentDirectory, $"..\\..\\Resources\\Maps\\{mapName}.generated.map");

			float RndFloat(float from, float to) => from + (to - from) * (float)RNG.Double();

			using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
			{
				using (var writer = new StreamWriter(stream))
				{
					for (var i = 0; i < object_count; ++i)
					{
						var name = object_names[RNG.Int(object_names.Length)];

						while (true)
						{
							var pos = new vec3
							(
								RndFloat(pos_from.x, pos_to.x),
								RndFloat(pos_from.y, pos_to.y),
								RndFloat(pos_from.z, pos_to.z)
							);
							var rot = new vec3
							(
								RndFloat(rot_from.x, rot_to.x),
								RndFloat(rot_from.y, rot_to.y),
								RndFloat(rot_from.z, rot_to.z)
							);
							var scl = new vec3
							(
								RndFloat(scl_from.x, scl_to.x),
								RndFloat(scl_from.y, scl_to.y),
								RndFloat(scl_from.z, scl_to.z)
							);

							var height = terrain.CalculateLocalHeight(pos.x - terrain_position.x, pos.z - terrain_position.z);
							if (height < 0.1) continue;
							pos.y += height;
							writer.WriteLine($"{name}:{pos.x},{pos.y},{pos.z}:{rot.x},{rot.y},{rot.z}:{scl.x},{scl.y},{scl.z}");
							break;
						}
					}
				}
			}
		}

		private Stream CurrentMap = null;
		private StreamReader Reader = null;

		public Map() { }
		public Map(string name) => LoadMap(name);

		public void LoadMap(string name)
		{
			if (CurrentMap != null) Dispose();
			CurrentMap = ManifestResourceManager.LoadFromResources($"{name}.map");
			Reader = new StreamReader(CurrentMap);
		}

		public bool Read(out ObjectDescriptor descriptor)
		{
			if (CurrentMap == null) throw new InvalidOperationException("Please, load a map.");

			if (Reader.EndOfStream)
			{
				descriptor = ObjectDescriptor.Empty;
				return false;
			}

			var line = Reader.ReadLine().Split(':');

			if (line.Length != 4) throw new FormatException("Invalid file format.");

			vec3 parse_vec3(string str)
			{
				var vals = str.Split(',');
				if (vals.Length != 3) throw new FormatException("Invalid file format.");
				return new vec3
				(
					float.Parse(vals[0], CultureInfo.InvariantCulture),
					float.Parse(vals[1], CultureInfo.InvariantCulture),
					float.Parse(vals[2], CultureInfo.InvariantCulture)
				);
			}

			var position = parse_vec3(line[1]);
			var rotation = parse_vec3(line[2]);
			var scale = parse_vec3(line[3]);

			descriptor = new ObjectDescriptor(line[0], position, rotation, scale);

			return true;
		}

		public void Dispose()
		{
			Reader.Dispose();
			Reader = null;
			CurrentMap.Dispose();
			CurrentMap = null;
		}
	}

	public struct ObjectDescriptor
	{
		public static ObjectDescriptor Empty = new ObjectDescriptor(string.Empty, Transform.Origin);

		public string name;
		public Transform transform;

		public ObjectDescriptor(string name, vec3 position, vec3 rotation, vec3 scale) : this(name, new Transform(position, rotation, scale)) { }
		public ObjectDescriptor(string name, Transform transform)
		{
			this.name = name;
			this.transform = transform;
		}
	}
}
