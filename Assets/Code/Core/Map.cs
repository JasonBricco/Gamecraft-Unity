using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

public struct AdjacentBlocks
{
	public Block left, right, front, back, top, bottom;
}

public static class Map
{
	public const int SizeBits = 10, Size = 1 << SizeBits;
	public const int Height = 128, SeaLevel = 50;
	public const int WidthChunks = Size / Chunk.Size;
	public const int Radius = Size / 4;
	public const int SqRadius = Radius * Radius;
	public const float Gravity = -30.0f;

	public static readonly Vector2i Center = new Vector2i(Size / 2, Size / 2);

	public static int GenID { get; private set; }
	
	private static Block[] blocks = new Block[Height * Size * Size];

	// Stores whether a chunk has been modified. If so, it will be saved to disk.
	private static int[] modified = new int[WidthChunks * WidthChunks];

	private static int numCompleted;
	private static int total = WidthChunks * WidthChunks;

	public static void SetWorldType(int ID)
	{
		GenID = ID;
	}

	public static void GenerateAllTerrain()
	{
		numCompleted = 0;

		Perlin2D.Initialize();
		Perlin3D.Initialize();
		Voronoi.Initialize();
		
		Vector3i pos = new Vector3i();

		SerializableData loadedData = MapData.LoadedData;

		for (int z = 0; z < WidthChunks; z++)
		{
			for (int x = 0; x < WidthChunks; x++)
			{
				pos.x = x;
				pos.z = z;

				if (loadedData != null && loadedData.modified[z * WidthChunks + x] == 1)
					ThreadPool.QueueUserWorkItem(DecodeTerrainSection);
				else ThreadPool.QueueUserWorkItem(GenerateTerrainSection, pos);
			}
		}
	}

	private static void GenerateTerrainSection(object posObj)
	{
		try
		{
			Vector3i pos = (Vector3i)posObj;

			TerrainGenerator.GetGenerator(GenID).Generate(pos.x * Chunk.Size, pos.z * Chunk.Size);

			numCompleted++;
		}
		catch (System.Exception e)
		{
			Logger.Log("Error while building terrain.", e.Message, e.StackTrace);
			Engine.SignalQuit();
		}
	}

	private static void DecodeTerrainSection(object state)
	{
		#if false
		SerializableData data = MapData.LoadedData;

		int cur = 0;

		for (int run = 0; run < data.countList.Count; run++)
		{
			for (int i = 0; i < data.countList[run]; i++)
			{
				ushort block = data.dataList[run];
				Map.SetBlockDirect(cur, new Block((BlockID)(block >> 8), block));
				cur++;
			}
		}

		data.countList.Clear();
		data.dataList.Clear();
		#endif
	}

	public static float GetProgress(UnityAction callback)
	{
		float percent = (float)numCompleted / total;

		if (numCompleted == total) 
			callback();

		TryForceComplete(callback);
		return percent;
	}

	[Conditional("DEBUG")]
	public static void TryForceComplete(UnityAction callback)
	{
		if (Input.GetKeyDown(KeyCode.Return))
			callback();
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
		if (y < 0 || !InBounds(x, z)) return new Block(BlockID.Boundary);
		
		return blocks[x + Size * (y + Height * z)];
	}

	public static void SetBlockSafe(int x, int y, int z, Block block)
	{
		if (InBounds(x, y, z))
			blocks[x + Size * (y + Height * z)] = block;
	}

	public static void SetBlockAdvanced(int x, int y, int z, Block block, Vector3i normal, bool undo)
	{
		if (InBounds(x, y, z))
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
			modified[ChunkManager.ToChunkZ(z) * WidthChunks + ChunkManager.ToChunkX(x)] = 1;
			
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
			
			if (!InBounds(x, y, z)) continue;

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
			modified[ChunkManager.ToChunkZ(z) * WidthChunks + ChunkManager.ToChunkX(x)] = 1;
			
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

	public static bool InBounds(int x, int y, int z)
	{
		return x >= 0 && z >= 0 && y >= 0 && x < Size && z < Size && y < Height;
	}
	
	public static bool InBounds(int x, int z)
	{
		return x >= 0 && z >= 0 && x < Size && z < Size;
	}

	public static void Encode(SerializableData data) 
	{ 
		data.modified = modified;

		int i = 0;
		int runCount = 0;
		Block currentBlock = new Block(BlockID.Air);

		for (int cZ = 0; cZ < modified.GetLength(1); cZ++)
		{
			for (int cX = 0; cX < modified.GetLength(0); cX++)
			{
				if (modified[cZ * WidthChunks + cX] != 1) continue;

				Vector2i wPos = new Vector2i(cX * Chunk.Size, cZ * Chunk.Size);

				for (int y = 0; y < Height; y++)
				{
					for (int z = wPos.z; z < wPos.z + Chunk.Size; z++)
					{
						for (int x = wPos.x; x < wPos.x + Chunk.Size; x++) 
						{
							Block nextBlock = GetBlock(x, y, z);

							if (nextBlock.ID != currentBlock.ID || nextBlock.data != currentBlock.data) 
							{
								if (i != 0) 
								{
									data.countList.Add(runCount);
									data.dataList.Add((ushort)((byte)currentBlock.ID << 8 | currentBlock.data));
								}

								runCount = 1;
								currentBlock = nextBlock;
							}
							else
								runCount++;

							if (i == blocks.Length - 1) 
							{
								data.countList.Add(runCount);
								data.dataList.Add((ushort)((byte)currentBlock.ID << 8 | currentBlock.data));
							}

							i++;
						}
					}
				}
			}
		}
	}
}
	