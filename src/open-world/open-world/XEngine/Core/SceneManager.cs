using System;
using System.Linq;
using System.Reflection;

namespace XEngine.Core
{
	public static class SceneManager
	{
		public static Scene CurrentScene { get; private set; } = null;

		static SceneManager()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(Scene).IsAssignableFrom(t)))
			{
				var genSceneAttr = type.GetCustomAttributes(typeof(GenerateSceneAttribute), false);
				if (genSceneAttr.Length == 0) continue;
				var attr = (GenerateSceneAttribute)genSceneAttr[0];
				var scene = (Scene)Activator.CreateInstance(type);
				scene.SceneId = attr.SceneId;
				Scene.SceneCache.Add(scene.SceneId, scene);
			}
		}

		public static void LoadScene(string sceneId, bool endLast = true)
		{
			if (endLast && CurrentScene != null) CurrentScene._Exit();
			CurrentScene = Scene.Resolve(sceneId);
			CurrentScene._Init();
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class GenerateSceneAttribute : Attribute
	{
		public string SceneId { get; }
		public GenerateSceneAttribute(string sceneId) => SceneId = sceneId;
	}
}
