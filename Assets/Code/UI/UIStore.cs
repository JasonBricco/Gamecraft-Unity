using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public sealed class UIStore : MonoBehaviour 
{
	private static Dictionary<string, GameObject> items = new Dictionary<string, GameObject>();

	private void Awake()
	{
		Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);

		for (int i = 0; i < children.Length; i++)
			items.Add(children[i].name, children[i].gameObject);
	}

	public static GameObject GetObject(string name)
	{
		return items[name];
	}

	public static T GetUI<T>(string name)
	{
		return items[name].GetComponent<T>();
	}
}
