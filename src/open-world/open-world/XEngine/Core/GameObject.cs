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
		private readonly float[] translate = new float[16];
		private readonly float[] rotate = new float[16];
		private readonly float[] scale = new float[16];

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
			var position_vec = transform.position;
			var rotate_vec = transform.rotation;
			var scale_vec = transform.scale;
			
			if (parent != null)
			{
				rotate_vec += parent.world_transform.rotation;
				scale_vec *= parent.world_transform.scale;
			}

			var translate = glm.translate(mat4.identity(), parent != null ? parent.world_transform.position : position_vec);
			var rotate = quaternion.euler(mat4.identity(), rotate_vec);
			var scale = glm.scale(mat4.identity(), scale_vec);

			if (parent != null)
			{
				translate = quaternion.euler(translate, parent.world_transform.rotation);
				translate = glm.translate(translate, transform.position);
				translate = quaternion.euler(translate, transform.rotation);
				translate = glm.translate(mat4.identity(), position_vec = (translate * vector4.neutral).to_vec3());
			}

			world_transform = new Transform(position_vec, rotate_vec, scale_vec);
			translate.serialize(this.translate);
			rotate.serialize(this.rotate);
			scale.serialize(this.scale);
		}
		public void Draw()
		{
			if (mesh == null) return;
			mesh.Draw(translate, rotate, scale);
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
