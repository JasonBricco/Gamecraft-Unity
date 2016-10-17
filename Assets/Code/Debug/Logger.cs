using UnityEngine;
using System;
using System.IO;
using System.Text;

public sealed class Logger : ScriptableObject
{
	private static string dataPath;

	private void Awake()
	{
		dataPath = Application.persistentDataPath;
		Application.logMessageReceived += HandleError;
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

	public static void LogWarning(params string[] items)
	{
		Debug.LogWarning(items[0]);
		Log(items);
	}

	public static void LogError(params string[] items)
	{
		Debug.LogError(items[0]);
		Log(items);
	}

	public static void VerifyCollection(Array collection, string name)
	{
		for (int i = 0; i < collection.Length; i++)
		{
			if (collection.GetValue(i) == null)
				Debug.LogError("Element " + i + " in the collection " + name + " is null!");
		}
	}
}
