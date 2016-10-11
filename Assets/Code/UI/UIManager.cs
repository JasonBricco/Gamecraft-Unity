using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour 
{
	private GameObject mainMenuWindow;
	private GameObject pauseWindow;
	private GameObject timeWindow;
	private GameObject settingsWindow;
	private GameObject loadingWindow;
	private GameObject generationWindow;
	private GameObject worldTypesWindow;
	private GameObject commandsWindow;
	private GameObject blockWindow;
	private GameObject structureWindow;

	private GameObject cursor;

	private GameObject[] modeWindows = new GameObject[2];
	private KeyCode[] keys = { KeyCode.Alpha1, KeyCode.Alpha2 };

	private void Start()
	{
		mainMenuWindow = UIStore.GetObject("MainMenu");
		pauseWindow = UIStore.GetObject("PauseWindow");
		timeWindow = UIStore.GetObject("TimeWindow");
		settingsWindow = UIStore.GetObject("SettingsWindow");
		loadingWindow = UIStore.GetObject("LoadingWindow");
		generationWindow = UIStore.GetObject("GenerationWindow");
		worldTypesWindow = UIStore.GetObject("WorldTypesWindow");
		commandsWindow = UIStore.GetObject("CommandsWindow");
		blockWindow = UIStore.GetObject("BlockWindow");
		structureWindow = UIStore.GetObject("StructureWindow");

		cursor = UIStore.GetObject("Cursor");

		modeWindows[0] = blockWindow;
		modeWindows[1] = structureWindow;

		blockWindow.SetActive(false);
		mainMenuWindow.SetActive(true);

		EventManager.OnStateChange += (state) => 
		{
			if (state == GameState.Paused || state == GameState.MainMenu)
				cursor.SetActive(false);
			
			if (state == GameState.Playing)
			{
				DisableActiveWindows();
				cursor.SetActive(true);
			}
		};

		EventManager.OnGameEvent += (type) => 
		{
			switch (type)
			{
			case GameEventType.BeginPlay:
				loadingWindow.SetActive(false);
				break;

			case GameEventType.GeneratingIsland:
				worldTypesWindow.SetActive(false);
				generationWindow.SetActive(true);
				break;

			case GameEventType.GenerateLight:
				mainMenuWindow.SetActive(false);
				generationWindow.SetActive(false);
				loadingWindow.SetActive(true);
				break;

			case GameEventType.EnteringCommand:
				commandsWindow.SetActive(true);
				break;
			}
		};
	}

	private void Update()
	{
		for (int i = 0; i < modeWindows.Length; i++)
		{
			if (Input.GetKeyDown(keys[i]))
			{
				if (modeWindows[i].activeSelf)
				{
					modeWindows[i].SetActive(false);
					Engine.ChangeState(GameState.Playing);
				}
				else
				{
					if (Engine.CurrentState == GameState.Playing)
					{
						Engine.ChangeState(GameState.Paused);
						modeWindows[i].SetActive(true);
					}
				}
			}
		}

		if (Engine.CurrentState != GameState.Playing) return;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Engine.ChangeState(GameState.Paused);
			pauseWindow.SetActive(true);
		}
	}

	private void DisableActiveWindows()
	{
		if (loadingWindow.activeSelf) loadingWindow.SetActive(false);
		if (pauseWindow.activeSelf) pauseWindow.SetActive(false);
		if (timeWindow.activeSelf) timeWindow.SetActive(false);
		if (commandsWindow.activeSelf) commandsWindow.SetActive(false);
	}

	public void PlayButtonHandler(MainMenu menu)
	{
		menu.TryPlay(worldTypesWindow);
	}

	public void SelectWorldTypeHandler(WorldTypeUIData data)
	{
		data.Menu.SetTypeAndBuild(data.WorldType);
	}

	public void UnpauseButtonHandler()
	{
		pauseWindow.SetActive(false);
		Engine.ChangeState(GameState.Playing);
	}

	public void BlockSelectedHandler(BlockUIData data)
	{
		blockWindow.SetActive(false);
		data.interactionClass.BlockSelected(data.blockID);
	}

	public void StructureSelectedHandler(BlockUIData data)
	{
		structureWindow.SetActive(false);
		data.interactionClass.StructureSelected(data.blockID);
	}

	public void SetDayButtonHandler(Environment env)
	{
		env.SetDay();
	}

	public void SetNightButtonHandler(Environment env)
	{
		env.SetNight();
	}

	public void StopTimeButtonHandler(Environment env)
	{
		env.StopTime();
	}

	public void StopTimeTextHandler(Text buttonText)
	{
		buttonText.text = buttonText.text == "Stop Time" ? "Start Time" : "Stop Time";
	}

	public void ToggleRainButtonHandler(Environment env)
	{
		env.ToggleRain();
	}

	public void TimeButtonHandler()
	{
		pauseWindow.SetActive(false);
		timeWindow.SetActive(true);
	}

	public void TimeBackButtonHandler()
	{
		timeWindow.SetActive(false);
		pauseWindow.SetActive(true);
	}

	public void ProcessCommandButtonHandler(Commands c)
	{
		c.ProcessCommand();
	}

	public void CancelCommandButtonHandler(Commands c)
	{
		c.EndCommand();
	}

	public void SettingsButtonHandler()
	{
		pauseWindow.SetActive(false);
		settingsWindow.SetActive(true);
	}

	public void SettingsBackButtonHandler()
	{
		settingsWindow.SetActive(false);
		pauseWindow.SetActive(true);
	}

	public void DeleteSaveButtonHandler()
	{
		MapData.DeleteAll();
	}

	public void QuitButtonHandler()
	{
		Engine.SignalQuit();
	}
}
