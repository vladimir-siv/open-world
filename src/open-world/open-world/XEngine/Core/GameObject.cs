using System;
using GlmNet;

namespace XEngine.Core
{
	public sealed class GameObject : IDisposable
	{
		public string name;

		public GameObject parent = null;
		public Transform transform = new Transform(new vec3(0.0f, 0.0f, 0.0f), new vec3(0.0f, 0.0f, 0.0f), new vec3(1.0f, 1.0f, 1.0f));
		public Mesh mesh = null;

		private Transform worldTransform;

		public GameObject(string name)
		{
			this.name = name;
			SceneManager.CurrentScene.GameObjects.AddLast(this);
		}

		public void SyncTransform()
		{
			if (parent == null) worldTransform = transform;
			else worldTransform = parent.transform + transform;
		}

		public void Draw()
		{
			if (mesh == null) return;
			mesh.Draw(worldTransform);
		}

		public void Dispose()
		{
			mesh?.Dispose();
			parent = null;
			mesh = null;
		}
	}
}
