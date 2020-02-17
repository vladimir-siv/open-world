namespace open_world
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OpenGLControl = new SharpGL.OpenGLControl();
			((System.ComponentModel.ISupportInitialize)(this.OpenGLControl)).BeginInit();
			this.SuspendLayout();
			// 
			// OpenGLControl
			// 
			this.OpenGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OpenGLControl.DrawFPS = false;
			this.OpenGLControl.FrameRate = 60;
			this.OpenGLControl.Location = new System.Drawing.Point(0, 0);
			this.OpenGLControl.Name = "OpenGLControl";
			this.OpenGLControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL4_4;
			this.OpenGLControl.RenderContextType = SharpGL.RenderContextType.NativeWindow;
			this.OpenGLControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
			this.OpenGLControl.Size = new System.Drawing.Size(800, 600);
			this.OpenGLControl.TabIndex = 0;
			this.OpenGLControl.OpenGLInitialized += new System.EventHandler(this.OpenGLControl_OpenGLInitialized);
			this.OpenGLControl.OpenGLDraw += new SharpGL.RenderEventHandler(this.OpenGLControl_OpenGLDraw);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.Controls.Add(this.OpenGLControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainForm";
			this.ShowIcon = false;
			this.Text = "Open World";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.OpenGLControl)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private SharpGL.OpenGLControl OpenGLControl;
	}
}

