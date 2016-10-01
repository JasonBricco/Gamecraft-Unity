using UnityEngine;
using UnityEngine.UI;

public enum DebugInfo
{
	ID,
	Position
}

public class ShowBlockInfo : MonoBehaviour 
{
	[SerializeField] private DebugInfo info;
	private Text label;

	private void Awake()
	{
		label = GetComponent<Text>();
	}

	private void Update()
	{
		switch (info)
		{
		case DebugInfo.ID:
			ushort ID = PlayerInteraction.CurrentBlock.ID;
			label.text = "Block: " + ID + " (" + BlockRegistry.GetBlock(ID).Name + ")";
			break;

		case DebugInfo.Position:
			BlockInstance block = PlayerInteraction.CurrentBlock;
			label.text = "Position: " + block.x + " " + block.y + " " + block.z;
			break;
		}
	}
}
