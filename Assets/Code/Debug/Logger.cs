using UnityEngine;
using System;
using System.IO;
using System.Text;

public sealed class Logger : ScriptableObject, IUpdatable
{
	private static string dataPath;
	private static bool allowPrinting = true;

	private void Awake()
	{
		dataPath = Application.persistentDataPath;
		Application.logMessageReceived += HandleError;

		if (Debug.isDebugBuild)
			Updater.Register(this);
	}

	public void UpdateTick()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			allowPrinting = !allowPrinting;
			Debug.Log("Printing Enabled: " + allowPrinting);
		}
	}

	private void HandleError(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error)
		{
			Log(logString, stackTrace);
			Engine.SignalQuit();
		}
	}

	public static void Log(params string[] items)
	{
		StringBuilder text = new StringBuilder(System.DateTime.Now.ToString() + System.Environment.NewLine);

		for (int i = 0; i < items.Length; i++)
			text.AppendLine(items[i]);

		File.AppendAllText(dataPath + "/Log.txt", text.ToString() + System.Environment.NewLine);
	}

	public static void LogError(params string[] items)
	{
		Debug.LogError(items[0]);
	}

	[System.Diagnostics.Conditional("DEBUG")]
	public static void Print(string message)
	{
		if (allowPrinting) Debug.Log(message);
	}

	public static void VerifyCollection(Array collection, string name)
	{
		for (int i = 0; i < collection.Length; i++)
		{
			if (collection.GetValue(i) == null)
				LogError("Element " + i + " in the collection " + name + " is null!");
		}
	}
}
