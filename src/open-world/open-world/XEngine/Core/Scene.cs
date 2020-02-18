using System;
using System.Collections.Generic;
using SharpGL;
using GlmNet;

namespace XEngine.Core
{
	using XEngine.Shading;

	public abstract class Scene
	{
		internal static Dictionary<string, Scene> SceneCache = new Dictionary<string, Scene>();
		public static Scene Resolve(string sceneId) => SceneCache[sceneId];

		internal Dictionary<string, Shader> Shaders { get; } = new Dictionary<string, Shader>();

		public string SceneId { get; internal set; } = string.Empty;

		public OpenGLControl GraphicsControl { get; private set; } = null;
		public OpenGL Graphics => GraphicsControl.OpenGL;
		public vec2 ApplicationSize { get; private set; } = new vec2(0.0f, 0.0f);

		public Camera MainCamera { get; protected set; } = new Camera();
		public LinkedList<GameObject> GameObjects { get; private set; } = new LinkedList<GameObject>();

		public void _Init(OpenGLControl control, float width, float height)
		{
			if (control == null) throw new ArgumentNullException(nameof(control));
			if (GraphicsControl != null) throw new ApplicationException("Scene has already been initialized.");
			GraphicsControl = control; // redundant
			ApplicationSize = new vec2(width, height);
			Init();
		}
		public void _Draw(OpenGLControl control)
		{
			if (control == null) throw new ArgumentNullException(nameof(control));
			if (control != GraphicsControl) throw new ApplicationException("Invalid drawing control has been passed.");
			GraphicsControl = control;
			Draw();
		}
		public void _Exit(OpenGLControl control)
		{
			if (control == null) throw new ArgumentNullException(nameof(control));
			if (control != GraphicsControl) throw new ApplicationException("Invalid drawing control has been passed.");
			GraphicsControl = control; // redundant
			Exit();
			foreach (var gameObject in GameObjects) gameObject.Dispose();
			GameObjects.Clear();
			ApplicationSize = new vec2(0.0f, 0.0f);
			GraphicsControl = null;
		}

		protected virtual void Init() { }
		protected virtual void Draw() { }
		protected virtual void Exit() { }
	}
}
