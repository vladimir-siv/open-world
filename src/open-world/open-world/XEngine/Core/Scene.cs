using System.Collections.Generic;
using SharpGL;

namespace XEngine.Core
{
	using XEngine.Structures;
	using XEngine.Shading;

	public abstract class Scene
	{
		private class Algorithms
		{
			public readonly Pouch<GameObject, GameObject> SyncObjectPouch = new Pouch<GameObject, GameObject>();
			public readonly Queue<GameObject> ObjectQueue = new Queue<GameObject>();
			public readonly Pouch<Shader, GameObject> DrawableObjectPouch = new Pouch<Shader, GameObject>();
			
			public IEnumerable<GameObject> SyncLevelOrder(GameObject gameObject)
			{
				ObjectQueue.Enqueue(gameObject);
				while (ObjectQueue.Count > 0)
				{
					var current = ObjectQueue.Dequeue();
					yield return current;
					while (SyncObjectPouch.Retrieve(current, out var child)) ObjectQueue.Enqueue(child);
				}
			}
		}

		internal static Dictionary<string, Scene> SceneCache = new Dictionary<string, Scene>();
		public static Scene Resolve(string sceneId) => SceneCache[sceneId];

		public string SceneId { get; internal set; } = string.Empty;
		public Camera MainCamera { get; protected set; } = new Camera();

		protected uint ClearStrategy { get; set; } = OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT;

		private bool Initialized = false;
		private readonly Algorithms Algs = new Algorithms();
		internal readonly LinkedList<GameObject> GameObjects = new LinkedList<GameObject>();
		
		internal void Add(GameObject gameObject)
		{
			GameObjects.AddLast(gameObject);
			
			if (Initialized)
			{
				gameObject.Awake();
				gameObject.Start();
			}
		}
		public void Clear()
		{
			foreach (var gameObject in GameObjects) gameObject.Dispose();
			GameObjects.Clear();
		}

		internal void _Init()
		{
			if (Initialized) return;
			var gl = XEngineContext.Graphics;
			gl.Enable(OpenGL.GL_DEPTH_TEST);
			gl.Enable(OpenGL.GL_CULL_FACE);
			gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
			Init();
			foreach (var gameObject in GameObjects) gameObject.Awake();
			foreach (var gameObject in GameObjects) gameObject.Start();
			Initialized = true;
		}
		internal void _Draw()
		{
			if (!Initialized) return;
			MainCamera.AspectRatio = (float)XEngineContext.GLControl.Width / (float)XEngineContext.GLControl.Height;
			foreach (var gameObject in GameObjects) gameObject.Update();
			foreach (var gameObject in GameObjects) gameObject.Late();
			var gl = XEngineContext.Graphics;
			if (ClearStrategy != 0u) gl.Clear(ClearStrategy);
			gl.Viewport(0, 0, XEngineContext.GLControl.Width, XEngineContext.GLControl.Height);
			Draw();
		}
		internal void _Exit()
		{
			if (!Initialized) return;
			Initialized = false;
			Exit();
			Clear();
		}

		protected virtual void Init() { }
		protected virtual void Draw() { Prepare(); SyncScene(); DrawScene(); }
		protected virtual void Exit() { }

		protected void Prepare()
		{
			// [Assert: Algs.SyncObjectPouch.Count == 0]
			// [Assert: Algs.ObjectQueue.Count == 0]
			// [Assert: Algs.DrawableObjectPouch.Count == 0]

			foreach (var gameObject in GameObjects)
			{
				if (gameObject.parent != null) Algs.SyncObjectPouch.Add(gameObject.parent, gameObject);
				if (gameObject.IsDrawable) Algs.DrawableObjectPouch.Add(gameObject.material.shader, gameObject);
			}
		}
		protected void SyncScene()
		{
			foreach (var gameObject in GameObjects)
			{
				if (gameObject.parent != null) continue;
				
				foreach (var obj in Algs.SyncLevelOrder(gameObject))
				{
					obj.Sync();
				}
			}

			MainCamera.Adjust();

			// [Assert: Algs.SyncObjectPouch.Count == 0]
			// [Assert: Algs.ObjectQueue.Count == 0]
		}
		protected void DrawScene()
		{
			var gl = XEngineContext.Graphics;

			foreach (var shader in XEngineContext.CompiledShaders)
			{
				shader.Use();

				if (shader.Eye != -1) gl.Uniform3(shader.Eye, MainCamera.Position.x, MainCamera.Position.y, MainCamera.Position.z);
				if (shader.Project != -1) gl.UniformMatrix4(shader.Project, 1, false, MainCamera.ViewToProjectData);
				if (shader.View != -1) gl.UniformMatrix4(shader.View, 1, false, MainCamera.WorldToViewData);

				while (Algs.DrawableObjectPouch.Retrieve(shader, out var gameObject))
				{
					gameObject.Draw();
				}
			}

			// [Assert: Algs.DrawableObjectPouch.Count == 0]
		}
	}
}
