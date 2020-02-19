using System.Collections.Generic;

namespace XEngine.Core
{
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
	}
}
