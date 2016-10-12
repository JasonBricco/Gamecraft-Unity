using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class SerializableData
{
	public int genID, seed2D, seed3D, voronoiSeed;
	public int[] modified;
	public List<int> chunkIndices = new List<int>();
	public List<int> countList = new List<int>();
	public List<ushort> dataList = new List<ushort>();
	public Vector3 playerPos = new Vector3(-1, -1, -1);
}
	
public sealed class MapData : ScriptableObject
{
	private static string path;
	private static bool allowSave;

	private static SerializableData loadedData = null;

	public static SerializableData LoadedData 
	{
		get { return loadedData; }
	}

	private void Awake()
	{
		path = Application.persistentDataPath + "/MapData.txt";

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
			SerializableData data = new SerializableData();
			Events.SendSaveEvent(data);
			Map.Encode(data);
			data.genID = Map.GenID;

			FileStream stream = new FileStream(path, FileMode.Create);
			StreamWriter dataWriter = new StreamWriter(stream);
			string json = JsonUtility.ToJson(data);
			dataWriter.Write(json);
			dataWriter.Close();
		}
	}

	public static void Load()
	{
		allowSave = true;

		if (File.Exists(path))
		{
			FileStream stream = new FileStream(path, FileMode.Open);
			StreamReader reader = new StreamReader(stream);
			string json = reader.ReadToEnd();
			loadedData = JsonUtility.FromJson<SerializableData>(json);
			reader.Close();
		}
	}

	public static void DeleteAll()
	{
		if (File.Exists(path)) File.Delete(path);

		allowSave = false;
		Engine.SignalQuit();
	}
}
