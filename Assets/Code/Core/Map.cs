using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading;

public struct AdjacentBlocks
{
	public Block left, right, front, back, top, bottom;
}

public static class Map
{
	public const int Size = 512;
	public const int Height = 128;
	public const int WidthChunks = Size / Chunk.Size;
	public const int SeaLevel = 50;
	public const int Radius = Size / 2;

	private static int generatorID;
	
	private static Block[] blocks = new Block[Height * Size * Size];

	private static int numCompleted;
	private static int total = WidthChunks * WidthChunks;

	public static int GetBlockCount()
	{
		return blocks.Length;
	}

	public static Vector3i GetWorldCenter()
	{
		return new Vector3i(Radius, 0, Radius);
	}

	public static void SetWorldType(int ID)
	{
		generatorID = ID;
	}

	public static void GenerateAllTerrain()
	{
		numCompleted = 0;

		Perlin2D.Initialize();
		Perlin3D.Initialize();
		Voronoi.Initialize();
		
		Vector3i pos = new Vector3i();

		for (int z = 0; z < WidthChunks; z++)
		{
			for (int x = 0; x < WidthChunks; x++)
			{
				pos.x = x;
				pos.z = z;

				ThreadPool.QueueUserWorkItem(GenerateTerrainSection, pos);
			}
		}
	}

	private static void GenerateTerrainSection(object posObj)
	{
		try
		{
			Vector3i pos = (Vector3i)posObj;

			TerrainGenerator.GetGenerator(generatorID).Initialize(pos.x * Chunk.Size, pos.z * Chunk.Size);

			numCompleted++;
		}
		catch (System.Exception e)
		{
			Debug.LogError("An error has occurred. See the error log for details.");
			ErrorHandling.LogText("Error while building terrain.", e.Message, e.StackTrace);
			Engine.SignalQuit();
		}
	}

	public static float GetProgress(UnityAction callback)
	{
		float percent = (float)numCompleted / total;

		if (numCompleted == total) 
			callback();

		return percent;
	}

	public static Block GetBlock(int x, int y, int z)
	{
		return blocks[x + Size * (y + Height * z)];
	}

	public static void SetBlock(int x, int y, int z, Block block)
	{
		blocks[x + Size * (y + Height * z)] = block;
	}

	public static Block GetBlockSafe(int x, int y, int z)
	{
		if (y >= Height) return new Block(BlockID.Air);
		
		if (!IsInMap(x, y, z))
			return new Block(BlockID.Boundary);
		
		return blocks[x + Size * (y + Height * z)];
	}

	public static void SetBlockSafe(int x, int y, int z, Block block)
	{
		if (y >= Height || !IsInMap(x, y, z)) return;

		blocks[x + Size * (y + Height * z)] = block;
	}

	public static Block GetBlockDirect(int index)
	{
		return blocks[index];
	}

	public static void SetBlockDirect(int index, Block block)
	{
		blocks[index] = block;
	}

	public static void SetBlockAdvanced(int x, int y, int z, Block block, Vector3i normal, bool undo)
	{
		if (IsInMap(x, y, z))
		{
			if (!block.CanPlace(normal, x, y, z))
				return;
			
			BlockInstance prevInst = new BlockInstance(GetBlock(x, y, z), x, y, z);
			BlockInstance newBlock = new BlockInstance(block, x, y, z);

			if (block.ID == BlockID.Air)
			{
				if (!prevInst.block.CanDelete())
					return;

				prevInst.block.OnDelete(x, y, z);
			}
			else block.OnPlace(normal, x, y, z);

			if (undo) UndoManager.RegisterAction(prevInst, newBlock);
			SetBlock(x, y, z, block);
			
			MapLight.RecomputeLighting(x, y, z);
		
			ChunkManager.FlagChunkForUpdate(x, z);
			ChunkManager.UpdateMeshes();
		}
	}

	public static void SetBlocksAdvanced(List<BlockInstance> blocks, bool undo)
	{
		List<BlockInstance> prevBlocks = new List<BlockInstance>();
		
		for (int i = 0; i < blocks.Count; i++)
		{
			BlockInstance blockInst = blocks[i];

			int x = blockInst.x;
			int y = blockInst.y;
			int z = blockInst.z;
			
			if (!IsInMap(x, y, z)) continue;

			if (!blockInst.block.CanPlace(Vector3i.zero, x, y, z))
				continue;

			BlockInstance prevInst = new BlockInstance(GetBlock(x, y, z), x, y, z);

			if (blockInst.block.ID == BlockID.Air)
			{
				if (!prevInst.block.CanDelete())
					continue;
				else prevInst.block.OnDelete(x, y, z);
			} else blockInst.block.OnPlace(Vector3i.zero, x, y, z);

			if (undo) prevBlocks.Add(new BlockInstance(GetBlock(x, y, z), x, y, z));
			
			SetBlock(x, y, z, blocks[i].block);
			
			MapLight.RecomputeLighting(x, y, z);
			ChunkManager.FlagChunkForUpdate(x, z);
		}

		ChunkManager.UpdateMeshes();
		
		if (undo) UndoManager.RegisterAction(prevBlocks, blocks);
	}

	public static AdjacentBlocks GetAdjacentBlocks(int x, int y, int z)
	{
		AdjacentBlocks blocks = new AdjacentBlocks();

		blocks.left = GetBlockSafe(x - 1, y, z);
		blocks.right = GetBlockSafe(x + 1, y, z);
		blocks.back = GetBlockSafe(x, y, z - 1);
		blocks.front = GetBlockSafe(x, y, z + 1);
		blocks.bottom = GetBlockSafe(x, y - 1, z);
		blocks.top = GetBlockSafe(x, y + 1, z);

		return blocks;
	}

	public static int GetSurface(int x, int z)
	{
		for (int y = Map.Height - 1; y >= 0; y--)
		{
			Block block = GetBlock(x, y, z);
			
			if (block.IsSurface())
				return y;
		}
		
		return 0;
	}

	public static bool IsInMap(int x, int y, int z)
	{
		if (x >= 0 && z >= 0 && y >= 0 && x < Size && z < Size && y < Height)
			return true;
		
		return false;
	}
	
	public static bool IsInMap(int x, int z)
	{
		if (x >= 0 && z >= 0 && x < Size && z < Size)
			return true;
		
		return false;
	}
}
	