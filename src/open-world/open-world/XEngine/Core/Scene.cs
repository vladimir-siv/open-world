using System.Collections.Generic;

namespace XEngine.Core
{
	public abstract class Scene
	{
		internal static Dictionary<string, Scene> SceneCache = new Dictionary<string, Scene>();
		public static Scene Resolve(string sceneId) => SceneCache[sceneId];

		private bool Initialized = false;

		public string SceneId { get; internal set; } = string.Empty;
		public Camera MainCamera { get; protected set; } = new Camera();
		public LinkedList<GameObject> GameObjects { get; private set; } = new LinkedList<GameObject>();

		internal void _Init()
		{
			if (Initialized) return;
			Init();
			Initialized = true;
		}
		internal void _Draw()
		{
			if (Initialized) Draw();
		}
		internal void _Exit()
		{
			if (!Initialized) return;
			Exit();
			foreach (var gameObject in GameObjects) gameObject.Dispose();
			GameObjects.Clear();
			Initialized = false;
		}

		protected virtual void Init() { }
		protected virtual void Draw() { }
		protected virtual void Exit() { }
	}
}
