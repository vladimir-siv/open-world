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
			this.cbModelLocalScale = new System.Windows.Forms.CheckBox();
			this.cbModelLocalTranslate = new System.Windows.Forms.CheckBox();
			this.cbParentModelToUser = new System.Windows.Forms.CheckBox();
			this.cbCameraLocalTranslate = new System.Windows.Forms.CheckBox();
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
			this.pnlUI.Controls.Add(this.cbModelLocalScale);
			this.pnlUI.Controls.Add(this.cbModelLocalTranslate);
			this.pnlUI.Controls.Add(this.cbParentModelToUser);
			this.pnlUI.Controls.Add(this.cbCameraLocalTranslate);
			this.pnlUI.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlUI.Location = new System.Drawing.Point(0, 0);
			this.pnlUI.Name = "pnlUI";
			this.pnlUI.Size = new System.Drawing.Size(800, 25);
			this.pnlUI.TabIndex = 1;
			// 
			// cbModelLocalScale
			// 
			this.cbModelLocalScale.AutoSize = true;
			this.cbModelLocalScale.Location = new System.Drawing.Point(549, 4);
			this.cbModelLocalScale.Name = "cbModelLocalScale";
			this.cbModelLocalScale.Size = new System.Drawing.Size(122, 17);
			this.cbModelLocalScale.TabIndex = 3;
			this.cbModelLocalScale.Text = "Model Local Scaling";
			this.cbModelLocalScale.UseVisualStyleBackColor = true;
			// 
			// cbModelLocalTranslate
			// 
			this.cbModelLocalTranslate.AutoSize = true;
			this.cbModelLocalTranslate.Location = new System.Drawing.Point(400, 4);
			this.cbModelLocalTranslate.Name = "cbModelLocalTranslate";
			this.cbModelLocalTranslate.Size = new System.Drawing.Size(139, 17);
			this.cbModelLocalTranslate.TabIndex = 2;
			this.cbModelLocalTranslate.Text = "Model Local Translation";
			this.cbModelLocalTranslate.UseVisualStyleBackColor = true;
			// 
			// cbParentModelToUser
			// 
			this.cbParentModelToUser.AutoSize = true;
			this.cbParentModelToUser.Location = new System.Drawing.Point(264, 4);
			this.cbParentModelToUser.Name = "cbParentModelToUser";
			this.cbParentModelToUser.Size = new System.Drawing.Size(126, 17);
			this.cbParentModelToUser.TabIndex = 1;
			this.cbParentModelToUser.Text = "Parent Model to User";
			this.cbParentModelToUser.UseVisualStyleBackColor = true;
			// 
			// cbCameraLocalTranslate
			// 
			this.cbCameraLocalTranslate.AutoSize = true;
			this.cbCameraLocalTranslate.Location = new System.Drawing.Point(108, 5);
			this.cbCameraLocalTranslate.Name = "cbCameraLocalTranslate";
			this.cbCameraLocalTranslate.Size = new System.Drawing.Size(146, 17);
			this.cbCameraLocalTranslate.TabIndex = 0;
			this.cbCameraLocalTranslate.Text = "Camera Local Translation";
			this.cbCameraLocalTranslate.UseVisualStyleBackColor = true;
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
		internal System.Windows.Forms.CheckBox cbParentModelToUser;
		internal System.Windows.Forms.CheckBox cbCameraLocalTranslate;
		internal System.Windows.Forms.CheckBox cbModelLocalTranslate;
		internal System.Windows.Forms.CheckBox cbModelLocalScale;
	}
}

