using UnityEngine;
using System;

public struct BlockInstance : IEquatable<BlockInstance>
{
	public Block block;
	public int x, y, z;

	public BlockInstance(Block block, int x, int y, int z)
	{
		this.block = block;
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public bool Equals(BlockInstance other)
	{
		if (block.ID == other.block.ID && x == other.x && y == other.y && z == other.z)
			return true;

		return false;
	}
}
