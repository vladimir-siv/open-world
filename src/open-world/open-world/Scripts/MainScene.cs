using System.Windows.Input;

using GlmNet;
using SharpGL;

using XEngine;
using XEngine.Core;
using XEngine.Data;
using XEngine.Shading;

using Cursor = System.Windows.Forms.Cursor;
using Control = System.Windows.Forms.Control;
using MouseButtons = System.Windows.Forms.MouseButtons;

namespace open_world.Scripts
{
	[GenerateScene("OpenWorld.MainScene")]
	public class MainScene : Scene
	{
		private GameObject MaleHead;

		private vec2 ShapeRotationAngle { get; set; } = new vec2(0.0f, 0.0f);
		private float Scroll { get; set; } = 0.0f;
		private vec2 LastMousePosition { get; set; } = new vec2(Cursor.Position.X, Cursor.Position.Y);

		protected override void Init()
		{
			var gl = Graphics;

			gl.Enable(OpenGL.GL_DEPTH_TEST);
			gl.Enable(OpenGL.GL_CULL_FACE);

			gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

			MainCamera.Position = new vec3(-30.0f, 20.0f, 30.0f);
			MainCamera.LookAt(new vec3(0.0f, 0.0f, 0.0f));

			GraphicsControl.MouseWheel += (s, e) => Scroll += e.Delta / System.Windows.Forms.SystemInformation.MouseWheelScrollDelta;

			MaleHead = new GameObject("MaleHead");
			MaleHead.mesh = new Mesh();
			MaleHead.mesh.LoadModel("male_head", VertexAttribute.POSITION | VertexAttribute.NORMAL).Wait();
			MaleHead.mesh.material = new Material(Shader.Find("phong"));
			MaleHead.mesh.material.Set("material_color", new vec3(232 / 255f, 176 / 255f, 141 / 255f));
			MaleHead.mesh.material.Set("ambient_light_color", new vec3(1.0f, 1.0f, 1.0f));
			MaleHead.mesh.material.Set("ambient_light_power", 0.25f);
			MaleHead.mesh.material.Set("light_source_position", new vec3(-15.0f, 40.0f, 30.0f));
			MaleHead.mesh.material.Set("light_source_color", new vec3(1.0f, 1.0f, 1.0f));
			MaleHead.mesh.material.Set("light_source_power", 60.0f);
		}

		private void Update()
		{
			if (!Host.CurrentApplicationIsActive) return;

			if (GraphicsControl.ClientRectangle.Contains(GraphicsControl.PointToClient(Cursor.Position)))
			{
				var delta = new vec2(Cursor.Position.X, Cursor.Position.Y) - LastMousePosition;

				// Rotate camera
				if (Control.MouseButtons.HasFlag(MouseButtons.Middle))
				{
					MainCamera.Rotate(delta);
				}

				// Rotate shape
				if (Control.MouseButtons.HasFlag(MouseButtons.Left))
				{
					ShapeRotationAngle += new vec2(delta.y, delta.x);
				}
			}

			// Move camera
			var moveDelta = new vec3(0.0f, 0.0f, 0.0f);

			if (Keyboard.IsKeyDown(Key.W)) moveDelta.z -= 1.0f;
			if (Keyboard.IsKeyDown(Key.S)) moveDelta.z += 1.0f;
			if (Keyboard.IsKeyDown(Key.A)) moveDelta.x -= 1.0f;
			if (Keyboard.IsKeyDown(Key.D)) moveDelta.x += 1.0f;
			if (Control.MouseButtons.HasFlag(MouseButtons.XButton1)) moveDelta.y -= 1.0f;
			if (Control.MouseButtons.HasFlag(MouseButtons.XButton2)) moveDelta.y += 1.0f;

			moveDelta.z -= Scroll;
			Scroll = 0.0f;

			MainCamera.Move(moveDelta);
		}

		private void WrapUp()
		{
			LastMousePosition = new vec2(Cursor.Position.X, Cursor.Position.Y);
		}

		protected override void Draw()
		{
			var gl = Graphics;

			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);
			gl.Viewport(0, 0, GraphicsControl.Width, GraphicsControl.Height);

			MainCamera.SetAspectRatio((float)GraphicsControl.Width / (float)GraphicsControl.Height);

			Update();
			WrapUp();

			MaleHead.transform.rotation = new vec3(ShapeRotationAngle, 0.0f);
			MaleHead.SyncTransform();
			MaleHead.Draw();
		}
	}
}
