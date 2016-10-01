using UnityEngine;

public class Stone : Block 
{
	public Stone()
	{
		name = "Stone";
		genericID = BlockType.Stone;
		elements.SetAll(5.0f);
	}
}
