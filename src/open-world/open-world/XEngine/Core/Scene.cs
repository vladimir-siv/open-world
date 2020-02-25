using System;
using System.Collections.Generic;
using SharpGL;
using GlmNet;

namespace XEngine.Core
{
	using XEngine.Structures;
	using XEngine.Shading;
	using XEngine.Lighting;
	using XEngine.Common;

	public abstract class Scene
	{
		private class Algorithms
		{
			public int LightingState { get; private set; } = 0;
			public uint LightCount { get; private set; } = 0u;

			public readonly Heap<float, LightSource> LightsHeap = new Heap<float, LightSource>(float.NegativeInfinity);
			public readonly List<LightSource> OrderedLights = new List<LightSource>(8);
			public readonly Pouch<GameObject, GameObject> SyncObjectPouch = new Pouch<GameObject, GameObject>();
			public readonly Queue<GameObject> ObjectQueue = new Queue<GameObject>();
			public readonly Pouch3L<Shader, Mesh, Material, GameObject> DrawableObjectPouch = new Pouch3L<Shader, Mesh, Material, GameObject>();

			public void Invalidate()
			{
				LightingState = 0;
				LightCount = 0u;
			}

			public void UpdateLights(IEnumerable<LightSource> lights, vec3 camera, uint count)
			{
				var change = false;

				LightCount = 0u;
				LightsHeap.Clear(); // Generates garbage

				foreach (var light in lights)
				{
					LightsHeap.Insert(light ^ camera, light);
				}

				for (LightCount = 0u; LightCount < count; ++LightCount)
				{
					if (LightsHeap.Count == 0) break;

					var i = (int)LightCount;

					var light = LightsHeap.RemoveMin().Data;

					if (i < OrderedLights.Count)
					{
						if (LightSource.AreEqual(light, OrderedLights[i])) continue;
						OrderedLights[i] = light;
					}
					else OrderedLights.Add(light);

					change = true;
				}

				if (change) ++LightingState;
			}
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
		private bool Initialized = false;
		protected uint ClearStrategy { get; set; } = OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT;
		private readonly Algorithms Algs = new Algorithms();

		public Camera MainCamera { get; private set; } = new Camera();
		internal int CameraState { get; private set; } = 0;

		internal int AmbientState { get; private set; } = 0;

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
				++AmbientState;
			}
		}
		public float SkyboxScale { get; set; } = 250.0f;

		private float _FogDensity = 0.005f;
		public float FogDensity
		{
			get
			{
				return _FogDensity;
			}
			set
			{
				if (value == _FogDensity) return;
				_FogDensity = value;
				++AmbientState;
			}
		}
		private float _FogGradient = 10f;
		public float FogGradient
		{
			get
			{
				return _FogGradient;
			}
			set
			{
				if (value == _FogGradient) return;
				_FogGradient = value;
				++AmbientState;
			}
		}

		private AmbientLight _AmbientLight = AmbientLight.Bright;
		public AmbientLight AmbientLight
		{
			get
			{
				return _AmbientLight;
			}
			set
			{
				if (AmbientLight.AreEqual(value, _AmbientLight)) return;
				AmbientLight = value;
				++AmbientState;
			}
		}

		public uint ActiveLights { get; set; } = 8u;
		internal readonly LinkedList<LightSource> Lights = new LinkedList<LightSource>();
		internal int LightingState => Algs.LightingState;
		internal uint LightCount => Algs.LightCount;
		internal LightSource GetLight(int i)
		{
			if (i >= ActiveLights) throw new ApplicationException("Invalid light requested.");
			if (i < Algs.LightCount) return Algs.OrderedLights[i];
			else return LightSource.PitchBlack;
		}
		
		internal readonly LinkedList<GameObject> GameObjects = new LinkedList<GameObject>();

		public void AddLight(LightSource lightSource)
		{
			Lights.AddLast(lightSource);
		}
		public void ClearLights()
		{
			Lights.Clear();
		}

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

		private void Invalidate()
		{
			var gl = XEngineContext.Graphics;

			ClearStrategy = OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT;

			Algs.Invalidate();

			MainCamera.LocalPosition = vector3.zero;
			MainCamera.LocalRotation = vector3.zero;
			CameraState = 0;

			AmbientState = 0;
			if (Skybox != null) gl.ClearColor(Skybox.SkyColor.r, Skybox.SkyColor.g, Skybox.SkyColor.b, Skybox.SkyColor.a);
			SkyboxScale = 250.0f;
			_FogDensity = 0.005f;
			_FogGradient = 10f;

			_AmbientLight = AmbientLight.Bright;

			ActiveLights = 8u;

			foreach (var shader in XEngineContext.Shaders) shader.Value.Invalidate();
		}

		internal void _Init()
		{
			if (Initialized) return;
			XEngineState.Reset();
			Invalidate();
			Init();
			if (Skybox == null) Skybox = Skybox.Default;
			if (Lights.Count == 0) AddLight(LightSource.Sun);
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
			Clear();
			ClearLights();
			Exit();
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

			MainCamera.Adjust(); ++CameraState;
			Algs.UpdateLights(Lights, MainCamera.Position, ActiveLights);
		}
		protected void DrawScene()
		{
			foreach (var gameObject in Algs.DrawableObjectPouch.Retrieve())
			{
				gameObject.Draw();
			}

			Skybox?.Draw(SkyboxScale);
		}
	}
}
