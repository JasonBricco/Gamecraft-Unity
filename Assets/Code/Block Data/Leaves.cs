using UnityEngine;

public class Leaves : Block 
{
	public Leaves()
	{
		name = "Leaves";
		genericID = BlockType.Leaves;
		meshIndex = 0; // TODO: Leaves should be cutout. Fix element below when changing.
		elements.SetAll(12.0f);
	}

	public override bool IsFaceVisible(ushort neighbor, int face)
	{
		if (neighbor == BlockType.Leaves)
		{
			if (face == Direction.Left || face == Direction.Back || face == Direction.Down) 
				return false;

			return true;
		}

		return base.IsFaceVisible(neighbor, face);
	}

	public override CullType GetCullType(int face)
	{
		return CullType.Cutout;
	}
}
