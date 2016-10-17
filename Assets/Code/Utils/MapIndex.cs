using UnityEngine;
using System;

public struct MapIndex : IEquatable<MapIndex>
{
	private int value;

	public MapIndex(int x, int y, int z)
	{
		value = x + Map.Size * (y + Map.Height * z);
	}

	public MapIndex(int value)
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
		v.y = (value >> Map.XBits) & (Map.Height - 1);
		v.z = value >> 17;

		return v;
	}

	public bool Equals(MapIndex index)
	{
		return index.value == value;
	}

	public override int GetHashCode()
	{
		return value;
	}

	public static void Verify(int x, int y, int z)
	{
		Debug.Log("Verifying MapIndex...");

		MapIndex index = new MapIndex(x, y, z);
		Vector3 coords = index.GetCoords();

		Debug.Log("Input: " + x + ", " + y + ", " + z);
		Debug.Log("Output: " + coords.x + ", " + coords.y + ", " + coords.z);

		if (x == coords.x && y == coords.y && z == coords.z)
			Debug.Log("They matched!");
		else Debug.Log("No match.");
	}
}
