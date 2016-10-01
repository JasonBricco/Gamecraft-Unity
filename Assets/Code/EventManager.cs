using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum GameEventType 
{ 
	BeginPlay,
	SaveWorld,
	GenerateLight,
	EnteringCommand,
	GeneratingIsland
}

public class EventManager : MonoBehaviour
{	
	public delegate void GameEvent(GameEventType type);
	public static event GameEvent OnGameEvent;

	public delegate void CommandEvent(CommandType command, string[] args);
	public static event CommandEvent OnCommand;

	public delegate void StateEvent(GameState state);
	public static event StateEvent OnStateChange;

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
}
