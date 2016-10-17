using UnityEngine;

public struct Vector3i 
{
	public int x, y, z;
	
	public static readonly Vector3i zero = new Vector3i(0, 0, 0);
	public static readonly Vector3i one = new Vector3i(1, 1, 1);
	public static readonly Vector3i front = new Vector3i(0, 0, 1);
	public static readonly Vector3i back = new Vector3i(0, 0, -1);
	public static readonly Vector3i up = new Vector3i(0, 1, 0);
	public static readonly Vector3i down = new Vector3i(0, -1, 0);
	public static readonly Vector3i left = new Vector3i(-1, 0, 0);
	public static readonly Vector3i right = new Vector3i(1, 0, 0);
	
	public static readonly Vector3i[] directions = new Vector3i[] 
	{
		left, right,
		back, front,
		down, up
	};

	public static readonly int[] directionNums = new int[]
	{
		Direction.Left, Direction.Right,
		Direction.Back, Direction.Front,
		Direction.Down, Direction.Up
	};

	public static readonly Vector3i[] diagonals = new Vector3i[]
	{
		left + back, right + back,
		left + front, right + front
	};

	public Vector3i(int x, int y, int z) 
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3i(int x, int y)
	{
		this.x = x;
		this.y = y;
		this.z = 0;
	}

	public Vector3i(Vector3 vector)
	{
		this.x = (int)vector.x;
		this.y = (int)vector.y;
		this.z = (int)vector.z;
	}

	public int DistanceSquared(Vector3i v) 
	{
		return DistanceSquared(this, v);
	}

	public static int DistanceSquared(Vector3i a, Vector3i b) 
	{
		int dx = b.x - a.x;
		int dy = b.y - a.y;
		int dz = b.z - a.z;
		
		return dx * dx + dy * dy + dz * dz;
	}

	public int GetNormalDirection()
	{
		if (x == 1) return Direction.Right;
		if (x == -1) return Direction.Left;
		if (z == 1) return Direction.Front;
		if (z == -1) return Direction.Back;
		if (y == 1) return Direction.Up;
		if (y == -1) return Direction.Down;

		return -1;
	}

	public override int GetHashCode() 
	{
		return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
	}
	
	public override bool Equals(object other) 
	{
		if (!(other is Vector3i)) 
			return false;

		Vector3i vector = (Vector3i)other;

		return x == vector.x && y == vector.y && z == vector.z;
	}
	
	public override string ToString() 
	{
		return "(" + x + ", " + y + ", " + z + ")";
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x, y, z);
	}

	public static Vector3i Min(Vector3i a, Vector3i b) 
	{
		return new Vector3i(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
	}

	public static Vector3i Max(Vector3i a, Vector3i b) 
	{
		return new Vector3i(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
	}
	
	public static bool operator == (Vector3i a, Vector3i b) 
	{
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}
	
	public static bool operator != (Vector3i a, Vector3i b) 
	{
		return a.x != b.x || a.y != b.y || a.z != b.z;
	}
	
	public static Vector3i operator - (Vector3i a, Vector3i b) 
	{
		return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
	}
	
	public static Vector3i operator + (Vector3i a, Vector3i b) 
	{
		return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector3i operator * (Vector3i vector, int scalar)
	{
		return new Vector3i(vector.x * scalar, vector.y * scalar, vector.z * scalar);
	}
	
	public static implicit operator Vector3(Vector3i v) 
	{
		return new Vector3(v.x, v.y, v.z);
	}
}
