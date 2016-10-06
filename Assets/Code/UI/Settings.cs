using UnityEngine;
using UnityEngine.UI;

public sealed class Settings : MonoBehaviour 
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

	public void AdjustViewRange(int amount)
	{
		SetViewRange(viewRange + amount);
	}

	private void SetViewRange(int range)
	{
		viewRange = Mathf.Clamp(range, 40, 300);
		Camera.main.farClipPlane = viewRange;
		RenderSettings.fogStartDistance = fogStart = viewRange - 40;
		RenderSettings.fogEndDistance = fogEnd = viewRange;
		viewRangeText.text = viewRange.ToString();
	}

	private void SetCameraSensitivity(int value)
	{
		sensitivitySlider.value = value;
	}

	public void SetSensitivity(float value)
	{
		cameraSensitivity = (int)value;
		sensitivityText.text = value.ToString("F0");
	}

	public void ChangeTickSpeed()
	{
		Text speedText = UIStore.GetUI<Text>("TickSpeedValue");

		switch (speedText.text)
		{
		case "Fast":
			SetTickSpeed(0.50f, speedText);
			break;

		case "Medium":
			SetTickSpeed(1.0f, speedText);
			break;

		case "Slow":
			SetTickSpeed(0.25f, speedText);
			break;
		}
	}

	private void SetTickSpeed(float val, Text t)
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
		SetTickSpeed(0.25f, UIStore.GetUI<Text>("TickSpeedValue"));
		SetViewRange(200);
	}

	private void OnDestroy()
	{
		PlayerPrefs.SetInt("CameraSensitivity", cameraSensitivity);
		PlayerPrefs.SetInt("ViewRange", viewRange);
		PlayerPrefs.SetFloat("TickSpeed", tickSpeed);
	}
}
