using System;
using System.Collections.Generic;
using GlmNet;

namespace XEngine.Core
{
	using XEngine.Scripting;

	public sealed class GameObject : IDisposable
	{
		public string name;

		public GameObject parent = null;
		public Transform transform = new Transform(new vec3(0.0f, 0.0f, 0.0f), new vec3(0.0f, 0.0f, 0.0f), new vec3(1.0f, 1.0f, 1.0f));
		public Mesh mesh = null;

		private Transform worldTransform = new Transform(new vec3(0.0f, 0.0f, 0.0f), new vec3(0.0f, 0.0f, 0.0f), new vec3(1.0f, 1.0f, 1.0f));
		private LinkedList<XBehaviour> Scripts = new LinkedList<XBehaviour>();

		public GameObject(string name, params XBehaviour[] scripts)
		{
			this.name = name;
			foreach (var script in Scripts) AttachBehavior(script);
			SceneManager.CurrentScene.Add(this);
		}

		public void AttachBehavior(XBehaviour script)
		{
			if (script.gameObject != null) throw new InvalidOperationException("Script instance is already attached.");
			script.gameObject = this;
			Scripts.AddLast(script);
		}
		public void DetachBehavior(XBehaviour script)
		{
			if (script.gameObject != this) throw new InvalidOperationException("Script instance not attached to this game object.");
			Scripts.Remove(script);
			script.gameObject = null;
		}

		internal void Awake() { foreach (var script in Scripts) script._Awake(); }
		internal void Start() { foreach (var script in Scripts) script._Start(); }
		internal void Update() { foreach (var script in Scripts) script._Update(); }
		internal void Late() { foreach (var script in Scripts) script._Late(); }
		internal void Destroy() { foreach (var script in Scripts) script._Destroy(); }

		public void Sync()
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
			Destroy();
			mesh?.Dispose();
			parent = null;
			mesh = null;
		}
	}
}
