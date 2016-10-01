using UnityEngine;

public class Sand : Block 
{
	public Sand()
	{
		name = "Sand";
		genericID = BlockType.Sand;
		elements.SetAll(4.0f);
	}
}
