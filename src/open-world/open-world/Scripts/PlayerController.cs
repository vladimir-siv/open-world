﻿using System;
using GlmNet;
using XEngine.Core;
using XEngine.Scripting;
using XEngine.Interaction;
using XEngine.Common;

namespace open_world
{
	public class PlayerController : XBehaviour
	{
		public float MovementSpeed = 0.5f;
		public float RotationSpeed = 0.2f;
		
		private void PrevScene(object sender, EventArgs e) => SceneManager.LoadScene("OpenWorld.MainScene");
		private void NextScene(object sender, EventArgs e) => SceneManager.LoadScene("OpenWorld.TestScene");
		protected override void Start() { UI.PrevScene.Click += PrevScene; UI.NextScene.Click += NextScene; }
		protected override void Destroy() { UI.PrevScene.Click -= PrevScene; UI.NextScene.Click -= NextScene; }

		protected override void Update()
		{
			if (!Input.IsCurrentApplicationActive) return;
			if (!Input.IsCursorInsideRenderingArea) return;
			
			var rotation = new vec3(Input.MouseDelta.y, Input.MouseDelta.x, 0.0f);
			var forward = 0.0f;
			var right = 0.0f;
			var up = 0.0f;
			var shift = 1.0f;

			if (Input.IsKeyDown(Key.W)) forward += 1.0f;
			if (Input.IsKeyDown(Key.S)) forward -= 1.0f;
			if (Input.IsKeyDown(Key.A)) right -= 1.0f;
			if (Input.IsKeyDown(Key.D)) right += 1.0f;
			if (Input.IsKeyDown(Key.LeftShift)) shift = 2.0f;
			if (Input.MouseButtonsPressed(MouseButtons.XButton1)) up -= 1.0f;
			if (Input.MouseButtonsPressed(MouseButtons.XButton2)) up += 1.0f;
			
			if (Input.MouseButtonsPressed(MouseButtons.Middle)) gameObject.transform.rotation -= rotation * RotationSpeed;
			var directions = gameObject.transform.WorldSpaceUnits;
			gameObject.transform.position += (forward * directions.forward + right * directions.right + up * vector3.up).normalize() * MovementSpeed * shift;
			
			if (Input.IsKeyDown(Key.R)) gameObject.transform = new Transform(new vec3(+0.0f, +10.0f, +0.0f), vector3.zero, vector3.one);

			if (Input.MouseButtonsPressed(MouseButtons.Left)) UI.Status.Text = MainCamera.ScreenToWorld(Input.LocalMousePosition).ToString();
			if (Input.MouseButtonsPressed(MouseButtons.Right)) UI.Status.Text = gameObject.transform.position.ToString();
			if (Input.IsKeyDown(Key.T)) UI.Status.Text = CurrentScene.ElapsedTime.ToString();
		}
	}
}
