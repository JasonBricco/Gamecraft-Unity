using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public sealed class SerializableData
{
	public Vector3 playerPos = new Vector3(-1, -1, -1);
}

[Serializable]
public sealed class SerializableChunk
{
	public int modified;
	public List<int> countList = new List<int>();
	public List<ushort> dataList = new List<ushort>();
}

public struct DecodableChunk
{
	public SerializableChunk chunk;
	public Vector2i pos;

	public DecodableChunk(SerializableChunk chunk, Vector2i pos)
	{
		this.chunk = chunk;
		this.pos = pos;
	}
}
	
public sealed class MapData : ScriptableObject
{
	private static string dataPath;
	private static bool allowSave;

	public static SerializableData LoadedData { get; private set; }

	private void Awake()
	{
		dataPath = Engine.Path + "MapData.txt";

		Events.OnGameEvent += (type) => 
		{
			if (type == GameEventType.Exit)
				Save();
		};
	}

	private void Save()
	{
		if (allowSave)
		{
			Map.Encode();

			SerializableData data = new SerializableData();
			Events.SendSaveEvent(data);

			FileStream stream = new FileStream(dataPath, FileMode.Create);
			StreamWriter dataWriter = new StreamWriter(stream);
			string json = JsonUtility.ToJson(data);
			dataWriter.Write(json);
			dataWriter.Close();
		}
	}

	public static void Load()
	{
		allowSave = true;

		if (File.Exists(dataPath))
		{
			FileStream stream = new FileStream(dataPath, FileMode.Open);
			StreamReader reader = new StreamReader(stream);
			string json = reader.ReadToEnd();
			LoadedData = JsonUtility.FromJson<SerializableData>(json);
			reader.Close();
		}
	}

	public static void DeleteAll()
	{
		if (File.Exists(dataPath)) File.Delete(dataPath);

		allowSave = false;
		Engine.SignalQuit();
	}
}
