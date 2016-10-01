using UnityEngine;

public class Pumpkin : Block
{
	public Pumpkin()
	{
		name = "Pumpkin";
		genericID = BlockType.Pumpkin;
	}

	public override ushort TryGetNonGenericID(Vector3i dir, int x, int y, int z)
	{
		return SetRotatedID(x, y, z);
	}
}

public class PumpkinLeft : Pumpkin
{
	public PumpkinLeft()
	{
		elements.SetAll(10.0f);
		elements.SetLeft(9.0f);
		elements.SetTop(11.0f);
	}
}

public class PumpkinRight : Pumpkin
{
	public PumpkinRight()
	{
		elements.SetAll(10.0f);
		elements.SetRight(9.0f);
		elements.SetTop(11.0f);
	}
}

public class PumpkinFront : Pumpkin
{
	public PumpkinFront()
	{
		elements.SetAll(10.0f);
		elements.SetFront(9.0f);
		elements.SetTop(11.0f);
	}
}

public class PumpkinBack : Pumpkin
{
	public PumpkinBack()
	{
		elements.SetAll(10.0f);
		elements.SetBack(9.0f);
		elements.SetTop(11.0f);
	}
}
