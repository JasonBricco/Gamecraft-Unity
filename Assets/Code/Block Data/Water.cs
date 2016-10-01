using UnityEngine;

public class Water : Fluid 
{
	public Water(ushort ID)
	{
		genericID = ID;
		name = "Water";
		fluidLevel = (ID - BlockType.Water1) + 1;
		canDelete = fluidLevel == 5;
		elements.SetAll(0.0f);
	}

	public override void OnPlace(Vector3i normal, int x, int y, int z)
	{
		FluidSimulator.AddFluidAndFlow(x, y, z, BlockType.Water5);
	}

	public override void OnDelete(int x, int y, int z)
	{
		if (BlockRegistry.GetBlock(Map.GetBlockSafe(x, y + 1, z)).IsFluid)
		{
			Map.SetBlock(x, y, z, BlockType.Water5);
			return;
		}
		else
			FluidSimulator.RemoveFluidAndUnflow(x, y, z, genericID);
	}
}
