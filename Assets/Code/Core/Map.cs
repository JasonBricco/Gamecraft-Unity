using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading;

public struct AdjacentBlocks
{
	public ushort left, right, front, back, top, bottom;
}

public class Map
{
	public const int Size = 512;
	public const int Height = 128;
	public const int WidthChunks = Size / Chunk.Size;
	public const int SeaLevel = 50;

	private static int generatorID;
	
	private static ushort[] blocks = new ushort[Height * Size * Size];

	private static int numCompleted;
	private static int total = WidthChunks * WidthChunks;

	public static int GetBlockCount()
	{
		return Height * Size * Size;
	}

	public static Vector3 GetWorldCenter()
	{
		int n = (Size >> 1) + (Chunk.Size >> 1);
		return new Vector3(n, 0, n);
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

		for (int x = 0; x < WidthChunks; x++)
		{
			for (int z = 0; z < WidthChunks; z++)
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

	public static ushort GetBlock(int x, int y, int z)
	{
		return blocks[x + Size * (y + Height * z)];
	}

	public static void SetBlock(int x, int y, int z, ushort block)
	{
		blocks[x + Size * (y + Height * z)] = block;
	}

	public static ushort GetBlockSafe(int x, int y, int z)
	{
		if (y >= Height) return BlockType.Air;
		
		if (!IsInMap(x, y, z))
			return BlockType.Boundary;
		
		return blocks[x + Size * (y + Height * z)];
	}


	public static void SetBlockSafe(int x, int y, int z, ushort block)
	{
		if (y >= Height || !IsInMap(x, y, z)) return;

		blocks[x + Size * (y + Height * z)] = block;
	}

	public static ushort GetBlockDirect(int index)
	{
		return blocks[index];
	}

	public static void SetBlockDirect(int index, ushort block)
	{
		blocks[index] = block;
	}

	public static void SetBlockAdvanced(int x, int y, int z, ushort ID, Vector3i normal, bool undo)
	{
		if (IsInMap(x, y, z))
		{
			BlockInstance prevInstance = new BlockInstance(GetBlock(x, y, z), x, y, z);

			Block block = BlockRegistry.GetBlock(ID);

			ID = block.TryGetNonGenericID(normal, x, y, z);
			BlockInstance newBlock = new BlockInstance(ID, x, y, z);
			
			if (undo) UndoManager.RegisterAction(prevInstance, newBlock);

			SetBlock(x, y, z, ID);

			if (ID == BlockType.Air)
			{
				Block prevBlock = BlockRegistry.GetBlock(prevInstance.ID);

				if (!prevBlock.CanDelete)
					SetBlock(x, y, z, prevInstance.ID);
				else
					BlockRegistry.GetBlock(prevInstance.ID).OnDelete(x, y, z);
			}
			else
				block.OnPlace(normal, x, y, z);
			
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
			int x = blocks[i].x;
			int y = blocks[i].y;
			int z = blocks[i].z;
			
			if (!IsInMap(x, y, z)) continue;

			BlockInstance prevBlock = new BlockInstance(GetBlock(x, y, z), x, y, z);

			if (blocks[i].ID == BlockType.Air)
				BlockRegistry.GetBlock(prevBlock.ID).OnDelete(x, y, z);
			else
				BlockRegistry.GetBlock(blocks[i].ID).OnPlace(Vector3i.zero, x, y, z);
			
			if (undo) prevBlocks.Add(new BlockInstance(GetBlock(x, y, z), x, y, z));
			
			SetBlock(x, y, z, blocks[i].ID);
			
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
			ushort block = GetBlock(x, y, z);
			
			if (BlockRegistry.GetBlock(block).IsSurface)
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
	