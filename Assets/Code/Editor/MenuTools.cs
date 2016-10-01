using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;

public static class MenuTools 
{
	[MenuItem("Tools/Open Save Folder")]
	private static void OpenSaveFolder()
	{
		EditorUtility.RevealInFinder(Application.persistentDataPath);
	}

	[MenuItem("Tools/Clear/Console %#m")]
	private static void ClearConsole()
	{
		var logEntries = Type.GetType("UnityEditorInternal.LogEntries, UnityEditor.dll");
		var clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
		clearMethod.Invoke(null, null);
	}

	[MenuItem("Tools/Clear/Player Prefs")]
	private static void ClearPlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
	
	[MenuItem("Tools/Toggle Active State %k")]
	private static void ToggleActiveState()
	{
		GameObject obj = Selection.activeGameObject;
		
		if (obj != null)
		{
			if (obj.activeSelf)
				obj.SetActive(false);
			else
				obj.SetActive(true);
		}
	}
}
