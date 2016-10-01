using UnityEngine;
using System.Collections.Generic;

public class WallGenerator : StructureGenerator 
{
	public override void Generate(HitInfo info)
	{
		List<BlockInstance> blocks = new List<BlockInstance>();

		int x = info.adjPos.x;
		int y = info.adjPos.y;
		int z = info.adjPos.z;

		int orientation = Player.GetRotation();

		switch (orientation)
		{
		case Direction.Left:
			CreateFrontWall(x, y, z, blocks);
			break;

		case Direction.Right:
			CreateBackWall(x, y, z, blocks);
			break;

		case Direction.Front:
			CreateRightWall(x, y, z, blocks);
			break;

		case Direction.Back:
			CreateLeftWall(x, y, z, blocks);
			break;
		}

		Map.SetBlocksAdvanced(blocks, true);
	}

	private void CreateRightWall(int startX, int startY, int startZ, List<BlockInstance> blocks)
	{
		for (int x = startX; x < startX + 10; x++)
		{
			for (int y = startY; y < startY + 5; y++)
				blocks.Add(new BlockInstance(BlockType.Stone, x, y, startZ));
		}
	}

	private void CreateLeftWall(int startX, int startY, int startZ, List<BlockInstance> blocks)
	{
		for (int x = startX - 9; x <= startX; x++)
		{
			for (int y = startY; y < startY + 5; y++)
				blocks.Add(new BlockInstance(BlockType.Stone, x, y, startZ));
		}
	}

	private void CreateFrontWall(int startX, int startY, int startZ, List<BlockInstance> blocks)
	{
		for (int z = startZ; z < startZ + 10; z++)
		{
			for (int y = startY; y < startY + 5; y++)
				blocks.Add(new BlockInstance(BlockType.Stone, startX, y, z));
		}
	}

	private void CreateBackWall(int startX, int startY, int startZ, List<BlockInstance> blocks)
	{
		for (int z = startZ - 9; z <= startZ; z++)
		{
			for (int y = startY; y < startY + 5; y++)
				blocks.Add(new BlockInstance(BlockType.Stone, startX, y, z));
		}
	}
}
