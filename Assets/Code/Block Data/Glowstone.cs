using UnityEngine;

public class Glowstone : Block 
{
	public Glowstone()
	{
		name = "Glowstone";
		genericID = BlockType.Glowstone;
		elements.SetAll(8.0f);
		lightEmitted = (byte)(LightUtils.MaxLight + 1);
	}
}
