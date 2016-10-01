using UnityEngine;
using UnityEngine.UI;

public class Settings : ScriptableObject 
{
	private Slider sensitivitySlider;
	private Text sensitivityText;
	private Text viewRangeText;

	private static int cameraSensitivity = 5;
	private static int viewRange = 200;
	
	private static int fogStart = 0;
	private static int fogEnd = 0;

	private static float tickSpeed = 0.25f;

	public static int CameraSensitivity
	{
		get { return cameraSensitivity; }
		set { cameraSensitivity = value; }
	}

	public static int FogStart
	{
		get { return fogStart; }
	}
	
	public static int FogEnd
	{
		get { return fogEnd; }
	}
	
	public static int ViewRange
	{
		get { return viewRange; }
	}

	public static float TickSpeed
	{
		get { return tickSpeed; }
	}
	
	private void Awake()
	{
		sensitivitySlider = UIStore.GetUI<Slider>("CamSensitivitySlider");
		sensitivityText = UIStore.GetUI<Text>("CamSensValue");
		viewRangeText = UIStore.GetUI<Text>("ViewRangeValue");

		SetCameraSensitivity(PlayerPrefs.HasKey("CameraSensitivity") ? PlayerPrefs.GetInt("CameraSensitivity") : 5);
		SetViewRange(PlayerPrefs.HasKey("ViewRange") ? PlayerPrefs.GetInt("ViewRange") : 200);
		SetTickSpeed(PlayerPrefs.HasKey("TickSpeed") ? PlayerPrefs.GetFloat("TickSpeed") : 0.25f, UIStore.GetUI<Text>("TickSpeedValue"));
	}

	public void SetViewRange(int range)
	{
		viewRange = Mathf.Clamp(range, 40, 300);
		Camera.main.farClipPlane = viewRange;
		RenderSettings.fogStartDistance = fogStart = viewRange - 40;
		RenderSettings.fogEndDistance = fogEnd = viewRange;
		viewRangeText.text = Settings.ViewRange.ToString();
	}

	public void SetCameraSensitivity(int value)
	{
		sensitivitySlider.value = value;
	}

	public void SetSensitivity(float value)
	{
		Settings.CameraSensitivity = (int)value;
		sensitivityText.text = value.ToString("F0");
	}

	public static void SetTickSpeed(float val, Text t)
	{
		if (Mathf.Approximately(val, 0.25f))
		{
			tickSpeed = 0.25f;
			t.text = "Fast";
		}
		else if (Mathf.Approximately(val, 0.50f))
		{
			tickSpeed = 0.50f;
			t.text = "Medium";
		}
		else
		{
			tickSpeed = 1.0f;
			t.text = "Slow";
		}
	}

	public void Reset()
	{
		PlayerPrefs.DeleteAll();
		SetCameraSensitivity(5);
		SetViewRange(200);
	}

	public void OnDestroy()
	{
		PlayerPrefs.SetInt("CameraSensitivity", cameraSensitivity);
		PlayerPrefs.SetInt("ViewRange", viewRange);
		PlayerPrefs.SetFloat("TickSpeed", tickSpeed);
	}
}
