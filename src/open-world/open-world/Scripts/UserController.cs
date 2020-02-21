using GlmNet;
using XEngine.Core;
using XEngine.Scripting;
using XEngine.Interaction;
using XEngine.Common;

using CheckBox = System.Windows.Forms.CheckBox;

namespace open_world
{
	public class UserController : XBehaviour
	{
		public CheckBox CameraLocalTranslate = null;
		public CheckBox ParentModelToUser = null;
		public CheckBox ModelLocalTranslate = null;
		public CheckBox ModelLocalScale = null;

		public GameObject Model = null;
		public float MovementSpeed = 1.0f;
		public float RotationSpeed = 0.2f;

		protected override void Start()
		{
			CameraLocalTranslate.CheckedChanged += (s, e) => MainCamera.LocalPosition = CameraLocalTranslate.Checked ? new vec3(-4.0f, +4.0f, +10.0f) : vector3.zero;
			ParentModelToUser.CheckedChanged += (s, e) => Model.parent = ParentModelToUser.Checked ? gameObject : null;
			ModelLocalTranslate.CheckedChanged += (s, e) => Model.transform.position = ModelLocalTranslate.Checked ? new vec3(+0.0f, -4.0f, -40.0f) : vector3.zero;
			ModelLocalScale.CheckedChanged += (s, e) => Model.transform.scale = ModelLocalScale.Checked ? new vec3(0.1f, 0.1f, 0.1f) : vector3.one;
		}

		protected override void Update()
		{
			if (!Input.IsCurrentApplicationActive) return;
			if (!Input.IsCursorInsideRenderingArea) return;

			var rotation = new vec3(Input.MouseDelta.y, Input.MouseDelta.x, 0.0f);
			var scale = Input.ScrollDelta / 10.0f;
			var forward = 0.0f;
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
			gameObject.transform.scale += scale;
			if (Input.MouseButtonsPressed(MouseButtons.Left)) Model.transform.rotation += rotation;

			if (Input.IsKeyDown(Key.R)) gameObject.transform = new Transform(new vec3(-30.0f, 20.0f, 30.0f), new vec3(-25.0f, -45.0f, 0.0f), vector3.one);
		}
	}
}
