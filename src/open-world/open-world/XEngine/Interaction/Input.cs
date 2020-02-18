using System.Windows.Input;

using SharpGL;
using GlmNet;

using Cursor = System.Windows.Forms.Cursor;
using Control = System.Windows.Forms.Control;

namespace XEngine.Interaction
{
	public static class Input
	{
		// readjust when creating rendering context
		private static OpenGLControl GLControl = null;

		public static vec2 LastMousePosition { get; private set; } = new vec2(Cursor.Position.X, Cursor.Position.Y);
		public static vec2 MousePosition { get; private set; } = new vec2(Cursor.Position.X, Cursor.Position.Y);
		public static vec2 MouseDelta => MousePosition - LastMousePosition;
		public static float ScrollDelta { get; private set; } = 0.0f;

		public static bool IsCursorInsideRenderingArea => GLControl.ClientRectangle.Contains(GLControl.PointToClient(Cursor.Position));

		public static bool IsKeyDown(Key key) => Keyboard.IsKeyDown((System.Windows.Input.Key)key);
		public static bool MouseButtonsPressed(MouseButtons buttons) => Control.MouseButtons.HasFlag((System.Windows.Forms.MouseButtons)buttons);

		public static void Init(OpenGLControl control)
		{
			if (GLControl != null) return;
			GLControl = control;
			GLControl.MouseWheel += (s, e) => ScrollDelta += e.Delta / System.Windows.Forms.SystemInformation.MouseWheelScrollDelta;
		}
		public static void Update()
		{
			if (!Host.CurrentApplicationIsActive) return;
			LastMousePosition = MousePosition;
			MousePosition = new vec2(Cursor.Position.X, Cursor.Position.Y);
			ScrollDelta = 0.0f;
		}
	}
}
