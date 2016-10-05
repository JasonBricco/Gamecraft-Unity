using UnityEngine;

public static class Direction
{
	public const int None = -1;
	public const int Left = 0;
	public const int Right = 1;
	public const int Back = 2;
	public const int Front = 3;
	public const int Up = 4;
	public const int Down = 5;
	public const int LeftBack = 6;
	public const int RightBack = 7;
	public const int LeftFront = 8;
	public const int RightFront = 9;

	private static string[] names = 
	{ 
		"Left", "Right", "Back", "Front", "Up", "Down",
		"LeftBack", "RightBack", "LeftFront", "RightFront", 
	};

	public static int GetEdge(int x, int y, int z)
	{
		if (x == 0)
		{
			if (z == 0) return Direction.LeftBack;
			if (z == Map.Size - 1) return Direction.LeftFront;

			return Direction.Left;
		}

		if (x == Map.Size - 1)
		{
			if (z == 0) return Direction.RightBack;
			if (z == Map.Size - 1) return Direction.RightFront;

			return Direction.Right;
		}

		if (z == 0) return Direction.Back;
		if (z == Map.Size - 1) return Direction.Front;

		return Direction.None;
	}
	
	public static bool IsOpposite(int a, int b)
	{
		if (a == Left) return b == Right;
		if (a == Right) return b == Left;
		if (a == Front) return b == Back;
		if (a == Back) return b == Front;

		return false;
	}

	public static int GetOpposite(int dir)
	{
		if (dir == Left) return Direction.Right;
		if (dir == Right) return Direction.Left;
		if (dir == Front) return Direction.Back;
		if (dir == Back) return Direction.Front;

		return Direction.None;
	}

	public static string GetString(int dir)
	{
		return names[dir];
	}

	public static string GetStringAsOrientation(int dir)
	{
		if (dir == Direction.Front) return "North";
		if (dir == Direction.Back) return "South";
		if (dir == Direction.Left) return "West";
		if (dir == Direction.Right) return "East";

		return "None";
	}
}
