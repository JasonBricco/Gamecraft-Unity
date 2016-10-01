using UnityEngine;
using System.Collections.Generic;

public class BlockLightEngine 
{
	public static void Recompute(Vector3i pos, Queue<Vector3i> nodes) 
	{
		byte oldLight = MapLight.GetLight(pos.x, pos.y, pos.z);
		byte light = BlockRegistry.GetBlock(Map.GetBlock(pos.x, pos.y, pos.z)).LightEmitted;
		
		if (oldLight > light)
			Remove(pos, nodes);
		
		if (light > LightUtils.MinLight)
		{
			MapLight.SetLight(pos.x, pos.y, pos.z, light);
			Scatter(pos, nodes);
		}
		else 
			Update(pos, nodes);
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

	private static void Scatter(Vector3i pos, Queue<Vector3i> nodes) 
	{
		nodes.Enqueue(pos);
		ScatterNodes(nodes, false);
	}

	public static void ScatterNodes(Queue<Vector3i> nodes, bool generator) 
	{
		while (nodes.Count > 0)
		{
			Vector3i pos = nodes.Dequeue();

			if (pos.y < 0 || pos.y >= Map.Height) continue;
			
			ushort block = Map.GetBlock(pos.x, pos.y, pos.z);
			int light = MapLight.GetLight(pos.x, pos.y, pos.z) - LightUtils.GetLightStep(block);
			
			if (light <= LightUtils.MinLight) continue;
			
			for (int i = 0; i < 6; i++)
			{
				Vector3i nextPos = pos + Vector3i.directions[i];

				if (Map.IsInMap(nextPos.x, nextPos.z))
				{
					block = Map.GetBlock(nextPos.x, nextPos.y, nextPos.z);
				
					if (BlockRegistry.GetBlock(block).IsTransparent && SetMax((byte)light, nextPos.x, nextPos.y, nextPos.z))
						nodes.Enqueue(nextPos);
				
					if (!generator) ChunkManager.FlagChunkForUpdate(nextPos.x, nextPos.z);
				}
			}
		}
	}

	private static void Remove(Vector3i pos, Queue<Vector3i> nodes) 
	{
		MapLight.SetLight(pos.x, pos.y, pos.z, LightUtils.MaxLight);
		nodes.Enqueue(pos);
		RemoveNodes(nodes);
	}

	private static void RemoveNodes(Queue<Vector3i> nodes) 
	{
		Queue<Vector3i> newLights = new Queue<Vector3i>();
		
		while (nodes.Count > 0)
		{
			Vector3i pos = nodes.Dequeue();
			
			if (pos.y < 0 || pos.y >= Map.Height) continue;
			
			int light = MapLight.GetLight(pos.x, pos.y, pos.z) - 1;
			MapLight.SetLight(pos.x, pos.y, pos.z, LightUtils.MinLight);
			
			if (light <= LightUtils.MinLight) continue;
			
			for (int i = 0; i < 6; i++)
			{
				Vector3i nextPos = pos + Vector3i.directions[i];

				if (Map.IsInMap(nextPos.x, nextPos.z))
				{
					ushort ID = Map.GetBlock(nextPos.x, nextPos.y, nextPos.z);

					Block block = BlockRegistry.GetBlock(ID);

					if (block.IsTransparent) 
					{
						if (MapLight.GetLight(nextPos.x, nextPos.y, nextPos.z) <= light)
							nodes.Enqueue(nextPos);
						else
							newLights.Enqueue(nextPos);
					}
					
					if (block.LightEmitted > LightUtils.MinLight)
						newLights.Enqueue(nextPos);
					
					ChunkManager.FlagChunkForUpdate(nextPos.x, nextPos.z);
				}
			}	
		}
		
		ScatterNodes(newLights, false);
	}

	public static bool SetMax(byte light, int x, int y, int z) 
	{
		byte oldLight = MapLight.GetLight(x, y, z);
		
		if (oldLight < light)
		{
			MapLight.SetLight(x, y, z, light);
			return true;
		}
		
		return false;
	}
}
