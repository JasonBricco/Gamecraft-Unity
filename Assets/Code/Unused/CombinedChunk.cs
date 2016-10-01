#if false

using UnityEngine;
using System.Collections.Generic;

public class CombinedChunk
{
	private CombineInstance[][] instances = new CombineInstance[4][];
	private Mesh[] combinedMeshes = new Mesh[MeshData.MeshCount];
	
	private Matrix4x4[] matrices = new Matrix4x4[4];
	
	private static Queue<Mesh> toDestroy = new Queue<Mesh>();
	private StandardChunk[] chunks = new StandardChunk[4];
	
	private Vector3 worldPos;
	private bool active;
	
	public CombinedChunk(int x, int z)
	{
		int worldX = x * Chunk.Size * 2;
		int worldZ = z * Chunk.Size * 2;
		
		worldPos = new Vector3(worldX, 0, worldZ);
		
		matrices[0].SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
		matrices[1].SetTRS(new Vector3(Chunk.Size, 0, 0), Quaternion.identity, Vector3.one);
		matrices[2].SetTRS(new Vector3(0, 0, Chunk.Size), Quaternion.identity, Vector3.one);
		matrices[3].SetTRS(new Vector3(Chunk.Size, 0, Chunk.Size), Quaternion.identity, Vector3.one);
		
		for (int i = 0; i < instances.Length; i++)
		{
			instances[i] = new CombineInstance[MeshData.MeshCount];
			
			for (int j = 0; j < MeshData.MeshCount; j++)
			{
				instances[i][j] = new CombineInstance();
				instances[i][j].transform = matrices[j];
			}
		}
		
		GetChunks();
	}
	
	private void GetChunks()
	{
		Vector3i pos = new Vector3i(worldPos);
		
		chunks[0] = Engine.GetChunk(pos.x, pos.z);
		chunks[1] = Engine.GetChunk(pos.x + Chunk.Size, pos.z);
		chunks[2] = Engine.GetChunk(pos.x, pos.z + Chunk.Size);
		chunks[3] = Engine.GetChunk(pos.x + Chunk.Size, pos.z + Chunk.Size);
	}
	
	public void SetActive()
	{
		for (int i = 0; i < chunks.Length; i++)
			chunks[i].FlagSelfForUpdate();
		
		active = true;
	}
	
	public void UpdateChunks()
	{
		for (int i = 0; i < chunks.Length; i++)
			chunks[i].Update();
	}
	
	public bool IsOutsideViewRange()
	{
		Vector3 playerPos = Camera.main.transform.position;
		
		float xDiff = Mathf.Abs(playerPos.x - (worldPos.x + 16.0f));
		float zDiff = Mathf.Abs(playerPos.z - (worldPos.z + 16.0f));
		
		bool xOutside = xDiff * (1.0f / Chunk.Size) >= Settings.ViewRange + 2;
		bool zOutside = zDiff * (1.0f / Chunk.Size) >= Settings.ViewRange + 2;
		
		return xOutside || zOutside;
	}
	
	public void SetMeshes(MeshData data, int chunkNum)
	{
		for (int i = 0; i < MeshData.MeshCount; i++)
		{
			toDestroy.Enqueue(instances[i][chunkNum].mesh);
			instances[i][chunkNum].mesh = data.GetMesh(i);
		}
		
		Combine();
		active = true;
	}
	
	public void ClearMeshes()
	{
		for (int i = 0; i < MeshData.MeshCount; i++)
		{
			for (int j = 0; j < MeshData.MeshCount; j++)
				toDestroy.Enqueue(instances[i][j].mesh);
		}
		
		for (int i = 0; i < combinedMeshes.Length; i++)
		{
			if (combinedMeshes[i] != null)
				toDestroy.Enqueue(combinedMeshes[i]);
		}
		
		active = false;
	}
	
	private void Combine()
	{
		for (int i = 0; i < combinedMeshes.Length; i++)
		{
			toDestroy.Enqueue(combinedMeshes[i]);
			combinedMeshes[i] = new Mesh();
			
			for (int j = 0; j < instances[i].Length; j++)
			{
				if (instances[i][j].mesh == null)
					instances[i][j].mesh = new Mesh();
			}
			
			combinedMeshes[i].CombineMeshes(instances[i], true, true);
		}
	}
	
	public void Draw()
	{
		if (toDestroy.Count > 0)
			GameObject.Destroy(toDestroy.Dequeue());
		
		for (int i = 0; i < combinedMeshes.Length; i++)
			Graphics.DrawMesh(combinedMeshes[i], worldPos, Quaternion.identity, Engine.GetMaterial(i), 0);
	}
}

#endif
