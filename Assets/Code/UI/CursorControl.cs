using UnityEngine;
using UnityEngine.UI;

public class CursorControl : ScriptableObject, IUpdatable
{
	private Transform cursor;

	private void Awake()
	{
		cursor = UIStore.GetUI<Image>("Cursor").transform;

		EventManager.OnStateChange += (state) => 
		{
			if (state == GameState.Paused)
			{
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}

			if (state == GameState.Playing) Cursor.lockState = CursorLockMode.Locked;
		};

		Updater.Register(this);
	}

	public void UpdateTick()
	{
		if (Engine.CurrentState == GameState.Playing)
			cursor.position = Input.mousePosition;
	}
}
