using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class SerializableData
{
	public List<int> countList = new List<int>();
	public List<ushort> dataList = new List<ushort>();
	public Vector3 playerPos = new Vector3(-1, -1, -1);
}
	
public class MapData : ScriptableObject
{
	private static SerializableData data = new SerializableData();
	private static string path;
	private static bool allowSave;

	private void Awake()
	{
		path = Application.persistentDataPath + "/MapData.txt";

		EventManager.OnGameEvent += (type) => 
		{
			if (type == GameEventType.SaveWorld)
				Save();
		};
	}

	private void Save()
	{
		if (allowSave)
		{
			Encoder.Encode(data);
			FileStream stream = new FileStream(path, FileMode.Create);
			StreamWriter dataWriter = new StreamWriter(stream);
			string json = JsonUtility.ToJson(data);
			dataWriter.Write(json);
			dataWriter.Close();
		}
	}

	public static bool Load()
	{
		allowSave = true;

		if (File.Exists(path))
		{
			FileStream stream = new FileStream(path, FileMode.Open);
			StreamReader reader = new StreamReader(stream);
			string json = reader.ReadToEnd();
			data = JsonUtility.FromJson<SerializableData>(json);
			reader.Close();
			Encoder.Decode(data);

			return true;
		}

		return false;
	}

	public static SerializableData GetData()
	{
		return data;
	}

	public static void DeleteAll()
	{
		if (File.Exists(path)) File.Delete(path);

		allowSave = false;
		Engine.SignalQuit();
	}
}
