namespace XEngine.Scripting
{
	using XEngine.Core;

	public abstract class XBehaviour
	{
		protected static Camera MainCamera => SceneManager.CurrentScene.MainCamera;
		protected internal GameObject gameObject;

		internal void _Awake() => Awake();
		internal void _Start() => Start();
		internal void _Update() => Update();
		internal void _Late() => Late();
		internal void _Destroy() => Destroy();

		protected virtual void Awake() { }
		protected virtual void Start() { }
		protected virtual void Update() { }
		protected virtual void Late() { }
		protected virtual void Destroy() { }
	}
}
