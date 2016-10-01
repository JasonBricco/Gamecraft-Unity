using UnityEngine;
using System;

public class Index : IEquatable<Index>
{
	private int value = 0;

	public Index(int x, int y, int z)
	{
		value = x + Map.Size * (y + Map.Height * z);
	}

	public Index(int value)
	{
		this.value = value;
	}

	public int Value
	{
		get { return value; }
	}

	public Vector3i GetCoords()
	{
		Vector3i v = new Vector3i();

		v.x = value & (Map.Size - 1);
		v.y = (value >> 9) & (Map.Size - 1);
		v.z = value >> 16;

		return v;
	}

	public bool Equals(Index index)
	{
		return index.value == value;
	}

	public override int GetHashCode()
	{
		return value;
	}
}
