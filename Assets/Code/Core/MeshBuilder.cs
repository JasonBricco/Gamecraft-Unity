﻿using UnityEngine;
using System.Collections.Generic;

public enum Axis { X, Y, Z }

public sealed class MeshBuilder
{
	public static readonly MeshBuilder instance = new MeshBuilder();

	private readonly Vector3[] squareFront = 
	{
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f)
	};

	private readonly Vector3[] squareBack =
	{
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, -0.5f)
	};

	private readonly Vector3[] squareLeft =
	{
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f)
	};

	private readonly Vector3[] squareRight =
	{
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f)
	};

	private readonly Vector3[] squareTop =
	{
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f)
	};

	private readonly Vector3[] squareBottom = 
	{
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, -0.5f)
	};

	private readonly Vector3[] ladderFront =
	{
		new Vector3(0.5f, -0.5f, -0.49f),
		new Vector3(0.5f, 0.5f, -0.49f),
		new Vector3(-0.5f, 0.5f, -0.49f),
		new Vector3(-0.5f, -0.5f, -0.49f)
	};

	private readonly Vector3[] ladderBack = 
	{
		new Vector3(-0.5f, -0.5f, 0.49f),
		new Vector3(-0.5f, 0.5f, 0.49f),
		new Vector3(0.5f, 0.5f, 0.49f),
		new Vector3(0.5f, -0.5f, 0.49f)
	};

	private readonly Vector3[] ladderRight =
	{
		new Vector3(-0.49f, -0.5f, -0.5f),
		new Vector3(-0.49f, 0.5f, -0.5f),
		new Vector3(-0.49f, 0.5f, 0.5f),
		new Vector3(-0.49f, -0.5f, 0.5f)
	};

	private readonly Vector3[] ladderLeft = 
	{
		new Vector3(0.49f, -0.5f, 0.5f),
		new Vector3(0.49f, 0.5f, 0.5f),
		new Vector3(0.49f, 0.5f, -0.5f),
		new Vector3(0.49f, -0.5f, -0.5f)
	};

	private readonly Vector3[] squareCutoutFront =
	{
		new Vector3(-0.5f, -0.5f, 0.25f),
		new Vector3(-0.5f, 0.5f, 0.25f),
		new Vector3(0.5f, 0.5f, 0.25f),
		new Vector3(0.5f, -0.5f, 0.25f)
	};

	private readonly Vector3[] squareCutoutBack =
	{
		new Vector3(0.5f, -0.5f, -0.25f),
		new Vector3(0.5f, 0.5f, -0.25f),
		new Vector3(-0.5f, 0.5f, -0.25f),
		new Vector3(-0.5f, -0.5f, -0.25f)
	};

	private readonly Vector3[] squareCutoutRight =
	{
		new Vector3(0.25f, -0.5f, 0.5f),
		new Vector3(0.25f, 0.5f, 0.5f),
		new Vector3(0.25f, 0.5f, -0.5f),
		new Vector3(0.25f, -0.5f, -0.5f)
	};

	private readonly Vector3[] squareCutoutLeft =
	{
		new Vector3(-0.25f, -0.5f, -0.5f),
		new Vector3(-0.25f, 0.5f, -0.5f),
		new Vector3(-0.25f, 0.5f, 0.5f),
		new Vector3(-0.25f, -0.5f, 0.5f)
	};

	private readonly Vector3[] frontSlopeTop =
	{
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f)
	};

	private readonly Vector3[] frontSlopeLeft =
	{
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f)
	};

	public readonly Vector3[] frontSlopeRight =
	{
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f)
	};

	private readonly Vector3[] backSlopeTop = 
	{
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, -0.5f)
	};

	private readonly Vector3[] backSlopeLeft =
	{
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f)
	};

	private readonly Vector3[] backSlopeRight =
	{
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0.5f)
	};

	private readonly Vector3[] rightSlopeTop = 
	{
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f)
	};

	private readonly Vector3[] rightSlopeFront =
	{
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f)
	};

	private readonly Vector3[] rightSlopeBack =
	{
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, -0.5f)
	};

	private readonly Vector3[] leftSlopeTop =
	{
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, -0.5f)
	};

	private readonly Vector3[] leftSlopeFront =
	{
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f)
	};

	private readonly Vector3[] leftSlopeBack =
	{
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f)
	};

	private readonly int[] squareOffsetFront = { -1, -1, 1, -1, 1, 1, 1, 1, 1, 1, -1, 1 };
	private readonly int[] squareOffsetBack = { 1, -1, -1, 1, 1, -1, -1, 1, -1, -1, -1, -1 };
	private readonly int[] squareOffsetRight = { 1, -1, 1, 1, 1, 1, 1, 1, -1, 1, -1, -1 };
	private readonly int[] squareOffsetLeft = { -1, -1, -1, -1, 1, -1, -1, 1, 1, -1, -1, 1 };
	private readonly int[] squareOffsetTop = { 1, 1, -1, 1, 1, 1, -1, 1, 1, -1, 1, -1 };
	private readonly int[] squareOffsetBottom = { -1, -1, -1, -1, -1, 1, 1, -1, 1, 1, -1, -1 };

	private readonly int[] ladderOffsetFront = { -1, -1, 0, -1, 1, 0, 1, 1, 0, 1, -1, 0 };
	private readonly int[] ladderOffsetBack = { 1, -1, 0, 1, 1, 0, -1, 1, 0, -1, -1, 0 };
	private readonly int[] ladderOffsetRight = { 0, -1, 1, 0, 1, 1, 0, 1, -1, 0, -1, -1 };
	private readonly int[] ladderOffsetLeft = { 0, -1, -1, 0, 1, -1, 0, 1, 1, 0, -1, 1 };

	private readonly int[] frontSlopeOffsetLeft = { -1, -1, -1, -1, 1, -1, -1, -1, 1 };
	private readonly int[] frontSlopeOffsetRight = { 1, -1, 1, 1, 1, -1, 1, -1, -1 };
	private readonly int[] frontSlopeOffsetTop = { 1, 1, -1, 1, -1, 1, -1, -1, 1, -1, 1, -1 };

	private readonly int[] backSlopeOffsetLeft = { -1, -1, -1, -1, 1, 1, -1, -1, 1 };
	private readonly int[] backSlopeOffsetRight = { 1, 1, 1, 1, -1, -1, 1, -1, 1 };
	private readonly int[] backSlopeOffsetTop = { 1, -1, -1, 1, 1, 1, -1, 1, 1, -1, -1, -1 };

	private readonly int[] rightSlopeOffsetFront = { -1, -1, 1, -1, 1, 1, 1, -1, 1 };
	private readonly int[] rightSlopeOffsetBack = { 1, -1, -1, -1, 1, -1, -1, -1, -1 };
	private readonly int[] rightSlopeOffsetTop = { 1, -1, -1, 1, -1, 1, -1, 1, 1, -1, 1, -1 };

	private readonly int[] leftSlopeOffsetFront = { -1, -1, 1, 1, 1, 1, 1, -1, 1 };
	private readonly int[] leftSlopeOffsetBack = { 1, 1, -1, -1, -1, -1, 1, -1, -1 };
	private readonly int[] leftSlopeOffsetTop = { 1, 1, -1, 1, 1, 1, -1, -1, 1, -1, -1, -1 };

	public void BuildCube(Block block, int x, int y, int z, MeshData meshData)
	{
		int index = block.MeshIndex();

		int localX = x & (Chunk.Size - 1);
		int localZ = z & (Chunk.Size - 1);

		Vector3i worldPos = new Vector3i(x, y, z);

		if (IsFaceVisible(block, x, y, z + 1, Direction.Back)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareFront, Direction.Front);
			BuildSquareLight(Axis.Z, index, worldPos, meshData, squareOffsetFront);
		}

		if (IsFaceVisible(block, x, y, z - 1, Direction.Front)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareBack, Direction.Back);
			BuildSquareLight(Axis.Z, index, worldPos, meshData, squareOffsetBack);
		}

		if (IsFaceVisible(block, x + 1, y, z, Direction.Left)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareRight, Direction.Right);
			BuildSquareLight(Axis.X, index, worldPos, meshData, squareOffsetRight);
		}

		if (IsFaceVisible(block, x - 1, y, z, Direction.Right)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareLeft, Direction.Left);
			BuildSquareLight(Axis.X, index, worldPos, meshData, squareOffsetLeft);
		}

		if (IsFaceVisible(block, x, y + 1, z, Direction.Down)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareTop, Direction.Up);
			BuildSquareLight(Axis.Y, index, worldPos, meshData, squareOffsetTop);
		}

		if (IsFaceVisible(block, x, y - 1, z, Direction.Up)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareBottom, Direction.Down);
			BuildSquareLight(Axis.Y, index, worldPos, meshData, squareOffsetBottom);
		}
	}

	public void BuildLadder(Block block, int x, int y, int z, MeshData meshData)
	{
		int direction = block.BlockDirection;
		int index = block.MeshIndex();

		int localX = x & (Chunk.Size - 1);
		int localZ = z & (Chunk.Size - 1);

		Vector3i worldPos = new Vector3i(x, y, z);

		switch (direction)
		{
		case Direction.Left:
			BuildSquareFace(index, block, localX, y, localZ, meshData, ladderLeft, Direction.Left);
			BuildSquareLight(Axis.X, index, worldPos, meshData, ladderOffsetLeft);
			break;

		case Direction.Right:
			BuildSquareFace(index, block, localX, y, localZ, meshData, ladderRight, Direction.Right);
			BuildSquareLight(Axis.X, index, worldPos, meshData, ladderOffsetRight);
			break;

		case Direction.Front:
			BuildSquareFace(index, block, localX, y, localZ, meshData, ladderFront, Direction.Front);
			BuildSquareLight(Axis.Z, index, worldPos, meshData, ladderOffsetFront);
			break;

		case Direction.Back:
			BuildSquareFace(index, block, localX, y, localZ, meshData, ladderBack, Direction.Back);
			BuildSquareLight(Axis.Z, index, worldPos, meshData, ladderOffsetBack);
			break;
		}
	}

	public void BuildFluid(Block block, int x, int y, int z, MeshData meshData)
	{
		int index = block.MeshIndex();

		int localX = x & (Chunk.Size - 1);
		int localZ = z & (Chunk.Size - 1);

		Vector3i worldPos = new Vector3i(x, y, z);

		AdjacentBlocks neighbors = Map.GetAdjacentBlocks(x, y, z);

		int curLevel = block.FluidLevel;
		float offset = FluidSimulator.GetOffset(curLevel);

		if (IsFluidFaceVisible(curLevel, x, y, z + 1, neighbors.front, Direction.Back)) 
		{
			BuildFrontFluid(index, block, localX, y, localZ, meshData, Direction.Front, offset, neighbors.front);
			BuildSquareLight(Axis.Z, index, worldPos, meshData, squareOffsetFront);
		}

		if (IsFluidFaceVisible(curLevel, x, y, z - 1, neighbors.back, Direction.Front)) 
		{
			BuildBackFluid(index, block, localX, y, localZ, meshData, Direction.Back, offset, neighbors.back);
			BuildSquareLight(Axis.Z, index, worldPos, meshData, squareOffsetBack);
		}

		if (IsFluidFaceVisible(curLevel, x + 1, y, z, neighbors.right, Direction.Left)) 
		{
			BuildRightFluid(index, block, localX, y, localZ, meshData, Direction.Right, offset, neighbors.right);
			BuildSquareLight(Axis.X, index, worldPos, meshData, squareOffsetRight);
		}

		if (IsFluidFaceVisible(curLevel, x - 1, y, z, neighbors.left, Direction.Right)) 
		{
			BuildLeftFluid(index, block, localX, y, localZ, meshData, Direction.Left, offset, neighbors.left);
			BuildSquareLight(Axis.X, index, worldPos, meshData, squareOffsetLeft);
		}

		if (IsFluidFaceVisible(curLevel, x, y + 1, z, neighbors.top, Direction.Down)) 
		{
			BuildTopFluid(index, block, localX, y, localZ, meshData, Direction.Up, offset);
			BuildSquareLight(Axis.Y, index, worldPos, meshData, squareOffsetTop);
		}

		if (IsFluidFaceVisible(curLevel, x, y - 1, z, neighbors.bottom, Direction.Up)) 
		{
			BuildBottomFluid(index, block, localX, y, localZ, meshData, Direction.Down);
			BuildSquareLight(Axis.Y, index, worldPos, meshData, squareOffsetBottom);
		}
	}

	public void BuildSquareCutout(Block block, int x, int y, int z, MeshData meshData)
	{
		int index = block.MeshIndex();

		int localX = x & (Chunk.Size - 1);
		int localZ = z & (Chunk.Size - 1);

		Vector3i worldPos = new Vector3i(x, y, z);

		BuildSquareFace(index, block, localX, y, localZ, meshData, squareCutoutFront, Direction.Front);
		BuildSquareLight(Axis.Z, index, worldPos, meshData, squareOffsetFront);

		BuildSquareFace(index, block, localX, y, localZ, meshData, squareCutoutBack, Direction.Back);
		BuildSquareLight(Axis.Z, index, worldPos, meshData, squareOffsetBack);

		BuildSquareFace(index, block, localX, y, localZ, meshData, squareCutoutRight, Direction.Right);
		BuildSquareLight(Axis.X, index, worldPos, meshData, squareOffsetRight);

		BuildSquareFace(index, block, localX, y, localZ, meshData, squareCutoutLeft, Direction.Left);
		BuildSquareLight(Axis.X, index, worldPos, meshData, squareOffsetLeft);
	}

	public void BuildFrontSlope(Block block, int x, int y, int z, MeshData meshData)
	{
		int index = block.MeshIndex();

		int localX = x & (Chunk.Size - 1);
		int localZ = z & (Chunk.Size - 1);

		Vector3i worldPos = new Vector3i(x, y, z);

		if (IsFaceVisible(block, x, y, z - 1, Direction.Front)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareBack, Direction.Back);
			BuildSquareLight(Axis.Z, index, worldPos, meshData, squareOffsetBack);
		}

		if (IsFaceVisible(block, x + 1, y, z, Direction.Left)) 
		{
			BuildTriangleFace(index, localX, y, localZ, meshData, frontSlopeRight, Direction.Right);
			AddFrontSlopeUVs(block, index, Direction.Right, meshData);
			BuildTriangleLight(Axis.X, index, worldPos, meshData, frontSlopeOffsetRight);
		}

		if (IsFaceVisible(block, x - 1, y, z, Direction.Right)) 
		{
			BuildTriangleFace(index, localX, y, localZ, meshData, frontSlopeLeft, Direction.Left);
			AddFrontSlopeUVs(block, index, Direction.Left, meshData);
			BuildTriangleLight(Axis.X, index, worldPos, meshData, frontSlopeOffsetLeft);
		}

		BuildSquareFace(index, block, localX, y, localZ, meshData, frontSlopeTop, Direction.Up);
		BuildSlopeLight(Axis.Z, index, worldPos, meshData, frontSlopeOffsetTop);

		if (IsFaceVisible(block, x, y - 1, z, Direction.Up)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareBottom, Direction.Down);
			BuildSquareLight(Axis.Y, index, worldPos, meshData, squareOffsetBottom);
		}
	}

	public void BuildBackSlope(Block block, int x, int y, int z, MeshData meshData)
	{
		int index = block.MeshIndex();

		int localX = x & (Chunk.Size - 1);
		int localZ = z & (Chunk.Size - 1);

		Vector3i worldPos = new Vector3i(x, y, z);

		if (IsFaceVisible(block, x, y, z + 1, Direction.Back)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareFront, Direction.Front);
			BuildSquareLight(Axis.Z, index, worldPos, meshData, squareOffsetFront);
		}

		if (IsFaceVisible(block, x + 1, y, z, Direction.Left)) 
		{
			BuildTriangleFace(index, localX, y, localZ, meshData, backSlopeRight, Direction.Right);
			AddBackSlopeUVs(block, index, Direction.Right, meshData);
			BuildTriangleLight(Axis.X, index, worldPos, meshData, backSlopeOffsetRight);
		}

		if (IsFaceVisible(block, x - 1, y, z, Direction.Right)) 
		{
			BuildTriangleFace(index, localX, y, localZ, meshData, backSlopeLeft, Direction.Left);
			AddBackSlopeUVs(block, index, Direction.Left, meshData);
			BuildTriangleLight(Axis.X, index, worldPos, meshData, backSlopeOffsetLeft);
		}

		BuildSquareFace(index, block, localX, y, localZ, meshData, backSlopeTop, Direction.Up);
		BuildSlopeLight(Axis.Z, index, worldPos, meshData, backSlopeOffsetTop);

		if (IsFaceVisible(block, x, y - 1, z, Direction.Up)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareBottom, Direction.Down);
			BuildSquareLight(Axis.Y, index, worldPos, meshData, squareOffsetBottom);
		}
	}

	public void BuildRightSlope(Block block, int x, int y, int z, MeshData meshData)
	{
		int index = block.MeshIndex();

		int localX = x & (Chunk.Size - 1);
		int localZ = z & (Chunk.Size - 1);

		Vector3i worldPos = new Vector3i(x, y, z);

		if (IsFaceVisible(block, x, y, z + 1, Direction.Back)) 
		{
			BuildTriangleFace(index, localX, y, localZ, meshData, rightSlopeFront, Direction.Front);
			AddRightSlopeUVs(block, index, Direction.Front, meshData);
			BuildTriangleLight(Axis.Z, index, worldPos, meshData, rightSlopeOffsetFront);
		}

		if (IsFaceVisible(block, x, y, z - 1, Direction.Front)) 
		{
			BuildTriangleFace(index, localX, y, localZ, meshData, rightSlopeBack, Direction.Back);
			AddRightSlopeUVs(block, index, Direction.Back, meshData);
			BuildTriangleLight(Axis.Z, index, worldPos, meshData, rightSlopeOffsetBack);
		}

		if (IsFaceVisible(block, x - 1, y, z, Direction.Right)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareLeft, Direction.Left);
			BuildSquareLight(Axis.X, index, worldPos, meshData, squareOffsetLeft);
		}

		BuildSquareFace(index, block, localX, y, localZ, meshData, rightSlopeTop, Direction.Up);
		BuildSlopeLight(Axis.X, index, worldPos, meshData, rightSlopeOffsetTop);

		if (IsFaceVisible(block, x, y - 1, z, Direction.Up)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareBottom, Direction.Down);
			BuildSquareLight(Axis.Y, index, worldPos, meshData, squareOffsetBottom);
		}
	}

	public void BuildLeftSlope(Block block, int x, int y, int z, MeshData meshData)
	{
		int index = block.MeshIndex();

		int localX = x & (Chunk.Size - 1);
		int localZ = z & (Chunk.Size - 1);

		Vector3i worldPos = new Vector3i(x, y, z);

		if (IsFaceVisible(block, x, y, z + 1, Direction.Back)) 
		{
			BuildTriangleFace(index, localX, y, localZ, meshData, leftSlopeFront, Direction.Front);
			AddLeftSlopeUVs(block, index, Direction.Front, meshData);
			BuildTriangleLight(Axis.Z, index, worldPos, meshData, leftSlopeOffsetFront);
		}

		if (IsFaceVisible(block, x, y, z - 1, Direction.Front)) 
		{
			BuildTriangleFace(index, localX, y, localZ, meshData, leftSlopeBack, Direction.Back);
			AddLeftSlopeUVs(block, index, Direction.Back, meshData);
			BuildTriangleLight(Axis.Z, index, worldPos, meshData, leftSlopeOffsetBack);
		}

		if (IsFaceVisible(block, x - 1, y, z, Direction.Left)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareRight, Direction.Right);
			BuildSquareLight(Axis.X, index, worldPos, meshData, squareOffsetRight);
		}

		BuildSquareFace(index, block, localX, y, localZ, meshData, leftSlopeTop, Direction.Up);
		BuildSlopeLight(Axis.X, index, worldPos, meshData, leftSlopeOffsetTop);

		if (IsFaceVisible(block, x, y - 1, z, Direction.Up)) 
		{
			BuildSquareFace(index, block, localX, y, localZ, meshData, squareBottom, Direction.Down);
			BuildSquareLight(Axis.Y, index, worldPos, meshData, squareOffsetBottom);
		}
	}

	public Mesh BuildFrontSlopeCollider(CollisionMeshData data)
	{
		BuildSquareCollider(data, squareBack);
		BuildTriangleCollider(data, frontSlopeRight);
		BuildTriangleCollider(data, frontSlopeLeft);
		BuildSquareCollider(data, frontSlopeTop);
		BuildSquareCollider(data, squareBottom);

		return data.GetMesh();
	}

	public Mesh BuildBackSlopeCollider(CollisionMeshData data)
	{
		BuildSquareCollider(data, squareFront);
		BuildTriangleCollider(data, backSlopeRight);
		BuildTriangleCollider(data, backSlopeLeft);
		BuildSquareCollider(data, backSlopeTop);
		BuildSquareCollider(data, squareBottom);

		return data.GetMesh();
	}

	public Mesh BuildRightSlopeCollider(CollisionMeshData data)
	{
		BuildTriangleCollider(data, rightSlopeFront);
		BuildTriangleCollider(data, rightSlopeBack);
		BuildSquareCollider(data, squareLeft);
		BuildSquareCollider(data, rightSlopeTop);
		BuildSquareCollider(data, squareBottom);

		return data.GetMesh();
	}

	public Mesh BuildLeftSlopeCollider(CollisionMeshData data)
	{
		BuildTriangleCollider(data, leftSlopeFront);
		BuildTriangleCollider(data, leftSlopeBack);
		BuildSquareCollider(data, squareRight);
		BuildSquareCollider(data, leftSlopeTop);
		BuildSquareCollider(data, squareBottom);

		return data.GetMesh();
	}

	private void BuildSquareCollider(CollisionMeshData data, Vector3[] vertices)
	{
		AddSquareColliderIndices(data);

		for (int i = 0; i < vertices.Length; i++)
			data.AddVertex(vertices[i]);
	}

	private void BuildTriangleCollider(CollisionMeshData data, Vector3[] vertices)
	{
		AddTriangleColliderIndices(data);

		for (int i = 0; i < vertices.Length; i++)
			data.AddVertex(vertices[i]);
	}

	private void AddSquareColliderIndices(CollisionMeshData data)
	{
		int offset = data.GetOffset();
		List<int> triangles = data.GetTriangles();

		triangles.Add(offset + 2);
		triangles.Add(offset + 1);
		triangles.Add(offset + 0);

		triangles.Add(offset + 3);
		triangles.Add(offset + 2);
		triangles.Add(offset + 0);
	}

	private void AddTriangleColliderIndices(CollisionMeshData data)
	{
		int offset = data.GetOffset();
		List<int> triangles = data.GetTriangles();

		triangles.Add(offset + 2);
		triangles.Add(offset + 1);
		triangles.Add(offset + 0);
	}

	private bool IsFaceVisible(Block block, int neighborX, int neighborY, int neighborZ, int neighborFace) 
	{
		Block neighbor = Map.GetBlockSafe(neighborX, neighborY, neighborZ);
		return block.IsFaceVisible(neighbor, neighborFace);
	}

	private bool IsFluidFaceVisible(int curLevel, int nX, int nY, int nZ, Block neighbor, int neighborFace) 
	{
		if (neighborFace == Direction.Down && curLevel < 5)
			return true;
		
		CullType cull = neighbor.GetCullType(neighborFace);

		if (cull == CullType.Solid || cull == CullType.Cutout)
			return false;

		if (cull == CullType.Transparent)
		{
			if (!neighbor.IsFluid())
				return false;

			if (neighborFace == Direction.Up || neighborFace == Direction.Down)
				return false;

			return neighbor.FluidLevel < curLevel;
		}

		return true;
	}

	private void BuildSquareFace(int index, Block block, int x, int y, int z, MeshData meshData, Vector3[] vertices, int dir)
	{
		AddIndices(index, meshData);

		for (int i = 0; i < vertices.Length; i++)
			meshData.AddVertex(index, vertices[i], x, y, z);

		AddUVs(block, index, dir, meshData);
	}

	private void BuildTriangleFace(int index, int x, int y, int z, MeshData meshData, Vector3[] vertices, int dir)
	{
		AddSlopeIndices(index, meshData);

		for (int i = 0; i < vertices.Length; i++)
			meshData.AddVertex(index, vertices[i], x, y, z);
	}

	private void AddIndices(int index, MeshData data)
	{
		List<int> triangles = data.GetTriangles(index);
		int offset = data.GetOffset(index);

		triangles.Add(offset + 2);
		triangles.Add(offset + 1);
		triangles.Add(offset + 0);

		triangles.Add(offset + 3);
		triangles.Add(offset + 2);
		triangles.Add(offset + 0);
	}

	private void AddSlopeIndices(int index, MeshData data)
	{
		List<int> triangles = data.GetTriangles(index);

		int offset = data.GetOffset(index);

		triangles.Add(offset + 2);
		triangles.Add(offset + 1);
		triangles.Add(offset + 0);
	}

	private void BuildFrontFluid(int index, Block block, int x, int y, int z, MeshData meshData, int dir, float offset, Block adj)
	{
		AddIndices(index, meshData);

		float adjOffset = -0.5f;

		if (adj.IsFluid())
		{
			int adjLevel = adj.FluidLevel;
			adjOffset = FluidSimulator.GetOffset(adjLevel);
		}

		meshData.AddVertex(index, new Vector3(-0.5f, adjOffset, 0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(-0.5f, offset, 0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(0.5f, offset, 0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(0.5f, adjOffset, 0.5f), x, y, z);

		AddFluidUVs(block, index, dir, meshData, offset - adjOffset);
	}

	private void BuildBackFluid(int index, Block block, int x, int y, int z, MeshData meshData, int dir, float offset, Block adj)
	{
		AddIndices(index, meshData);

		float adjOffset = -0.5f;

		if (adj.IsFluid())
		{
			int adjLevel = adj.FluidLevel;
			adjOffset = FluidSimulator.GetOffset(adjLevel);
		}

		meshData.AddVertex(index, new Vector3(0.5f, adjOffset, -0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(0.5f, offset, -0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(-0.5f, offset, -0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(-0.5f, adjOffset, -0.5f), x, y, z);

		AddFluidUVs(block, index, dir, meshData, offset - adjOffset);
	}

	private void BuildRightFluid(int index, Block block, int x, int y, int z, MeshData meshData, int dir, float offset, Block adj)
	{
		AddIndices(index, meshData);

		float adjOffset = -0.5f;

		if (adj.IsFluid())
		{
			int adjLevel = adj.FluidLevel;
			adjOffset = FluidSimulator.GetOffset(adjLevel);
		}

		meshData.AddVertex(index, new Vector3(0.5f, adjOffset, 0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(0.5f, offset, 0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(0.5f, offset, -0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(0.5f, adjOffset, -0.5f), x, y, z);

		AddFluidUVs(block, index, dir, meshData, offset - adjOffset);
	}

	private void BuildLeftFluid(int index, Block block, int x, int y, int z, MeshData meshData, int dir, float offset, Block adj)
	{
		AddIndices(index, meshData);

		float adjOffset = -0.5f;

		if (adj.IsFluid())
		{
			int adjLevel = adj.FluidLevel;
			adjOffset = FluidSimulator.GetOffset(adjLevel);
		}

		meshData.AddVertex(index, new Vector3(-0.5f, adjOffset, -0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(-0.5f, offset, -0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(-0.5f, offset, 0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(-0.5f, adjOffset, 0.5f), x, y, z);

		AddFluidUVs(block, index, dir, meshData, offset - adjOffset);
	}

	private void BuildTopFluid(int index, Block block, int x, int y, int z, MeshData meshData, int dir, float offset)
	{
		AddIndices(index, meshData);

		meshData.AddVertex(index, new Vector3(0.5f, offset, -0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(0.5f, offset, 0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(-0.5f, offset, 0.5f), x, y, z);
		meshData.AddVertex(index, new Vector3(-0.5f, offset, -0.5f), x, y, z);

		AddFluidUVs(block, index, dir, meshData);
	}

	private void BuildBottomFluid(int index, Block block, int x, int y, int z, MeshData meshData, int dir)
	{
		AddIndices(index, meshData);

		for (int i = 0; i < squareBottom.Length; i++)
			meshData.AddVertex(index, squareBottom[i], x, y, z);

		AddFluidUVs(block, index, dir, meshData);
	}

	private void AddUVs(Block block, int index, int face, MeshData data)
	{
		float element = block.GetTexture(face);

		data.AddUV(index, new Vector3(1, 0, element));
		data.AddUV(index, new Vector3(1, 1, element));
		data.AddUV(index, new Vector3(0, 1, element));
		data.AddUV(index, new Vector3(0, 0, element));
	}

	private void AddFluidUVs(Block block, int index, int face, MeshData data, float diff = 0.0f)
	{
		if (face == Direction.Up || face == Direction.Down)
		{
			data.AddUV(index, new Vector2(1.0f, 0.0f));
			data.AddUV(index, new Vector2(1.0f, 1.0f));
			data.AddUV(index, new Vector2(0.0f, 1.0f));
			data.AddUV(index, new Vector2(0.0f, 0.0f));
		}
		else
		{
			data.AddUV(index, new Vector2(1.0f, 0.0f));
			data.AddUV(index, new Vector2(1.0f, diff));
			data.AddUV(index, new Vector2(0.0f, diff));
			data.AddUV(index, new Vector2(0.0f, 0.0f));
		}
	}

	private Vector3 BottomRightSlopeUV(float element)
	{
		return new Vector3(1.0f, 0.0f, element);
	}

	private Vector3 TopRightSlopeUV(float element)
	{
		return new Vector3(1.0f, 1.0f, element);
	}

	private Vector3 TopLeftSlopeUV(float element)
	{
		return new Vector3(0.0f, 1.0f, element);
	}

	private Vector3 BottomLeftSlopeUV(float element)
	{
		return new Vector3(0.0f, 0.0f, element);
	}
		
	private void AddFrontSlopeUVs(Block block, int index, int face, MeshData data)
	{
		float element = block.GetTexture(face);

		if (face == Direction.Left)
			data.SetUVs(index, BottomRightSlopeUV(element), TopRightSlopeUV(element), BottomLeftSlopeUV(element), Vector3.zero, true);
		else if (face == Direction.Right)
			data.SetUVs(index, BottomLeftSlopeUV(element), TopRightSlopeUV(element), BottomRightSlopeUV(element), Vector3.zero, true);
	}

	private void AddBackSlopeUVs(Block block, int index, int face, MeshData data)
	{
		float element = block.GetTexture(face);

		if (face == Direction.Left)
			data.SetUVs(index, BottomRightSlopeUV(element), TopLeftSlopeUV(element), BottomLeftSlopeUV(element), Vector3.zero, true);
		else if (face == Direction.Right)
			data.SetUVs(index, TopRightSlopeUV(element), BottomLeftSlopeUV(element), BottomRightSlopeUV(element), Vector3.zero, true);
	}

	private void AddRightSlopeUVs(Block block, int index, int face, MeshData data)
	{
		float element = block.GetTexture(face);

		if (face == Direction.Front)
			data.SetUVs(index, BottomLeftSlopeUV(element), TopLeftSlopeUV(element), BottomRightSlopeUV(element), Vector3.zero, true);
		else if (face == Direction.Back)
			data.SetUVs(index, BottomLeftSlopeUV(element), TopRightSlopeUV(element), BottomRightSlopeUV(element), Vector3.zero, true);
	}

	private void AddLeftSlopeUVs(Block block, int index, int face, MeshData data)
	{
		float element = block.GetTexture(face);

		if (face == Direction.Front)
			data.SetUVs(index, BottomRightSlopeUV(element), TopLeftSlopeUV(element), BottomLeftSlopeUV(element), Vector2.zero, true);
		else if (face == Direction.Back)
			data.SetUVs(index, TopRightSlopeUV(element), BottomLeftSlopeUV(element), BottomRightSlopeUV(element), Vector2.zero, true);
	}

	private void BuildSquareLight(Axis axis, int index, Vector3i pos, MeshData data, int[] offset)
	{
		Color32 a = GetVertexLight(axis, pos, offset[0], offset[1], offset[2]);
		Color32 b = GetVertexLight(axis, pos, offset[3], offset[4], offset[5]);
		Color32 c = GetVertexLight(axis, pos, offset[6], offset[7], offset[8]);
		Color32 d = GetVertexLight(axis, pos, offset[9], offset[10], offset[11]);

		data.AddColors(index, a, b, c, d);
	}

	private void BuildTriangleLight(Axis axis, int index, Vector3i pos, MeshData data, int[] offset)
	{
		Color32 a = GetVertexLight(axis, pos, offset[0], offset[1], offset[2]);
		Color32 b = GetVertexLight(axis, pos, offset[3], offset[4], offset[5]);
		Color32 c = GetVertexLight(axis, pos, offset[6], offset[7], offset[8]);

		data.AddColors(index, a, b, c);
	}

	private void BuildSlopeLight(Axis axis, int index, Vector3i pos, MeshData data, int[] offset)
	{
		Color32 a = GetSlopeLight(axis, pos, offset[0], offset[1], offset[2]);
		Color32 b = GetSlopeLight(axis, pos, offset[3], offset[4], offset[5]);
		Color32 c = GetSlopeLight(axis, pos, offset[6], offset[7], offset[8]);
		Color32 d = GetSlopeLight(axis, pos, offset[9], offset[10], offset[11]);

		data.AddColors(index, a, b, c, d);
	}

	private Color32 GetVertexLight(Axis axis, Vector3i pos, int dx, int dy, int dz)
	{
		Vector3i a, b, c, d;

		switch (axis)
		{
		case Axis.X:
			a = pos + new Vector3i(dx, 0, 0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(dx, 0, dz);
			d = pos + new Vector3i(dx, dy, dz);
			break;

		case Axis.Y:
			a = pos + new Vector3i(0, dy, 0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(0, dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
			break;

		default:
			a = pos + new Vector3i(0, 0, dz);
			b = pos + new Vector3i(dx, 0, dz);
			c = pos + new Vector3i(0, dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
			break;
		}

		return GetFinalLight(a, b, c, d);
	}

	private Color32 GetFinalLight(Vector3i a, Vector3i b, Vector3i c, Vector3i d)
	{
		bool t1 = Map.GetBlockSafe(b.x, b.y, b.z).IsTransparent();
		bool t2 = Map.GetBlockSafe(c.x, c.y, c.z).IsTransparent();

		if (t1 || t2) 
		{
			Color32 c1 = LightUtils.GetBlockLight(a);
			Color32 c2 = LightUtils.GetBlockLight(b);
			Color32 c3 = LightUtils.GetBlockLight(c);
			Color32 c4 = LightUtils.GetBlockLight(d);

			return LightUtils.Average(c1, c2, c3, c4);
		}
		else 
		{
			Color32 c1 = LightUtils.GetBlockLight(a);
			Color32 c2 = LightUtils.GetBlockLight(b);
			Color32 c3 = LightUtils.GetBlockLight(c);

			return LightUtils.Average(c1, c2, c3);
		}
	}

	private Color32 GetSlopeLight(Axis axis, Vector3i pos, int dx, int dy, int dz)
	{
		Vector3i a, b, c, d;

		if (Utils.Sign(dy) == -1) 
		{
			if (axis == Axis.X)
			{
				a = pos + new Vector3i(dx, 0, 0);
				b = pos + new Vector3i(dx, dy, 0);
				c = pos + new Vector3i(dx, 0, dz);
				d = pos + new Vector3i(dx, dy, dz);
			}
			else
			{
				a = pos + new Vector3i(0, 0, dz);
				b = pos + new Vector3i(dx, 0, dz);
				c = pos + new Vector3i(0, dy, dz);
				d = pos + new Vector3i(dx, dy, dz);
			}
		}
		else
		{
			a = pos + new Vector3i(0, 1, 0);
			b = pos + new Vector3i(dx, 1, 0);
			c = pos + new Vector3i(0, 1, dz);
			d = pos + new Vector3i(dx, 1, dz);
		}

		if (!Map.GetBlockSafe(a.x, a.y, a.z).IsTransparent())
		{
			Color32 c1 = LightUtils.GetBlockLight(a);
			Color32 c2 = LightUtils.GetBlockLight(b);

			return LightUtils.Average(c1, c2);
		}
		else
		{
			Color32 c1 = LightUtils.GetBlockLight(a);
			Color32 c2 = LightUtils.GetBlockLight(b);
			Color32 c3 = LightUtils.GetBlockLight(c);
			Color32 c4 = LightUtils.GetBlockLight(d);

			return LightUtils.Average(c1, c2, c3, c4);
		}
	}
}
