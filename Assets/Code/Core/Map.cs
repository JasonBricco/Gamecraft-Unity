using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.IO;

public struct AdjacentBlocks
{
	public Block left, right, front, back, top, bottom;
}

public sealed class Map : ScriptableObject, IUpdatable
{
	public const int XBits = 10, YBits = 7;
	public const int Size = 1024, Height = 128;
	public const int SeaLevel = 50;
	public const int WidthChunks = Size / Chunk.Size, HeightChunks = Height / Chunk.Size;
	public const int Radius = Size / 4;
	public const int SqRadius = Radius * Radius;
	public const float Gravity = -30.0f;

	public static readonly Vector2i Center = new Vector2i(Size / 2, Size / 2);

	public static int GenID { get; private set; }

	private static Block[] blocks = new Block[Size * Height * Size];
	private static Chunk[] chunks = new Chunk[WidthChunks * HeightChunks * WidthChunks];

	private static int numCompleted;
	private static int total = WidthChunks * WidthChunks;

	private static Queue<PreparedMeshInfo> preparedMeshes = new Queue<PreparedMeshInfo>(256);
	private static Queue<Chunk> chunksToUpdate = new Queue<Chunk>(8);

	private void Awake()
	{
		Updater.Register(this);

		for (int y = 0; y < HeightChunks; y++)
		{
			for (int z = 0; z < WidthChunks; z++)
			{
				for (int x = 0; x < WidthChunks; x++)
				{
					Chunk chunk = new Chunk(x * Chunk.Size, y * Chunk.Size, z * Chunk.Size);
					chunks[x + WidthChunks * (y + HeightChunks * z)] = chunk;
				}
			}
		}
	}

	public void UpdateTick()
	{
		if (preparedMeshes.Count > 0)
		{
			PreparedMeshInfo info = preparedMeshes.Dequeue();

			for (int i = 0; i < MeshDataGroup.MeshCount; i++)
				info.chunk.SetMesh(info.group.GetMesh(i), i);
		}

		for (int c = 0; c < chunks.Length; c++)
			chunks[c].DrawMeshes();
	}

	public static void BuildChunks()
	{
		List<Chunk> sortedChunks = new List<Chunk>(256);

		for (int i = 0; i < chunks.Length; i++)
			sortedChunks.Add(chunks[i]);

		Vector3i playerPos = new Vector3i(Camera.main.transform.position);
		sortedChunks.Sort((first, second) => CompareChunksByDistance(first, second, playerPos));

		for (int i = 0; i < sortedChunks.Count; i++)
			sortedChunks[i].BuildMesh(false);
	}

	private static int CompareChunksByDistance(Chunk first, Chunk second, Vector3i pos)
	{
		return Vector3i.DistanceSquared(first.Position, pos).CompareTo(Vector3i.DistanceSquared(second.Position, pos));
	}
		
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

