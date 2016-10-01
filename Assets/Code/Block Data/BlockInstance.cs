using UnityEngine;

public struct BlockInstance : System.IEquatable<BlockInstance>
{
	public ushort ID;
	public int x, y, z;

	public BlockInstance(ushort ID, int x, int y, int z)
	{
		this.ID = ID;
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public bool Equals(BlockInstance other)
	{
		if (ID == other.ID && x == other.x && y == other.y && z == other.z)
			return true;

		return false;
	}
}
