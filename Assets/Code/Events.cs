using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum GameEventType 
{ 
	BeginPlay,
	Exit,
	GenerateLight,
	EnteringCommand,
	GeneratingIsland
}

public sealed class Events : MonoBehaviour
{	
	public delegate void GameEvent(GameEventType type);
	public static event GameEvent OnGameEvent;

	public delegate void CommandEvent(CommandType command, string[] args);
	public static event CommandEvent OnCommand;

	public delegate void StateEvent(GameState state);
	public static event StateEvent OnStateChange;

	public delegate void SaveEvent(SerializableData data);
	public static event SaveEvent OnSave;

	public static void SendGameEvent(GameEventType type)
	{
		if (OnGameEvent != null) OnGameEvent(type);
	}

	public static void SendCommand(CommandType command, string[] args)
	{
		if (OnCommand != null) OnCommand(command, args);
	}

	public static void SendStateEvent(GameState state)
	{
		if (OnStateChange != null) OnStateChange(state);
	}

	public static void SendSaveEvent(SerializableData data)
	{
		if (OnSave != null) OnSave(data);
	}
}
