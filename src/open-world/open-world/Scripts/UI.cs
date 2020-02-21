using XEngine;
using XEngine.Core;

using Button = System.Windows.Forms.Button;
using CheckBox = System.Windows.Forms.CheckBox;

namespace open_world
{
	[XEngineActivation(nameof(Init))]
	public static class UI
	{
		public static Button PrevScene { get; private set; } = null;
		public static Button NextScene { get; private set; } = null;
		public static CheckBox Mode1 { get; private set; } = null;
		public static CheckBox Mode2 { get; private set; } = null;
		public static CheckBox Mode3 { get; private set; } = null;
		public static CheckBox Mode4 { get; private set; } = null;

		private static void Init()
		{
			var MainForm = (MainForm)XEngineContext.GLControl.ParentForm;
			PrevScene = MainForm.btnPrevScene;
			NextScene = MainForm.btnNextScene;
			Mode1 = MainForm.cbMode1;
			Mode2 = MainForm.cbMode2;
			Mode3 = MainForm.cbMode3;
			Mode4 = MainForm.cbMode4;
			SceneManager.SceneChanged += OnSceneChanged;
		}

		private static void OnSceneChanged(Scene last, Scene current)
		{
			var MainForm = (MainForm)XEngineContext.GLControl.ParentForm;

			MainForm.cbMode1.Checked = false;
			MainForm.cbMode2.Checked = false;
			MainForm.cbMode3.Checked = false;
			MainForm.cbMode4.Checked = false;

			if (current.SceneId == "OpenWorld.MainScene")
			{
				MainForm.cbMode1.Text = "Mode1";
				MainForm.cbMode1.Location = new System.Drawing.Point(243, 4);
				MainForm.cbMode2.Text = "Mode2";
				MainForm.cbMode2.Location = new System.Drawing.Point(323, 4);
				MainForm.cbMode3.Text = "Mode3";
				MainForm.cbMode3.Location = new System.Drawing.Point(403, 4);
				MainForm.cbMode4.Text = "Mode4";
				MainForm.cbMode4.Location = new System.Drawing.Point(483, 4);
				return;
			}

			if (current.SceneId == "OpenWorld.TestScene")
			{
				MainForm.cbMode1.Text = "Camera Local Translation";
				MainForm.cbMode1.Location = new System.Drawing.Point(119, 4);
				MainForm.cbMode2.Text = "Parent Model to User";
				MainForm.cbMode2.Location = new System.Drawing.Point(275, 4);
				MainForm.cbMode3.Text = "Model Local Translation";
				MainForm.cbMode3.Location = new System.Drawing.Point(411, 4);
				MainForm.cbMode4.Text = "Model Local Scaling";
				MainForm.cbMode4.Location = new System.Drawing.Point(560, 4);
				return;
			}
		}
	}
}
