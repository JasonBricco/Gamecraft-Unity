#if false

using UnityEngine;

public class Combiner : MonoBehaviour
{
	private static CombinedChunk[,] combinedChunks = new CombinedChunk[16, 16];
	
	private void Start()
	{
		for (int x = 0; x < combinedChunks.GetLength(0); x++)
		{
			for (int z = 0; z < combinedChunks.GetLength(1); z++)
				combinedChunks[x, z] = new CombinedChunk(x, z);
		}
	}
	
	private static CombinedChunk GetCombinedChunk(Vector3i chunkPos, out int chunkNum)
	{
		int x = chunkPos.x & 1;
		int z = chunkPos.z & 1;
		
		int chunkZ = z == 0 ? 0 : 2;
		chunkNum = x + chunkZ;
		
		return combinedChunks[chunkPos.x / 2, chunkPos.z / 2];
	}
	
	public static void SetMeshes(MeshData data, Vector3i chunkPos)
	{
		int chunkNum;
		GetCombinedChunk(chunkPos, out chunkNum).SetMeshes(data, chunkNum);
	}
	
	private void Update()
	{
		for (int x = 0; x < combinedChunks.GetLength(0); x++)
		{
			for (int z = 0; z < combinedChunks.GetLength(1); z++)
			{
				CombinedChunk chunk = combinedChunks[x, z];
				
				if (chunk.IsOutsideViewRange())
				{
					if (chunk.active)
					{
						chunk.ClearMeshes();
						continue;
					}
				}
				else
				{
					chunk.UpdateChunks();
					
					if (!chunk.active) 
						chunk.SetActive();
					
					chunk.Draw();
				}
			}
		}
	}
}

#endif
