using UnityEngine;
using UnityEngine.UI;

public class ShowDirection : MonoBehaviour 
{
	private Text label;

	private void Awake()
	{
		label = GetComponent<Text>();
	}

	private void Update()
	{
		int dir = Player.GetRotation();
		label.text = "Facing: " + Direction.GetStringAsOrientation(dir);
	}
}
