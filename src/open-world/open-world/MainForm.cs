using System;
using System.Windows.Forms;

using SharpGL;
using XEngine.Core;

namespace open_world
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void OpenGLControl_OpenGLInitialized(object sender, EventArgs e)
		{
			SceneManager.CurrentScene.Init((OpenGLControl)sender, Width, Height);
		}

		private void OpenGLControl_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
		{
			SceneManager.CurrentScene.Draw((OpenGLControl)sender);
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			SceneManager.CurrentScene.Exit(OpenGLControl);
		}
	}
}
