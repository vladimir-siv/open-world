using System;
using System.Globalization;
using System.IO;

using GlmNet;

using XEngine;
using XEngine.Core;

namespace open_world
{
	public sealed class Map : IDisposable
	{
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

	public static class MapGenerator
	{
		public static void Generate
		(
			string mapName,
			vec3 pos_from, vec3 pos_to,
			vec3 rot_from, vec3 rot_to,
			vec3 scl_from, vec3 scl_to,
			int object_count,
			params string[] object_names
		)
		{
			if (mapName.Contains("/") || mapName.Contains("\\")) throw new FormatException("File name cannot have subdirectories.");

			var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			var path = Path.Combine(desktop, mapName + ".map");

			var rnd = new Random();

			float RndFloat(float from, float to) => from + (to - from) * (float)rnd.NextDouble();

			using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
			{
				using (var writer = new StreamWriter(stream))
				{
					for (var i = 0; i < object_count; ++i)
					{
						var name = object_names[rnd.Next(object_names.Length)];

						var yd = 0.0f;
						if (name == "crate") yd += 0.5f;

						var pos = new vec3
						(
							RndFloat(pos_from.x, pos_to.x),
							RndFloat(pos_from.y + yd, pos_to.y + yd),
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

						writer.WriteLine($"{name}:{pos.x},{pos.y},{pos.z}:{rot.x},{rot.y},{rot.z}:{scl.x},{scl.y},{scl.z}");
					}
				}
			}
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
