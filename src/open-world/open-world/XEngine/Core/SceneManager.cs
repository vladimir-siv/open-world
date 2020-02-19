using System;
using System.Linq;
using System.Reflection;

namespace XEngine.Core
{
	public static class SceneManager
	{
		public static string MainSceneId { get; private set; } = null;
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
				if (attr.IsMain) MainSceneId = scene.SceneId;
			}
		}

		public static void LoadScene(string sceneId, bool endLast = true) => LoadScene(Scene.Resolve(sceneId), endLast);
		public static void LoadScene(Scene scene, bool endLast = true)
		{
			if (endLast) CurrentScene?._Exit();
			CurrentScene = scene;
			CurrentScene._Init();
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class GenerateSceneAttribute : Attribute
	{
		public string SceneId { get; }
		public bool IsMain { get; }

		public GenerateSceneAttribute(string sceneId, bool isMain = false)
		{
			SceneId = sceneId;
			IsMain = isMain;
		}
	}
}
