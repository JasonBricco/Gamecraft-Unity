using UnityEngine;
using System.Collections.Generic;

public class SunlightEngine 
{
	public static void ComputeRays(int worldX, int worldZ) 
	{
		for (int z = worldZ; z < worldZ + Chunk.Size; z++) 
		{
			for (int x = worldX; x < worldX + Chunk.Size; x++)
				ComputeRayAtPosition(x, z);
		}
	}
	
	private static void ComputeRayAtPosition(int x, int z)
	{
		int surface = Map.GetSurface(x, z);
		MapLight.SetRay(x, z, (byte)(surface + 1));
	}

	public static void Recompute(Vector3i pos, Queue<Vector3i> nodes) 
	{
		int oldSunHeight = MapLight.GetRay(pos.x, pos.z);
		ComputeRayAtPosition(pos.x, pos.z);
		int newSunHeight = MapLight.GetRay(pos.x, pos.z);
		
		if (newSunHeight < oldSunHeight)
		{
			for (int ty = newSunHeight; ty <= oldSunHeight; ty++) 
			{
				pos.y = ty;
				
				if (ty < Map.Height) 
					MapLight.SetSunlight(pos.x, pos.y, pos.z, LightUtils.MinLight);
				
				nodes.Enqueue(pos);
			}
			
			ScatterNodes(nodes, false);
			return;
		}
		
		if (newSunHeight > oldSunHeight) 
		{ 
			for (int ty = oldSunHeight; ty <= newSunHeight; ty++) 
			{
				pos.y = ty;
				
				if (ty < Map.Height) 
					MapLight.SetSunlight(pos.x, pos.y, pos.z, LightUtils.MaxLight);
				
				nodes.Enqueue(pos);
			}
			
			RemoveNodes(nodes);
			return;
		}
		
		if (newSunHeight == oldSunHeight) 
		{
			if (BlockRegistry.GetBlock(Map.GetBlock(pos.x, pos.y, pos.z)).IsTransparent)
				Update(pos, nodes);
			else
				Remove(pos, nodes);
		}
	}

	private static void Update(Vector3i pos, Queue<Vector3i> nodes) 
	{
		for (int i = 0; i < 6; i++)
		{
			Vector3i next = pos + Vector3i.directions[i];
			
			if (Map.IsInMap(next.x, next.z))
				nodes.Enqueue(next);
		}
		
		ScatterNodes(nodes, false);
	}

	public static void Scatter(int worldX, int worldZ, Queue<Vector3i> sunNodes, Queue<Vector3i> lightNodes) 
	{
		for (int x = worldX; x < worldX + Chunk.Size; x++) 
		{
			for (int z = worldZ; z < worldZ + Chunk.Size; z++) 
			{
				int maxY = ComputeMaxY(x, z);
				
				for (int y = 0; y <= maxY; y++) 
				{
					if (y >= Map.Height) continue;
					
					if (MapLight.GetSunlight(x, y, z) > LightUtils.MinLight)
						sunNodes.Enqueue(new Vector3i(x, y, z));
					
					byte light = BlockRegistry.GetBlock(Map.GetBlock(x, y, z)).LightEmitted;
					
					if (light > LightUtils.MinLight)
					{
						MapLight.SetLight(x, y, z, light);
						lightNodes.Enqueue(new Vector3i(x, y, z));
					}
				}
			}
		}
		
		ScatterNodes(sunNodes, true);
		BlockLightEngine.ScatterNodes(lightNodes, true);
	}

	public static void ScatterNodes(Queue<Vector3i> nodes, bool generator) 
	{
		while (nodes.Count > 0)
		{
			Vector3i pos = nodes.Dequeue();

			ushort block = Map.GetBlock(pos.x, pos.y, pos.z);
			int light = MapLight.GetSunlight(pos.x, pos.y, pos.z) - LightUtils.GetLightStep(block);
			
			if (light <= LightUtils.MinLight) 
				continue;
			
			for (int i = 0; i < 6; i++)
			{
				Vector3i nextPos = pos + Vector3i.directions[i];

				if (Map.IsInMap(nextPos.x, nextPos.y, nextPos.z))
				{
					block = Map.GetBlock(nextPos.x, nextPos.y, nextPos.z);

					if (BlockRegistry.GetBlock(block).IsTransparent && SetMax((byte)light, nextPos.x, nextPos.y, nextPos.z)) 
					{
						nodes.Enqueue(nextPos);

						if (!generator) ChunkManager.FlagChunkForUpdate(nextPos.x, nextPos.z);
					}
				}
			}
		}
	}

	private static void Remove(Vector3i pos, Queue<Vector3i> nodes) 
	{
		MapLight.SetSunlight(pos.x, pos.y, pos.z, LightUtils.MaxLight);
		nodes.Enqueue(pos);
		RemoveNodes(nodes);
	}

	private static void RemoveNodes(Queue<Vector3i> nodes) 
	{
		Queue<Vector3i> newNodes = new Queue<Vector3i>();
		
		while (nodes.Count > 0)
		{
			Vector3i pos = nodes.Dequeue();
			
			if (pos.y < 0 || pos.y >= Map.Height) 
				continue;
			
			if (pos.y >= MapLight.GetRay(pos.x, pos.z))
			{
				newNodes.Enqueue(pos);
				continue;
			}
			
			byte light = (byte)(MapLight.GetSunlight(pos.x, pos.y, pos.z) - 1);
			MapLight.SetSunlight(pos.x, pos.y, pos.z, LightUtils.MinLight);
			
			if (light <= LightUtils.MinLight) continue;
			
			for (int i = 0; i < 6; i++)
			{
				Vector3i nextPos = pos + Vector3i.directions[i];

				if (Map.IsInMap(nextPos.x, nextPos.z))
				{
					ushort block = Map.GetBlock(nextPos.x, nextPos.y, nextPos.z);
					
					if (BlockRegistry.GetBlock(block).IsTransparent) 
					{
						if (MapLight.GetSunlight(nextPos.x, nextPos.y, nextPos.z) <= light) 
						{
							nodes.Enqueue(nextPos);
							ChunkManager.FlagChunkForUpdate(nextPos.x, nextPos.z);
						}
						else
							newNodes.Enqueue(nextPos);
					}
				}
			}
		}
		
		ScatterNodes(newNodes, false);
	}

	private static int ComputeMaxY(int x, int z)
	{
		int maxY = MapLight.GetRay(x, z);
		maxY = Mathf.Max(maxY, MapLight.GetRaySafe(x - 1, z));
		maxY = Mathf.Max(maxY, MapLight.GetRaySafe(x + 1, z));
		maxY = Mathf.Max(maxY, MapLight.GetRaySafe(x, z - 1));
		maxY = Mathf.Max(maxY, MapLight.GetRaySafe(x, z + 1));
		
		return maxY;
	}

	private static bool SetMax(byte light, int x, int y, int z) 
	{
		if (y >= MapLight.GetRay(x, z)) 
			return false;
		
		byte oldLight = MapLight.GetSunlight(x, y, z);
		
		if (oldLight < light) 
		{
			MapLight.SetSunlight(x, y, z, light);
			return true;
		}
		
		return false;
	}
}
