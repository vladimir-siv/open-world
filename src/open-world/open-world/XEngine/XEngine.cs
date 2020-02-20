using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SharpGL;

namespace XEngine
{
	using XEngine.Core;
	using XEngine.Shading;
	using XEngine.Interaction;

	public static class XEngineActivator
	{
		public static void InitEngine(OpenGLControl control)
		{
			if (XEngineContext.GLControl != null) return;
			XEngineContext.GLControl = control;

			Input.Init();

			foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass))
			{
				var engActivateAttr = type.GetCustomAttributes(typeof(XEngineActivationAttribute), false);
				if (engActivateAttr.Length == 0) continue;
				var attr = (XEngineActivationAttribute)engActivateAttr[0];
				type.GetMethod(attr.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
			}

			SceneManager.LoadScene(SceneManager.MainSceneId);
		}

		public static void Shutdown()
		{
			if (XEngineContext.GLControl == null) return;
			foreach (var scene in Scene.SceneCache) scene.Value._Exit();
			XEngineContext.Graphics.UseProgram(0);
			foreach (var shader in XEngineContext.Shaders) shader.Value.Clean();
			XEngineContext.Shaders.Clear();
			XEngineContext.CompiledShaders.Clear();
			XEngineContext.GLControl = null;
		}
	}

	public static class XEngineContext
	{
		internal static OpenGLControl GLControl { get; set; } = null;
		internal static OpenGL Graphics => GLControl.OpenGL;
		internal static Dictionary<string, Shader> Shaders { get; } = new Dictionary<string, Shader>();
		internal static LinkedList<Shader> CompiledShaders = new LinkedList<Shader>();

		public static void Draw()
		{
			Input.Update();
			SceneManager.CurrentScene._Draw();
			Input.Late();
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class XEngineActivationAttribute : Attribute
	{
		public string MethodName { get; }
		public XEngineActivationAttribute(string methodName) => MethodName = methodName;
	}
}
