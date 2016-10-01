using UnityEngine;

public class WorldTypeUIData : MonoBehaviour 
{
	[SerializeField] private MainMenu menu;
	[SerializeField] private int worldType;

	public MainMenu Menu
	{
		get { return menu; }
	}

	public int WorldType
	{
		get { return worldType; }
	}
}
