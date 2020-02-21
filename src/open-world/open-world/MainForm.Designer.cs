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
			this.pnlUI = new System.Windows.Forms.Panel();
			this.btnPrevScene = new System.Windows.Forms.Button();
			this.btnNextScene = new System.Windows.Forms.Button();
			this.cbMode4 = new System.Windows.Forms.CheckBox();
			this.cbMode3 = new System.Windows.Forms.CheckBox();
			this.cbMode2 = new System.Windows.Forms.CheckBox();
			this.cbMode1 = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.OpenGLControl)).BeginInit();
			this.pnlUI.SuspendLayout();
			this.SuspendLayout();
			// 
			// OpenGLControl
			// 
			this.OpenGLControl.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.OpenGLControl.DrawFPS = false;
			this.OpenGLControl.FrameRate = 60;
			this.OpenGLControl.Location = new System.Drawing.Point(0, 25);
			this.OpenGLControl.Name = "OpenGLControl";
			this.OpenGLControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL4_4;
			this.OpenGLControl.RenderContextType = SharpGL.RenderContextType.NativeWindow;
			this.OpenGLControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
			this.OpenGLControl.Size = new System.Drawing.Size(800, 600);
			this.OpenGLControl.TabIndex = 0;
			this.OpenGLControl.OpenGLInitialized += new System.EventHandler(this.OpenGLControl_OpenGLInitialized);
			this.OpenGLControl.OpenGLDraw += new SharpGL.RenderEventHandler(this.OpenGLControl_OpenGLDraw);
			// 
			// pnlUI
			// 
			this.pnlUI.Controls.Add(this.btnPrevScene);
			this.pnlUI.Controls.Add(this.btnNextScene);
			this.pnlUI.Controls.Add(this.cbMode4);
			this.pnlUI.Controls.Add(this.cbMode3);
			this.pnlUI.Controls.Add(this.cbMode2);
			this.pnlUI.Controls.Add(this.cbMode1);
			this.pnlUI.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlUI.Location = new System.Drawing.Point(0, 0);
			this.pnlUI.Name = "pnlUI";
			this.pnlUI.Size = new System.Drawing.Size(800, 25);
			this.pnlUI.TabIndex = 1;
			// 
			// btnPrevScene
			// 
			this.btnPrevScene.Dock = System.Windows.Forms.DockStyle.Left;
			this.btnPrevScene.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPrevScene.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnPrevScene.Location = new System.Drawing.Point(0, 0);
			this.btnPrevScene.Name = "btnPrevScene";
			this.btnPrevScene.Size = new System.Drawing.Size(75, 25);
			this.btnPrevScene.TabIndex = 0;
			this.btnPrevScene.Text = "< Scene";
			this.btnPrevScene.UseVisualStyleBackColor = true;
			// 
			// btnNextScene
			// 
			this.btnNextScene.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnNextScene.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnNextScene.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnNextScene.Location = new System.Drawing.Point(725, 0);
			this.btnNextScene.Name = "btnNextScene";
			this.btnNextScene.Size = new System.Drawing.Size(75, 25);
			this.btnNextScene.TabIndex = 4;
			this.btnNextScene.Text = "Scene >";
			this.btnNextScene.UseVisualStyleBackColor = true;
			// 
			// cbMode4
			// 
			this.cbMode4.AutoSize = true;
			this.cbMode4.Location = new System.Drawing.Point(483, 4);
			this.cbMode4.Name = "cbMode4";
			this.cbMode4.Size = new System.Drawing.Size(59, 17);
			this.cbMode4.TabIndex = 4;
			this.cbMode4.Text = "Mode4";
			this.cbMode4.UseVisualStyleBackColor = true;
			// 
			// cbMode3
			// 
			this.cbMode3.AutoSize = true;
			this.cbMode3.Location = new System.Drawing.Point(403, 4);
			this.cbMode3.Name = "cbMode3";
			this.cbMode3.Size = new System.Drawing.Size(59, 17);
			this.cbMode3.TabIndex = 3;
			this.cbMode3.Text = "Mode3";
			this.cbMode3.UseVisualStyleBackColor = true;
			// 
			// cbMode2
			// 
			this.cbMode2.AutoSize = true;
			this.cbMode2.Location = new System.Drawing.Point(323, 4);
			this.cbMode2.Name = "cbMode2";
			this.cbMode2.Size = new System.Drawing.Size(59, 17);
			this.cbMode2.TabIndex = 2;
			this.cbMode2.Text = "Mode2";
			this.cbMode2.UseVisualStyleBackColor = true;
			// 
			// cbMode1
			// 
			this.cbMode1.AutoSize = true;
			this.cbMode1.Location = new System.Drawing.Point(243, 4);
			this.cbMode1.Name = "cbMode1";
			this.cbMode1.Size = new System.Drawing.Size(59, 17);
			this.cbMode1.TabIndex = 1;
			this.cbMode1.Text = "Mode1";
			this.cbMode1.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 625);
			this.Controls.Add(this.pnlUI);
			this.Controls.Add(this.OpenGLControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainForm";
			this.ShowIcon = false;
			this.Text = "Open World";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.OpenGLControl)).EndInit();
			this.pnlUI.ResumeLayout(false);
			this.pnlUI.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private SharpGL.OpenGLControl OpenGLControl;
		private System.Windows.Forms.Panel pnlUI;
		internal System.Windows.Forms.CheckBox cbMode2;
		internal System.Windows.Forms.CheckBox cbMode1;
		internal System.Windows.Forms.CheckBox cbMode3;
		internal System.Windows.Forms.CheckBox cbMode4;
		internal System.Windows.Forms.Button btnPrevScene;
		internal System.Windows.Forms.Button btnNextScene;
	}
}

