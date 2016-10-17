using UnityEngine;
using System.Collections.Generic;

public sealed class TreeGenerator : StructureGenerator 
{
	private static readonly Block treeTrunk = new Block(BlockID.TreeTrunk);
	private static readonly Block leaves = new Block(BlockID.Leaves);

	public override void Generate(HitInfo info)
	{
		int x = info.adjPos.x;
		int y = info.adjPos.y;
		int z = info.adjPos.z;

		List<BlockInstance> blocks = new List<BlockInstance>();
		
		for (int i = y; i < y + 7; i++)
			blocks.Add(new BlockInstance(treeTrunk, x, i, z));
		
		for (int j = -3; j <= 3; j++)
		{
			for (int k = -3; k <= 3; k++)
			{
				for (int l = 7; l <= 10; l++)
					blocks.Add(new BlockInstance(leaves, x + j, y + l, z + k));
			}
		}
		
		Map.SetBlocksAdvanced(blocks, true);
	}

	public static void BoxyTreeTerrain(int x, int y, int z)
	{
		if (Map.GetBlock(x, y, z).ID != BlockID.Grass || Map.GetBlock(x, y + 1, z).IsFluid())
			return;

		for (int i = -5; i <= 5; i++)
		{
			for (int j = -5; j <= 5; j++)
			{
				for (int k = 1; k <= 9; k++)
				{
					Block block = Map.GetBlockSafe(x + i, y + k, z + j);

					if (block.ID == BlockID.Boundary || block.ID == BlockID.TreeTrunk || block.ID == BlockID.Leaves)
						return;
				}
			}
		}

		for (int i = y + 1; i < y + 9; i++)
			Map.SetBlock(x, i, z, treeTrunk);

		for (int j = -3; j <= 3; j++)
		{
			for (int k = -3; k <= 3; k++)
			{
				for (int l = 7; l <= 10; l++)
					Map.SetBlock(x + j, y + l, z + k, leaves);
			}
		}
	}
}
