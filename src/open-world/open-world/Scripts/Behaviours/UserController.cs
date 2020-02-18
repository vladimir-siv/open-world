using GlmNet;
using XEngine;
using XEngine.Core;
using XEngine.Scripting;
using XEngine.Interaction;

namespace open_world.Scripts.Behaviours
{
	public class UserController : XBehaviour
	{
		public GameObject MaleHead = null;

		protected override void Update()
		{
			if (!Host.CurrentApplicationIsActive) return;

			if (Input.IsCursorInsideRenderingArea)
			{
				var delta = Input.MouseDelta;

				// Rotate camera
				if (Input.MouseButtonsPressed(MouseButtons.Middle))
				{
					MainCamera.Rotate(delta);
				}

				// Rotate shape
				if (Input.MouseButtonsPressed(MouseButtons.Left))
				{
					MaleHead.transform.rotation += new vec3(delta.y, delta.x, 0.0f);
				}
			}

			// Move camera
			var moveDelta = new vec3(0.0f, 0.0f, -Input.ScrollDelta);

			if (Input.IsKeyDown(Key.W)) moveDelta.z -= 1.0f;
			if (Input.IsKeyDown(Key.S)) moveDelta.z += 1.0f;
			if (Input.IsKeyDown(Key.A)) moveDelta.x -= 1.0f;
			if (Input.IsKeyDown(Key.D)) moveDelta.x += 1.0f;
			if (Input.MouseButtonsPressed(MouseButtons.XButton1)) moveDelta.y -= 1.0f;
			if (Input.MouseButtonsPressed(MouseButtons.XButton2)) moveDelta.y += 1.0f;

			MainCamera.Move(moveDelta);
		}
	}
}