		for (int x = 0; x < WidthChunks; x++)
		{
			for (int z = 0; z < WidthChunks; z++)
				ThreadPool.QueueUserWorkItem(GenerateTerrainSection, new Vector2i(x, z));
		}
	}

	private static void GenerateTerrainSection(object posObj)
	{
		try
		{
			Vector2i pos = (Vector2i)posObj;
			TerrainGenerator.GetGenerator(GenID).Generate(pos.x * Chunk.Size, pos.z * Chunk.Size);

			numCompleted++;
		}
		catch (System.Exception e)
		{
			Logger.LogError("Error while building terrain.", e.Message, e.StackTrace);
			Engine.SignalQuit();
		}
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

	public static void ProcessMeshData(PreparedMeshInfo info)
	{
		preparedMeshes.Enqueue(info);
	}

	public static void FlagChunkForUpdate(int x, int y, int z)
	{
		QueueChunkIfNecessary(ChunkFromWorld(x, y, z));

		Vector3i local = ToLocalPos(x, y, z);

		if (local.x == 0) QueueChunkIfNecessary(ChunkFromWorldSafe(x - 1, y, z));
		else if (local.x == Chunk.Size - 1) QueueChunkIfNecessary(ChunkFromWorldSafe(x + 1, y, z));

		if (local.y == 0) QueueChunkIfNecessary(ChunkFromWorldSafe(x, y - 1, z));
		else if (local.y == Chunk.Size - 1) QueueChunkIfNecessary(ChunkFromWorldSafe(x, y + 1, z));

		if (local.z == 0) QueueChunkIfNecessary(ChunkFromWorldSafe(x, y, z - 1));
		else if (local.z == Chunk.Size - 1) QueueChunkIfNecessary(ChunkFromWorldSafe(x, y, z + 1));
	}

	private static void QueueChunkIfNecessary(Chunk chunk)
	{
		if (chunk != null && !chunk.flaggedForUpdate)
		{
			chunk.flaggedForUpdate = true;
			chunksToUpdate.Enqueue(chunk);
		}
	}

	public static void UpdateMeshes()
	{
		while (chunksToUpdate.Count > 0)
			chunksToUpdate.Dequeue().BuildMesh(true);
	}

	public static Chunk GetChunk(int cX, int cY, int cZ)
	{
		return chunks[cZ + WidthChunks * (cY + HeightChunks * cZ)];
	}

	public static Chunk ChunkFromWorldSafe(int wX, int wY, int wZ)
	{
		if (!InBounds(wX, wY, wZ)) 
			return null;

		Vector3i cPos = ToChunkPos(wX, wY, wZ);
		return GetChunk(cPos.x, cPos.y, cPos.z);
	}

	public static Chunk ChunkFromWorld(int wX, int wY, int wZ)
	{
		Vector3i cPos = ToChunkPos(wX, wY, wZ);
		return GetChunk(cPos.x, cPos.y, cPos.z);
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

		return GetBlock(x, y, z);
	}

	public static void SetBlockSafe(int x, int y, int z, Block block)
	{
		if (InBounds(x, y, z)) SetBlock(x, y, z, block);
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
			
			MapLight.RecomputeLighting(x, y, z);
		
			FlagChunkForUpdate(x, y, z);
			UpdateMeshes();
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
			
			MapLight.RecomputeLighting(x, y, z);
			FlagChunkForUpdate(x, y, z);
		}

		UpdateMeshes();
		
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

	public static void Encode() 
	{ 
		StreamWriter file = new StreamWriter(Engine.Path + "ChunkData.txt");

		int sectionSize = blocks.Length / 4096;
		Assert.IsTrue(sectionSize < ushort.MaxValue, "Section size is too large!");

		for (int i = 0; i < blocks.Length; i += sectionSize)
			EncodeSection(i, sectionSize, file);
		
		file.Flush();
		file.Close();
	}

	private static void EncodeSection(int start, int length, StreamWriter file)
	{
		StringWriter writer = new StringWriter();

		int i = 0;

		ushort currentCount = 0;
		ushort currentData = 0;

		for (i = start; i < start + length; i++) 
		{
			ushort thisData = (ushort)((byte)blocks[i].ID << 8 | blocks[i].data);

			if (thisData != currentData) 
			{
				if (i != 0) 
				{
					writer.Write((char)currentCount);
					writer.Write((char)currentData);
				}

				currentCount = 1;
				currentData = thisData;
			}
			else currentCount ++;

			if (i == length - 1) 
			{
				writer.Write((char)currentCount);
				writer.Write((char)currentData);
			}
		}

		string compressedData = writer.ToString();
		writer.Flush();
		writer.Close();

		file.Write(compressedData);
		file.Write((char)ushort.MaxValue);
	}

	public static bool Decode()
	{
		string path = Engine.Path + "ChunkData.txt";

		if (File.Exists(path))
		{
			StreamReader reader = new StreamReader(path);
			string[] chunkData = reader.ReadToEnd().Split((char)ushort.MaxValue);
			reader.Close();

			int sectionSize = blocks.Length / 4096;

			for (int i = 0; i < blocks.Length; i += sectionSize)
			{
				if (!DecodeSection(i, sectionSize, chunkData[i]))
					return false;
			}

			return true;
		}

		return false;
	}

	private static bool DecodeSection(int start, int length, string data)
	{
		StringReader reader = new StringReader(data);

		int i = start;

		try 
		{
			while (i < length ) 
			{
				ushort currentCount = (ushort)reader.Read();
				ushort currentData = (ushort)reader.Read();

				int j = 0;

				while (j < currentCount) 
				{
					blocks[i] = new Block((BlockID)(currentData >> 8), currentData);
					j ++;
					i ++;
				}
			}
		}
		catch (System.Exception) 
		{
			Logger.LogError("Map data is corrupt. Generating a new map.");
			reader.Close();
			return false;
		}

		reader.Close();
		return true;
	}

	public static Vector3i ToChunkPos(int x, int y, int z)
	{
		return new Vector3i(x >> Chunk.SizeBits, y >> Chunk.SizeBits, z >> Chunk.SizeBits);
	}

	public static Vector3i ToLocalPos(int x, int y, int z)
	{
		return new Vector3i(x & (Chunk.Size - 1), y & (Chunk.Size - 1), z & (Chunk.Size - 1));
	}
}
	