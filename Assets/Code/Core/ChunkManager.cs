﻿using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : ScriptableObject, IUpdatable
{
	private static Chunk[,] chunks = new Chunk[Map.WidthChunks, Map.WidthChunks];
	private static Queue<PreparedMeshInfo> preparedMeshes = new Queue<PreparedMeshInfo>(256);

	private static Queue<Chunk> chunksToUpdate = new Queue<Chunk>(8);

	private void Awake()
	{
		Updater.Register(this);

		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int z = 0; z < chunks.GetLength(1); z++)
				chunks[x, z] = new Chunk(x * Chunk.Size, z * Chunk.Size);
		}
	}

	public void UpdateTick()
	{
		if (preparedMeshes.Count > 0)
		{
			PreparedMeshInfo info = preparedMeshes.Dequeue();

			for (int i = 0; i < MeshData.MeshCount; i++)
				info.chunk.SetMesh(info.data.GetMesh(i), i);
		}

		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int z = 0; z < chunks.GetLength(1); z++)
				chunks[x, z].DrawMeshes();
		}
	}

	public static Chunk GetChunk(int worldX, int worldZ)
	{
		if (!Map.IsInMap(worldX, worldZ)) 
			return null;
		
		return chunks[ToChunkX(worldX), ToChunkZ(worldZ)];
	}

	public static Chunk GetChunkFast(int worldX, int worldZ)
	{
		return chunks[ToChunkX(worldX), ToChunkZ(worldZ)];
	}

	public static void BuildChunks()
	{
		List<Chunk> sortedChunks = new List<Chunk>(256);

		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int z = 0; z < chunks.GetLength(1); z++)
				sortedChunks.Add(chunks[x, z]);
		}
		
		Vector3i playerPos = new Vector3i(Camera.main.transform.position);
		sortedChunks.Sort((first, second) => CompareChunksByDistance(first, second, playerPos));
		
		for (int i = 0; i < sortedChunks.Count; i++)
			sortedChunks[i].BuildMesh(false);
	}

	private static int CompareChunksByDistance(Chunk first, Chunk second, Vector3i pos)
	{
		return Vector3i.DistanceSquared(first.Position, pos).CompareTo(Vector3i.DistanceSquared(second.Position, pos));
	}

	public static void ProcessMeshData(PreparedMeshInfo info)
	{
		preparedMeshes.Enqueue(info);
	}

	public static void FlagChunkForUpdate(int x, int z)
	{
		QueueChunkIfNecessary(GetChunkFast(x, z));

		int localX = x & (Chunk.Size - 1);
		int localZ = z & (Chunk.Size - 1);

		if (localX == 0) QueueChunkIfNecessary(GetChunk(x - 1, z));
		else if (localX == Chunk.Size - 1) QueueChunkIfNecessary(GetChunk(x + 1, z));

		if (localZ == 0) QueueChunkIfNecessary(GetChunk(x, z - 1));
		else if (localZ == Chunk.Size - 1) QueueChunkIfNecessary(GetChunk(x, z + 1));
	}

	private static void QueueChunkIfNecessary(Chunk chunk)
	{
		if (!chunk.flaggedForUpdate)
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

	public static int ToChunkX(int x)
	{
		return x >> Chunk.SizeBits;
	}
	
	public static int ToChunkZ(int z)
	{
		return z >> Chunk.SizeBits;
	}
}