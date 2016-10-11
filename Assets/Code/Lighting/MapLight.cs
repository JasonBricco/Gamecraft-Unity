using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public sealed class MapLight
{
	private static byte[,] rays = new byte[Map.Size, Map.Size];
	private static byte[] lights = new byte[Map.Size * Map.Height * Map.Size];
	private static byte[] sunlights = new byte[Map.Size * Map.Height * Map.Size];
	
	private static Queue<Vector3i> lightNodes = new Queue<Vector3i>();
	private static Queue<Vector3i> sunNodes = new Queue<Vector3i>();

	private static int numCompleted;
	private static int total = Map.WidthChunks * Map.WidthChunks;

	public static void GenerateAllLight()
	{
		EventManager.SendGameEvent(GameEventType.GenerateLight);
		numCompleted = 0;

		Vector3i pos = new Vector3i();
		
		for (int x = 0; x < Map.WidthChunks; x++)
		{
			for (int z = 0; z < Map.WidthChunks; z++)
			{
				pos.x = x;
				pos.z = z;
				
				ThreadPool.QueueUserWorkItem(GenerateLightSection, pos);
			}
		}
	}

	private static void GenerateLightSection(object posObj)
	{
		try
		{
			Vector3i pos = (Vector3i)posObj;

			SunlightEngine.ComputeRays(pos.x * Chunk.Size, pos.z * Chunk.Size);
			SunlightEngine.Scatter(pos.x * Chunk.Size, pos.z * Chunk.Size, new Queue<Vector3i>(), new Queue<Vector3i>());
			numCompleted++;

			if (numCompleted == total)
				ThreadManager.QueueForMainThread(LightFinished);
		}
		catch (System.Exception e)
		{
			Debug.LogError("An error has occurred. See the error log for details.");
			ErrorHandling.LogText("Error while generating lighting.", e.Message, e.StackTrace);
			Engine.SignalQuit();
		}
	}

	private static void LightFinished()
	{
		EventManager.SendGameEvent(GameEventType.BeginPlay);
		Engine.ChangeState(GameState.Playing);
		ChunkManager.BuildChunks();
	}

	public static void RecomputeLighting(int x, int y, int z)
	{
		SunlightEngine.Recompute(new Vector3i(x, y, z), sunNodes);
		BlockLightEngine.Recompute(new Vector3i(x, y, z), lightNodes);
	}

	public static void SetRay(int x, int z, byte height)
	{
		rays[x, z] = height;
	}
	
	public static int GetRay(int x, int z)
	{
		return rays[x, z];
	}
	
	public static int GetRaySafe(int x, int z)
	{
		if (!Map.IsInMap(x, z)) 
			return 0;
		
		return rays[x, z];
	}
	
	public static void SetLight(int x, int y, int z, byte light)
	{
		lights[x + Map.Size * (y + Map.Height * z)] = light;
	}
	
	public static void SetSunlight(int x, int y, int z, byte light)
	{
		sunlights[x + Map.Size * (y + Map.Height * z)] = light;
	}
	
	public static byte GetLight(int x, int y, int z)
	{
		return lights[x + Map.Size * (y + Map.Height * z)];
	}
	
	public static byte GetLightSafe(int x, int y, int z)
	{
		if (y < 0 || y >= Map.Height) return LightUtils.MinLight;
		
		if (!Map.IsInMap(x, z))
			return LightUtils.MinLight;
		
		byte light = lights[x + Map.Size * (y + Map.Height * z)];
		
		if (light == 0) return 1;
		return light;
	}
	
	public static byte GetSunlight(int x, int y, int z)
	{
		if (y >= GetRay(x, z))
			return LightUtils.MaxLight;

		byte light = sunlights[x + Map.Size * (y + Map.Height * z)];
		
		if (light == 0) return 1;
		return light;
	}
	
	public static byte GetSunlightSafe(int x, int y, int z)
	{
		if (y < 0) return LightUtils.MinLight;
		else if (y >= Map.Height) return LightUtils.MaxLight;
		
		if (!Map.IsInMap(x, z))
			return y >= Map.SeaLevel ? LightUtils.MaxLight : LightUtils.MinLight;

		if (y >= GetRay(x, z)) return LightUtils.MaxLight;

		byte light = sunlights[x + Map.Size * (y + Map.Height * z)];
		
		if (light == 0) return 1;
		return light;
	}
}
