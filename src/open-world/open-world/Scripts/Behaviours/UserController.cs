using GlmNet;
using XEngine.Core;
using XEngine.Scripting;
using XEngine.Interaction;
using XEngine.Extensions;

namespace open_world.Scripts.Behaviours
{
	public class UserController : XBehaviour
	{
		public GameObject MaleHead = null;
		public float MovementSpeed = 1.0f;
		public float RotationSpeed = 0.2f;

		protected override void Update()
		{
			if (!Input.IsCurrentApplicationActive) return;
			if (!Input.IsCursorInsideRenderingArea) return;

			var rotation = new vec3(Input.MouseDelta.y, Input.MouseDelta.x, 0.0f);
			var forward = Input.ScrollDelta;
			var right = 0.0f;
			var up = 0.0f;

			if (Input.IsKeyDown(Key.W)) forward += 1.0f;
			if (Input.IsKeyDown(Key.S)) forward -= 1.0f;
			if (Input.IsKeyDown(Key.A)) right -= 1.0f;
			if (Input.IsKeyDown(Key.D)) right += 1.0f;
			if (Input.MouseButtonsPressed(MouseButtons.XButton1)) up -= 1.0f;
			if (Input.MouseButtonsPressed(MouseButtons.XButton2)) up += 1.0f;

			if (Input.MouseButtonsPressed(MouseButtons.Middle)) gameObject.transform.rotation -= rotation * RotationSpeed;
			var directions = gameObject.transform.WorldSpaceUnits;
			gameObject.transform.position += (forward * directions.forward + right * directions.right + up * vector3.up).normalize() * MovementSpeed;
			if (Input.MouseButtonsPressed(MouseButtons.Left)) MaleHead.transform.rotation += rotation;
		}
	}
}
