using UnityEngine;

public struct Vector2i 
{
	public int x, z;
	
	public static readonly Vector2i zero = new Vector2i(0, 0);
	public static readonly Vector2i one = new Vector2i(1, 1);
	public static readonly Vector2i up = new Vector2i(0, 1);
	public static readonly Vector2i down = new Vector2i(0, -1);
	public static readonly Vector2i left = new Vector2i(-1, 0);
	public static readonly Vector2i right = new Vector2i(1, 0);
	
	public static readonly Vector2i[] directions = new Vector2i[] 
	{
		left, right,
		down, up
	};

	public static readonly int[] directionNums = new int[]
	{
		Direction.Left, Direction.Right,
		Direction.Down, Direction.Up
	};

	public Vector2i(int x, int y) 
	{
		this.x = x;
		this.z = y;
	}

	public Vector2i(Vector2 vector)
	{
		this.x = (int)vector.x;
		this.z = (int)vector.y;
	}

	public int DistanceSquared(Vector2i v) 
	{
		return DistanceSquared(this, v);
	}

	public static int DistanceSquared(Vector2i a, Vector2i b) 
	{
		int dx = b.x - a.x;
		int dz = b.z - a.z;
		
		return dx * dx + dz * dz;
	}

	public int GetNormalDirection()
	{
		if (x == 1) return Direction.Right;
		if (x == -1) return Direction.Left;
		if (z == 1) return Direction.Up;
		if (z == -1) return Direction.Down;

		return -1;
	}

	public override int GetHashCode() 
	{
		return x.GetHashCode() ^ z.GetHashCode() << 2;
	}
	
	public override bool Equals(object other) 
	{
		if (!(other is Vector2i)) 
			return false;

		Vector2i vector = (Vector2i)other;

		return x == vector.x && z == vector.z;
	}

	public override string ToString() 
	{
		return "(" + x + ", " + z + ")";
	}

	public Vector2 ToVector2()
	{
		return new Vector3(x, z);
	}

	public static Vector2i Min(Vector2i a, Vector2i b) 
	{
		return new Vector2i(Mathf.Min(a.x, b.x), Mathf.Min(a.z, b.z));
	}

	public static Vector2i Max(Vector2i a, Vector2i b) 
	{
		return new Vector2i(Mathf.Max(a.x, b.x), Mathf.Max(a.z, b.z));
	}
	
	public static bool operator == (Vector2i a, Vector2i b) 
	{
		return a.x == b.x && a.z == b.z;
	}
	
	public static bool operator != (Vector2i a, Vector2i b) 
	{
		return a.x != b.x || a.z != b.z;
	}
	
	public static Vector2i operator - (Vector2i a, Vector2i b) 
	{
		return new Vector2i(a.x - b.x, a.z - b.z);
	}
	
	public static Vector2i operator + (Vector2i a, Vector2i b) 
	{
		return new Vector2i(a.x + b.x, a.z + b.z);
	}

	public static Vector2i operator * (Vector2i vector, int scalar)
	{
		return new Vector2i(vector.x * scalar, vector.z * scalar);
	}
	
	public static implicit operator Vector2(Vector2i v) 
	{
		return new Vector2i(v.x, v.z);
	}
}
