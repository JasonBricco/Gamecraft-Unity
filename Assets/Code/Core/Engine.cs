using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GameState
{
	MainMenu,
	Paused,
	Playing
}

public sealed class Engine : MonoBehaviour, IUpdatable
{	
	private static GameState currentState;
	private static bool signalQuit = false;

	private void Awake()
	{
		Updater.Register(this);

		ScriptableObject.CreateInstance<MapData>();
		ScriptableObject.CreateInstance<MaterialManager>();
		ScriptableObject.CreateInstance<ErrorHandling>();
		ScriptableObject.CreateInstance<CursorControl>();
		ScriptableObject.CreateInstance<FluidSimulator>();
		ScriptableObject.CreateInstance<Commands>();
		ScriptableObject.CreateInstance<ThreadManager>();
		ScriptableObject.CreateInstance<ChunkManager>();
		ScriptableObject.CreateInstance<EntityManager>();

		EventManager.OnStateChange += (state) => 
		{
			if (state == GameState.Paused)
				System.GC.Collect();
		};
	}

	public static void BeginPlay()
	{
		MapLight.GenerateAllLight();
	}

	public static GameState CurrentState
	{
		get { return currentState; }
	}

	public static void ChangeState(GameState newState)
	{
		currentState = newState;
		EventManager.SendStateEvent(newState);
	}

	public void UpdateTick()
	{
		if (signalQuit) Application.Quit();

		if (Input.GetKeyDown(KeyCode.P)) 
			Application.CaptureScreenshot(Application.persistentDataPath + "/Screenshot.png");
	}

	public static void SignalQuit()
	{
		signalQuit = true;
	}

	private void OnApplicationQuit()
	{
		EventManager.SendGameEvent(GameEventType.SaveWorld);
	}
}
