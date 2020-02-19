using System;
using System.Collections.Generic;
using GlmNet;

namespace XEngine.Core
{
	using XEngine.Scripting;
	using XEngine.Common;

	public sealed class GameObject : IDisposable
	{
		public string name;

		public GameObject parent = null;
		public Transform transform = new Transform(vector3.zero, vector3.zero, vector3.one);
		public Mesh mesh = null;

		private Transform world_transform = new Transform(vector3.zero, vector3.zero, vector3.one);
		private readonly float[] model = new float[16];
		private readonly float[] rotate = new float[16];

		private readonly LinkedList<XBehaviour> Scripts = new LinkedList<XBehaviour>();
		
		public GameObject(string name, params XBehaviour[] scripts)
		{
			this.name = name;
			foreach (var script in scripts) AttachBehavior(script);
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
			world_transform.rotation = transform.rotation;
			world_transform.scale = transform.scale;

			var _model = mat4.identity();

			if (parent != null)
			{
				_model = glm.translate(_model, parent.world_transform.position);
				_model = glm.scale(_model, parent.world_transform.scale);
				_model = quaternion.euler(_model, parent.world_transform.rotation);

				world_transform.rotation += parent.world_transform.rotation;
				world_transform.scale *= parent.world_transform.scale;
			}

			_model = glm.translate(_model, transform.position);
			_model = glm.scale(_model, transform.scale);
			_model = quaternion.euler(_model, transform.rotation);

			world_transform.position = (_model * vector4.neutral).to_vec3();
			
			_model.serialize(model);
			quaternion.euler(world_transform.rotation).serialize(rotate);
		}
		public void Draw()
		{
			if (mesh == null) return;
			mesh.Draw(model, rotate);
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
