using System;
using System.Windows.Forms;
using SharpGL;
using XEngine;

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
			XEngineActivator.InitEngine(OpenGLControl);
		}

		private void OpenGLControl_OpenGLDraw(object sender, RenderEventArgs args)
		{
			XEngineContext.Draw();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			XEngineActivator.Shutdown();
		}
	}
}
