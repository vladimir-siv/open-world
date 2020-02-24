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
			public readonly Pouch3L<Shader, Mesh, Material, GameObject> DrawableObjectPouch = new Pouch3L<Shader, Mesh, Material, GameObject>();
			
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
		public Camera MainCamera { get; private set; } = new Camera();

		private Skybox _Skybox = null;
		public Skybox Skybox
		{
			get
			{
				return _Skybox;
			}
			set
			{
				if (_Skybox == value) return;
				_Skybox = value;
				var color = value.SkyColor;
				var gl = XEngineContext.Graphics;
				gl.ClearColor(color.r, color.g, color.b, color.a);
			}
		}

		public float FogDensity { get; set; } = 0.0125f;
		public float FogGradient { get; set; } = 7.5f;

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
			XEngineState.Reset();
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
			foreach (var gameObject in GameObjects)
			{
				if (gameObject.parent != null) Algs.SyncObjectPouch.Add(gameObject.parent, gameObject);
				if (gameObject.IsDrawable && !gameObject.DisableRendering) Algs.DrawableObjectPouch.Add(gameObject.material.shader, gameObject.mesh, gameObject.material, gameObject);
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
		}
		protected void DrawScene()
		{
			foreach (var gameObject in Algs.DrawableObjectPouch.Retrieve())
			{
				gameObject.Draw();
			}
		}
	}
}
