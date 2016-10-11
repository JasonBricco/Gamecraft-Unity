using UnityEngine;
using UnityEngine.UI;

public enum DebugInfo
{
	ID,
	Position
}

public sealed class ShowBlockInfo : MonoBehaviour 
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
			Block block = MapInteraction.CurrentBlock.block;
			label.text = "Block: " + (int)block.ID + " (" + block.Name() + "), Data: " + block.data;
			break;

		case DebugInfo.Position:
			BlockInstance blockInst = MapInteraction.CurrentBlock;
			label.text = "Position: " + blockInst.x + " " + blockInst.y + " " + blockInst.z;
			break;
		}
	}
}
