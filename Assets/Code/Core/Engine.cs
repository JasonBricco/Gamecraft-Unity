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

	public static string Path { get; private set; }

	private void Awake()
	{
		Updater.Register(this);

		Path = Application.persistentDataPath + "/";

		ScriptableObject.CreateInstance<Map>();
		ScriptableObject.CreateInstance<MapData>();
		ScriptableObject.CreateInstance<MaterialManager>();
		ScriptableObject.CreateInstance<Logger>();
		ScriptableObject.CreateInstance<CursorControl>();
		ScriptableObject.CreateInstance<FluidSimulator>();
		ScriptableObject.CreateInstance<Commands>();
		ScriptableObject.CreateInstance<ThreadManager>();
		ScriptableObject.CreateInstance<EntityManager>();

		Events.OnStateChange += (state) => 
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
		Events.SendStateEvent(newState);
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
		Events.SendGameEvent(GameEventType.Exit);
	}
}
