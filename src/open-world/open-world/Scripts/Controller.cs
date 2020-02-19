using XEngine;
using XEngine.Core;

namespace open_world.Scripts
{
	[XEngineActivation(nameof(Init))]
	public static class Controller
	{
		private static void Init()
		{
			SceneManager.LoadScene("OpenWorld.MainScene");
		}
	}
}
