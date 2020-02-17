using XEngine;
using XEngine.Core;

namespace open_world.Scripts
{
	[XEngineActivation(nameof(Init))]
	public static class Controller
	{
		private static readonly string MainScene = "OpenWorld.TestScene";

		private static void Init()
		{
			SceneManager.LoadScene(MainScene);
		}
	}
}
