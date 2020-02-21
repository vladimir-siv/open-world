using System;
using System.Collections.Generic;
using SharpGL;
using GlmNet;

namespace XEngine.Core
{
	using XEngine.Shading;
	using XEngine.Scripting;
	using XEngine.Common;

	public sealed class GameObject : IDisposable
	{
		public static GameObject FindByName(string name)
		{
			foreach (var gameObject in SceneManager.CurrentScene.GameObjects)
			{
				if (gameObject.name == name) return gameObject;
			}

			return null;
		}

		public string name;

		public GameObject parent = null;
		public Transform transform = new Transform(vector3.zero, vector3.zero, vector3.one);

		private Mesh _mesh = null;
		public Mesh mesh
		{
			get
			{
				return _mesh;
			}
			set
			{
				if (value == _mesh) return;
				_mesh?.Release();
				_mesh = value;
				_mesh?.Register();
			}
		}
		private Material _material = null;
		public Material material
		{
			get => _material;
			set => _material = value;
		}

		public bool IsDrawable => mesh?.shape != null && material?.shader != null;
		public bool DisableRendering { get; set; } = false;

		internal mat4 transform_model = mat4.identity();
		internal readonly float[] model = new float[16];
		internal mat4 rotate_model = mat4.identity();
		internal readonly float[] rotate = new float[16];

		internal mat4 transform_scale_invariant = mat4.identity();
		internal vec3 total_scale = vector3.one;

		private readonly LinkedList<XBehaviour> Scripts = new LinkedList<XBehaviour>();

		public GameObject(string name, params XBehaviour[] scripts)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentException("GameObject name cannot be null or empty.");
			this.name = name;
			foreach (var script in scripts) AttachBehaviour(script);
			SceneManager.CurrentScene.Add(this);
		}

		public void AttachBehaviour(XBehaviour script)
		{
			if (script.gameObject != null) throw new InvalidOperationException("Script instance is already attached.");
			script.gameObject = this;
			Scripts.AddLast(script);
		}
		public void DetachBehaviour(XBehaviour script)
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
			transform_scale_invariant = parent?.transform_scale_invariant.copy_to(transform_scale_invariant) ?? transform_scale_invariant.identify();
			rotate_model = parent?.rotate_model.copy_to(rotate_model) ?? rotate_model.identify();

			transform_scale_invariant = glm.translate(transform_scale_invariant, transform.position);
			transform_scale_invariant = quaternion.euler(transform_scale_invariant, transform.rotation);
			rotate_model = quaternion.euler(rotate_model, transform.rotation);

			total_scale = (parent?.total_scale ?? vector3.one) * transform.scale;
			transform_model = transform_scale_invariant.copy_to(transform_model);
			transform_model = glm.scale(transform_model, total_scale);

			transform_model.serialize(model);
			rotate_model.serialize(rotate);
		}
		public void Draw()
		{
			if (!IsDrawable) return;
			if (DisableRendering) return;
			var camera = SceneManager.CurrentScene.MainCamera;
			material.shader.Use();
			var gl = XEngineContext.Graphics;
			gl.BindVertexArray(mesh.VertexArrayId);
			if (material.shader.Project != -1) gl.UniformMatrix4(material.shader.Project, 1, false, camera.ViewToProjectData);
			if (material.shader.View != -1) gl.UniformMatrix4(material.shader.View, 1, false, camera.WorldToViewData);
			if (material.shader.Model != -1) gl.UniformMatrix4(material.shader.Model, 1, false, model);
			if (material.shader.Rotate != -1) gl.UniformMatrix4(material.shader.Rotate, 1, false, rotate);
			if (material.shader.Eye != -1) gl.Uniform3(material.shader.Eye, camera.Position.x, camera.Position.y, camera.Position.z);
			material.Prepare();
			gl.DrawElements(mesh.shape.OpenGLShapeType, mesh.shape.IndexCount, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero);
		}

		public void Dispose()
		{
			Destroy();
			parent = null;
			mesh = null;
		}
	}
}
