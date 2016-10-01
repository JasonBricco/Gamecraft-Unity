using UnityEngine;

public class TreeTrunk : Block 
{
	public TreeTrunk()
	{
		name = "Tree Trunk";
		genericID = BlockType.TreeTrunk;
		elements.SetLayered(7.0f, 6.0f, 7.0f);
	}
}
