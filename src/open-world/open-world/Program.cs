using System;
using System.Windows.Forms;
using XEngine;

namespace open_world
{
	public static class Program
	{
		[STAThread] private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			XEngineActivator.InitEngine();
			Application.Run(new MainForm());
		}
	}
}
