using UnityEngine;
using System.Collections.Generic;

public class Updater : MonoBehaviour 
{
	private static List<IUpdatable> updateList = new List<IUpdatable>();

	public static void Register(IUpdatable item)
	{
		updateList.Add(item);
	}

	private void Update()
	{
		for (int i = 0; i < updateList.Count; i++)
			updateList[i].UpdateTick();
	}
}
