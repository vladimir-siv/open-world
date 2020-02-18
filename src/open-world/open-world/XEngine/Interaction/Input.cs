using System.Windows.Input;

using GlmNet;

using Cursor = System.Windows.Forms.Cursor;
using Control = System.Windows.Forms.Control;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using SystemInformation = System.Windows.Forms.SystemInformation;

namespace XEngine.Interaction
{
	public static class Input
	{
		private static bool Initialized = false;

		public static vec2 LastMousePosition { get; private set; } = new vec2(Cursor.Position.X, Cursor.Position.Y);
		public static vec2 MousePosition { get; private set; } = new vec2(Cursor.Position.X, Cursor.Position.Y);
		public static vec2 MouseDelta => MousePosition - LastMousePosition;
		public static float ScrollDelta { get; private set; } = 0.0f;

		public static bool IsCursorInsideRenderingArea => XEngineContext.GLControl.ClientRectangle.Contains(XEngineContext.GLControl.PointToClient(Cursor.Position));

		public static bool IsKeyDown(Key key) => Keyboard.IsKeyDown((System.Windows.Input.Key)key);
		public static bool MouseButtonsPressed(MouseButtons buttons) => Control.MouseButtons.HasFlag((System.Windows.Forms.MouseButtons)buttons);

		internal static void Init()
		{
			if (Initialized) return;
			XEngineContext.GLControl.MouseWheel += OnScroll;
			Initialized = true;
		}
		internal static void Update()
		{
			if (!Initialized) return;
			if (!Host.CurrentApplicationIsActive) return;
			LastMousePosition = MousePosition;
			MousePosition = new vec2(Cursor.Position.X, Cursor.Position.Y);
			ScrollDelta = 0.0f;
		}
		internal static void Exit()
		{
			if (!Initialized) return;
			XEngineContext.GLControl.MouseWheel -= OnScroll;
			Initialized = false;
		}

		private static void OnScroll(object sender, MouseEventArgs e) => ScrollDelta += e.Delta / SystemInformation.MouseWheelScrollDelta;
	}
}
