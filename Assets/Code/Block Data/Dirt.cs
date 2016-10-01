using UnityEngine;

public class Dirt : Block 
{
	public Dirt()
	{
		name = "Dirt";
		genericID = BlockType.Dirt;
		elements.SetAll(3.0f);
	}
}
