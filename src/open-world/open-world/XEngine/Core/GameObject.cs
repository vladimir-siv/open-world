﻿using System;
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

		internal mat4 transform_model = mat4.identity();
		internal readonly float[] model = new float[16];
		internal mat4 rotate_model = mat4.identity();
		internal readonly float[] rotate = new float[16];

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
			transform_model = parent?.transform_model.clone() ?? mat4.identity();
			rotate_model = parent?.rotate_model.clone() ?? mat4.identity();

			transform_model = glm.translate(transform_model, transform.position);
			transform_model = glm.scale(transform_model, transform.scale);
			transform_model = quaternion.euler(transform_model, transform.rotation);
			rotate_model = quaternion.euler(rotate_model, transform.rotation);

			transform_model.serialize(model);
			rotate_model.serialize(rotate);
		}
		public void Draw()
		{
			if (mesh == null) return;
			mesh.Draw(this);
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
