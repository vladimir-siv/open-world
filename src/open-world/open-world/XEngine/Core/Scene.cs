using System.Collections.Generic;

namespace XEngine.Core
{
	using XEngine.Structures;
	using XEngine.Shading;

	public abstract class Scene
	{
		internal static Dictionary<string, Scene> SceneCache = new Dictionary<string, Scene>();
		public static Scene Resolve(string sceneId) => SceneCache[sceneId];

		public string SceneId { get; internal set; } = string.Empty;
		public Camera MainCamera { get; protected set; } = new Camera();

		internal readonly LinkedList<GameObject> GameObjects = new LinkedList<GameObject>();
		private bool Initialized = false;
		
		public void Add(GameObject gameObject)
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
			Init();
			foreach (var gameObject in GameObjects) gameObject.Awake();
			foreach (var gameObject in GameObjects) gameObject.Start();
			Initialized = true;
		}
		internal void _Draw()
		{
			if (!Initialized) return;
			MainCamera.SetAspectRatio((float)XEngineContext.GLControl.Width / (float)XEngineContext.GLControl.Height);
			foreach (var gameObject in GameObjects) gameObject.Update();
			foreach (var gameObject in GameObjects) gameObject.Late();
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
		protected virtual void Draw() { }
		protected virtual void Exit() { }

		#region Sync Scene

		protected void SyncScene()
		{

		}

		#endregion

		#region Draw Scene

		private Pouch<Shader, GameObject> DrawableObjectPouch = new Pouch<Shader, GameObject>();

		protected void DrawScene()
		{
			// [Assert: DrawableObjectPouch.Count == 0]

			foreach (var gameObject in GameObjects)
			{
				if (!gameObject.IsDrawable) continue;
				var shader = gameObject.mesh.material.shader;
				DrawableObjectPouch.Add(shader, gameObject);
			}

			foreach (var shader in Shader.CompiledShaders)
			{
				while (DrawableObjectPouch.Retrieve(shader, out var gameObject))
				{
					gameObject.Draw();
				}
			}

			// [Assert: DrawableObjectPouch.Count == 0]
		}

		#endregion
	}
}
