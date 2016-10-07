using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct PreparedMeshInfo
{
	public MeshManager data;
	public Chunk chunk;

	public PreparedMeshInfo(MeshManager data, Chunk chunk)
	{
		this.data = data;
		this.chunk = chunk;
	}
}

public sealed class Chunk
{
	public const int SizeBits = 5;
	public const int Size = 1 << SizeBits;

	private Vector3i position;
	private Vector3 fPosition;

	private Mesh[] meshes = new Mesh[MeshManager.MeshCount];
	private MeshManager meshManager = new MeshManager();

	public Vector3i Position { get { return position; } }
	public int X { get { return position.x; } }
	public int Z { get { return position.z; } }

	public bool flaggedForUpdate = false;

	public Chunk(int worldX, int worldZ)
	{
		position = new Vector3i(worldX, 0, worldZ);
		fPosition = position.ToVector3();

		for (int i = 0; i < meshes.Length; i++)
			meshes[i] = new Mesh();
	}

	public void BuildMesh(bool priority)
	{
		flaggedForUpdate = false;
		ThreadManager.QueueWork(BuildMeshAsync, priority);
	}

	private void BuildMeshAsync(object data)
	{	
		try
		{			
			int wX = X, wZ = Z;

			for (int z = wZ; z < wZ + Chunk.Size; z++)
			{
				for (int y = 0; y < Map.Height; y++) 
				{
					for (int x = wX; x < wX + Chunk.Size; x++)
					{
						Block block = Map.GetBlock(x, y, z);

						if (block.ID != BlockID.Air)
							block.Build(x, y, z, meshManager.GetData(block.MeshIndex()));
					}
				}
			}
			
			PreparedMeshInfo info = new PreparedMeshInfo(meshManager, this);
			ChunkManager.ProcessMeshData(info);
		}
		catch (System.Exception e)
		{
			Debug.LogError("An error has occurred. See the error log for details.");
			ErrorHandling.LogText("Error while building meshes.", e.Message, e.StackTrace);
			Engine.SignalQuit();
		}
	}
	
	public void SetMesh(Mesh mesh, int index)
	{
		if (meshes[index] != null)
			GameObject.Destroy(meshes[index]);

		meshes[index] = mesh;
	}

	public void DrawMeshes()
	{
		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i] != null)
				Graphics.DrawMesh(meshes[i], fPosition, Quaternion.identity, MaterialManager.GetMaterial(i), 0);
		}
	}
}
