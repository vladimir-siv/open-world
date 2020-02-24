using System;

namespace XEngine.Core
{
	using XEngine.Shading;

	public class Prefab
	{
		public string name;

		public Mesh mesh;
		public Material material;

		public Prefab(string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentException("Prefab name cannot be null or empty.");
			this.name = name;
		}

		public GameObject Instantiate() => Instantiate(Transform.Origin);
		public GameObject Instantiate(Transform transform)
		{
			var instance = new GameObject(name);
			instance.mesh = mesh;
			instance.material = material;
			instance.transform = transform;
			return instance;
		}
	}
}
