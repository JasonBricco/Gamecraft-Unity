using UnityEngine;
using System.Collections.Generic;

public class TreeGenerator : StructureGenerator 
{
	public override void Generate(HitInfo info)
	{
		int x = info.adjPos.x;
		int y = info.adjPos.y;
		int z = info.adjPos.z;

		List<BlockInstance> blocks = new List<BlockInstance>();
		
		for (int i = y; i < y + 7; i++)
			blocks.Add(new BlockInstance(BlockType.TreeTrunk, x, i, z));
		
		for (int j = -3; j <= 3; j++)
		{
			for (int k = -3; k <= 3; k++)
			{
				for (int l = 7; l <= 10; l++)
					blocks.Add(new BlockInstance(BlockType.Leaves, x + j, y + l, z + k));
			}
		}
		
		Map.SetBlocksAdvanced(blocks, true);
	}

	public static void BoxyTreeTerrain(int x, int y, int z)
	{
		if (Map.GetBlockSafe(x, y, z) != BlockType.Grass || BlockRegistry.GetBlock(Map.GetBlockSafe(x, y + 1, z)).IsFluid)
			return;

		for (int i = -5; i <= 5; i++)
		{
			for (int j = -5; j <= 5; j++)
			{
				for (int k = 1; k <= 9; k++)
				{
					ushort block = Map.GetBlockSafe(x + i, y + k, z + j);

					if (block == BlockType.Boundary || block == BlockType.TreeTrunk || block == BlockType.Leaves)
						return;
				}
			}
		}

		for (int i = y + 1; i < y + 9; i++)
			Map.SetBlock(x, i, z, BlockType.TreeTrunk);

		for (int j = -3; j <= 3; j++)
		{
			for (int k = -3; k <= 3; k++)
			{
				for (int l = 7; l <= 10; l++)
					Map.SetBlock(x + j, y + l, z + k, BlockType.Leaves);
			}
		}
	}
}
