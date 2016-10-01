using UnityEngine;

public class Grass : Block 
{
	public Grass()
	{
		name = "Grass";
		genericID = BlockType.Grass;
		elements.SetLayered(1.0f, 2.0f, 3.0f);
	}
}
