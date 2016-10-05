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
			Block block = PlayerInteraction.CurrentBlock.block;
			label.text = "Block: " + (int)block.ID + " (" + block.Name() + ")";
			break;

		case DebugInfo.Position:
			BlockInstance blockInst = PlayerInteraction.CurrentBlock;
			label.text = "Position: " + blockInst.x + " " + blockInst.y + " " + blockInst.z;
			break;
		}
	}
}
