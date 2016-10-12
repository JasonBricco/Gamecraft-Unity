using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Text;

public enum CommandType
{
	WalkSpeed,
	FlySpeed,
	Teleport,
	ChangeTime,
	ShowDebug,
	ToggleReticle
}

public sealed class Commands : ScriptableObject, IUpdatable 
{
	private InputField command;
	private Text error;
	private bool active = false;
	
	private string[] args;

	private Dictionary<string, UnityAction> functions = new Dictionary<string, UnityAction>();

	private void Awake()
	{
		Updater.Register(this);

		command = UIStore.GetUI<InputField>("CommandInput");
		error = UIStore.GetUI<Text>("CommandError");

		UIStore.GetUI<Button>("CommandReadyButton").onClick.AddListener(ProcessCommand);
		UIStore.GetUI<Button>("CancelCommandButton").onClick.AddListener(EndCommand);

		Events.OnCommand += (input, args) => { EndCommand(); };

		AddFunctions();
	}

	public void UpdateTick()
	{
		if (active && Input.GetKeyDown(KeyCode.Return))
			ProcessCommand();

		if (Engine.CurrentState != GameState.Playing) return;

		if (Input.GetKeyDown(KeyCode.Slash))
		{
			Events.SendGameEvent(GameEventType.EnteringCommand);
			active = true;
			command.Select();
			command.ActivateInputField();
			Engine.ChangeState(GameState.Paused);
		}
	}

	public void ProcessCommand()
	{
		string modified = command.text.ToLower();
		modified = modified.Trim();

		StringBuilder sb = new StringBuilder(modified.Length);

		int whiteCount = 0;

		for (int i = 0; i < modified.Length; i++)
		{
			if (!Char.IsWhiteSpace(modified[i]))
			{
				sb.Append(modified[i]);
				whiteCount = 0;
			}
			else
			{
				whiteCount++;

				if (whiteCount == 1) 
					sb.Append(modified[i]);
			}
		}

		modified = sb.ToString();
		args = modified.Split(' ');

		if (functions.ContainsKey(args[0]))
			functions[args[0]].Invoke();
		else 
			ShowError("The command entered does not exist!");
	}

	public void EndCommand()
	{
		active = false;
		command.text = "";
		Engine.ChangeState(GameState.Playing);
	}

	private void ShowError(string message)
	{
		error.text = message;
		error.enabled = true;
		UIFader.CancelCurrent();
		UIFader.FadeIn(error, 0.1f);
		UIFader.FadeOut(error, 3.0f, 1.0f);
	}

	private void ChangeWalkingSpeed()
	{
		if (args.Length != 2)
		{
			ShowError("Usage: walkspeed [speed]");
			return;
		}

		if (!IsValidNumber(args[1]))
		{
			ShowError("Invalid number.");
			return;
		}

		Events.SendCommand(CommandType.WalkSpeed, args);
	}

	private void ChangeFlyingSpeed()
	{
		if (args.Length != 3)
		{
			ShowError("Usage: flyspeed [horizontal speed] [vertical speed]");
			return;
		}
		
		if (!IsValidNumber(args[1]) || !IsValidNumber(args[2]))
		{
			ShowError("Invalid number.");
			return;
		}
		
		Events.SendCommand(CommandType.FlySpeed, args);
	}

	private void Teleport()
	{
		if (args.Length != 4)
		{
			ShowError("Usage: teleport [x] [y] [z]");
			return;
		}
		
		if (!IsValidNumber(args[1]) || !IsValidNumber(args[2]) || !IsValidNumber(args[3]))
		{
			ShowError("Invalid number.");
			return;
		}
		
		Events.SendCommand(CommandType.Teleport, args);
	}

	private void ChangeTime()
	{
		if (args.Length != 2)
		{
			ShowError("Usage: time [minutes]");
			return;
		}

		if (!IsValidNumber(args[1]))
		{
			ShowError("Invalid number.");
			return;
		}

		if (!IsPositive(args[1]))
		{
			ShowError("Minutes must be positive.");
			return;
		}

		Events.SendCommand(CommandType.ChangeTime, args);
	}

	private void ToggleDebugInfo()
	{
		if (args.Length != 1)
		{
			ShowError("Command \"Debug\" has no arguments.");
			return;
		}

		GameObject debug = UIStore.GetObject("Debug");
		debug.SetActive(!debug.activeSelf);
		Events.SendCommand(CommandType.ShowDebug, args);
	}

	private void ToggleReticle()
	{
		if (args.Length != 1)
		{
			ShowError("Command \"Reticle\" has no arguments.");
			return;
		}

		Events.SendCommand(CommandType.ToggleReticle, args);
	}

	private bool IsValidNumber(string val)
	{
		for (int i = 0; i < val.Length; i++)
		{
			if (!Char.IsDigit(val[i]) && val[i] != '-')
				return false;
		}

		if (val.Length > 6) return false;

		return true;
	}

	private bool IsPositive(string val)
	{
		for (int i = 0; i < val.Length; i++)
		{
			if (val[i] == '-')
				return false;
		}

		return true;
	}

	private void AddFunctions()
	{
		functions.Add("walkspeed", ChangeWalkingSpeed);
		functions.Add("flyspeed", ChangeFlyingSpeed);
		functions.Add("teleport", Teleport);
		functions.Add("time", ChangeTime);
		functions.Add("debug", ToggleDebugInfo);
		functions.Add("reticle", ToggleReticle);
	}
}
