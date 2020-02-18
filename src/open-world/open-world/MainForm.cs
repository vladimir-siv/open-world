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
			XEngine.Interaction.Input.Init(OpenGLControl); // move this from here later
			SceneManager.CurrentScene._Init(OpenGLControl, Width, Height);
		}

		private void OpenGLControl_OpenGLDraw(object sender, RenderEventArgs args)
		{
			SceneManager.CurrentScene._Draw(OpenGLControl);
			XEngine.Interaction.Input.Update(); // move this from here later
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			SceneManager.CurrentScene._Exit(OpenGLControl);
		}
	}
}
