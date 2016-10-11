using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public sealed class Environment : MonoBehaviour 
{
	private const int SecondsPerMinute = 60;

	private int dayLength = 20;

	private float seconds = 0;
	private int minutes = 0;

	private bool stopTime = false;

	private Color nightLight = new Color(0.06f, 0.06f, 0.06f, 1.0f);
	private Color dayLight = new Color(1.0f, 1.0f, 1.0f, 1.0f);

	private Color nightSky = new Color(0.0f, 0.0f, 0.0f, 1.0f);
	private Color daySky = new Color(0.45f, 0.86f, 1.0f, 1.0f);

	private Color rainLight = new Color(0.4f, 0.4f, 0.4f, 1.0f);
	private Color rainSky = new Color(0.17f, 0.36f, 0.42f, 1.0f);

	private int day = 0, dusk = 8, night = 10, dawn = 18;

	private bool forceFade = false;
	private bool fadeWeather = false;
	private float forceFadeTime = 1.0f;
	private float forceLerpAmount = 0.0f;
	private Color currentLight, currentSky, targetLight, targetSky;

	private GameObject rain;
	private bool isRaining = false;

	private bool? fadeFog = null;

	private Color Ambient
	{
		get { return RenderSettings.ambientLight; }
		set { RenderSettings.ambientLight = value; }
	}

	private Color SkyColor
	{
		get { return Camera.main.backgroundColor; }
		set
		{
			Camera.main.backgroundColor = value;
			RenderSettings.fogColor = value;
		}
	}

	public void SetDay()
	{
		ForceFade(dayLight, daySky, 1.0f, day);
		Engine.ChangeState(GameState.Playing);
	}

	public void SetNight()
	{
		ForceFade(nightLight, nightSky, 1.0f, night);
		Engine.ChangeState(GameState.Playing);
	}

	public void StopTime()
	{
		stopTime = !stopTime;
		Engine.ChangeState(GameState.Playing);
	}

	private void Awake()
	{
		rain = transform.Find("Rain").gameObject;

		EventManager.OnGameEvent += GameEventHandler;
		EventManager.OnCommand += CommandHandler;
	}

	private void GameEventHandler(GameEventType type)
	{
		if (type == GameEventType.BeginPlay)
		{
			Ambient = dayLight;
			SkyColor = daySky;
		}
	}

	private void CommandHandler(CommandType command, string[] args)
	{
		if (command == CommandType.ChangeTime)
		{
			minutes = int.Parse(args[1]) % dayLength;
			ForceFadeToTime(1.0f);
		}
	}

	private void Update()
	{
		if (Engine.CurrentState != GameState.Playing) return;

		if (fadeFog.HasValue)
		{
			if (fadeFog.Value)
			{
				RenderSettings.fogStartDistance = Mathf.Max(RenderSettings.fogStartDistance - Time.deltaTime * 100.0f, 0);
				RenderSettings.fogEndDistance = Mathf.Max(RenderSettings.fogEndDistance - Time.deltaTime * 100.0f, 40);
				
				if (RenderSettings.fogStartDistance == 0 && RenderSettings.fogEndDistance == 40)
					fadeFog = null;
			}
			else 
			{
				RenderSettings.fogStartDistance = Mathf.Min(RenderSettings.fogStartDistance + Time.deltaTime * 100.0f, Settings.FogStart);
				RenderSettings.fogEndDistance = Mathf.Min(RenderSettings.fogEndDistance + Time.deltaTime * 100.0f, Settings.FogEnd);
				
				if (RenderSettings.fogStartDistance == Settings.FogStart && RenderSettings.fogEndDistance == Settings.FogEnd)
					fadeFog = null;
			}
		}

		if (fadeWeather)
		{
			if (Ambient.r <= rainLight.r)
			{
				fadeWeather = false;
				return;
			}

			forceLerpAmount += Time.deltaTime / 4.0f;
			Lerp(Ambient, rainLight, SkyColor, rainSky, forceLerpAmount);

			return;
		}

		if (forceFade)
		{
			forceLerpAmount += Time.deltaTime / forceFadeTime;
			ConstrainedLerp(currentLight, targetLight, currentSky, targetSky, forceLerpAmount);

			if (Ambient == targetLight)
				forceFade = false;

			return;
		}

		if (stopTime) return;

		seconds += Time.deltaTime;

		if (seconds >= SecondsPerMinute)
		{
			minutes++;
			minutes = minutes % dayLength;
			seconds -= SecondsPerMinute;
		}

		if (minutes >= dusk && minutes < night)
		{
			float percent = GetTransitionPercent(dusk);
			ConstrainedLerp(dayLight, nightLight, daySky, nightSky, percent);
		}

		if (minutes >= dawn)
		{
			float percent = GetTransitionPercent(dawn);
			ConstrainedLerp(nightLight, dayLight, nightSky, daySky, percent);
		}
	}

	private float GetTransitionPercent(int transition)
	{
		float time = minutes + (seconds / SecondsPerMinute);
		return (time - transition) * 0.5f;
	}

	private void ConstrainedLerp(Color lightStart, Color lightEnd, Color skyStart, Color skyEnd, float lerpVal)
	{
		Color newLight = Color.Lerp(lightStart, lightEnd, lerpVal);
		Color newSky = Color.Lerp(skyStart, skyEnd, lerpVal);

		if (isRaining && newLight.r >= rainLight.r) return;
	
		Ambient = newLight;
		SkyColor = newSky;
	}

	private void Lerp(Color lightStart, Color lightEnd, Color skyStart, Color skyEnd, float lerpVal)
	{	
		Ambient = Color.Lerp(lightStart, lightEnd, lerpVal);
		SkyColor = Color.Lerp(skyStart, skyEnd, lerpVal);
	}

	private void ForceFade(Color targetLight, Color targetSky, float time, int newMinutes)
	{
		this.currentLight = Ambient;
		this.currentSky = SkyColor;
		this.targetLight = targetLight;
		this.targetSky = targetSky;

		forceFadeTime = time;

		minutes = newMinutes != -1 ? newMinutes : minutes;
		forceLerpAmount = 0.0f;
		forceFade = true;
	}

	private void ForceFadeToTime(float time)
	{
		currentLight = Ambient;
		currentSky = SkyColor;

		if (minutes >= day && minutes < dusk)
		{
			targetLight = dayLight;
			targetSky = daySky;
		}
		else if (minutes >= dusk && minutes < night)
		{
			float percent = GetTransitionPercent(dusk);
			targetLight = Color.Lerp(dayLight, nightLight, percent);
			targetSky = Color.Lerp(daySky, nightSky, percent);
		}
		else if (minutes >= night && minutes < dawn)
		{
			targetLight = nightLight;
			targetSky = nightSky;
		}
		else
		{
			float percent = GetTransitionPercent(dawn);
			targetLight = Color.Lerp(nightLight, dayLight, percent);
			targetSky = Color.Lerp(nightSky, daySky, percent);
		}

		forceFadeTime = time;
		forceLerpAmount = 0.0f;
		forceFade = true;
	}

	public void ToggleRain()
	{
		if (rain.activeSelf)
		{
			ForceFadeToTime(2.0f);
			fadeFog = false;
			isRaining = false;
			rain.SetActive(false);
		}
		else
		{
			forceLerpAmount = 0.0f;
			fadeWeather = true;
			fadeFog = true;
			isRaining = true;
			rain.SetActive(true);
		}

		Engine.ChangeState(GameState.Playing);
	}
}
