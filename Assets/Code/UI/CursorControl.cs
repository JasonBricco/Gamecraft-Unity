using UnityEngine;

public class CursorControl : ScriptableObject 
{
	private Texture2D cursor;

	private void Awake()
	{
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
	}
}
