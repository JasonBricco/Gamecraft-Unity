using UnityEngine;
using System.Collections.Generic;

public sealed class MassBreak : StructureGenerator 
{
	private readonly Block air = new Block(BlockID.Air);

	public override void Generate(HitInfo info)
	{
		List<BlockInstance> blocks = new List<BlockInstance>();

		int x = info.hitPos.x;
		int y = info.hitPos.y;
		int z = info.hitPos.z;

		int dir = info.normal.GetNormalDirection();

		if (dir == Direction.Up || dir == Direction.Down)
			BreakHorizontal(x, y, z, blocks);
		else if (dir == Direction.Left || dir == Direction.Right)
			BreakZ(x, y, z, blocks);
		else
			BreakX(x, y, z, blocks);

		Map.SetBlocksAdvanced(blocks, true);
	}
	
	private void BreakHorizontal(int startX, int startY, int startZ, List<BlockInstance> blocks)
	{
		for (int x = startX - 1; x <= startX + 1; x++)
		{
			for (int z = startZ - 1; z <= startZ + 1; z++)
				blocks.Add(new BlockInstance(air, x, startY, z));
		}
	}
	
	private void BreakX(int startX, int startY, int startZ, List<BlockInstance> blocks)
	{
		for (int x = startX - 1; x <= startX + 1; x++)
		{
			for (int y = startY - 1; y <= startY + 1; y++)
				blocks.Add(new BlockInstance(air, x, y, startZ));
		}
	}
	
	private void BreakZ(int startX, int startY, int startZ, List<BlockInstance> blocks)
	{
		for (int z = startZ - 1; z <= startZ + 1; z++)
		{
			for (int y = startY - 1; y <= startY + 1; y++)
				blocks.Add(new BlockInstance(air, startX, y, z));
		}
	}
}
