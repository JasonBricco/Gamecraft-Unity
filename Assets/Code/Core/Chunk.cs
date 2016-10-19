using UnityEngine;
using System.Collections.Generic;

public struct PreparedMeshInfo
{
	public MeshDataGroup group;
	public Chunk chunk;

	public PreparedMeshInfo(MeshDataGroup data, Chunk chunk)
	{
		this.group = data;
		this.chunk = chunk;
	}
}

public sealed class Chunk
{
	public const int SizeBits = 5;
	public const int Size = 32;

	// Chunk position in world coordinates.
	public Vector3i Position { get; private set; }

	private Mesh[] meshes = new Mesh[MeshDataGroup.MeshCount];

	public bool flaggedForUpdate = false;

	// True if this chunk has no visible mesh to draw.
	private bool invisible = true;

	public Chunk(int wX, int wY, int wZ)
	{
		Position = new Vector3i(wX, wY, wZ);

		for (int i = 0; i < meshes.Length; i++)
			meshes[i] = new Mesh();
	}

	public void BuildMeshAsync(bool priority)
	{
		flaggedForUpdate = false;
		ThreadManager.QueueWork(BuildMesh, null, priority);
	}

	public void BuildMesh(object data)
	{
		try
		{
			MeshDataGroup group = new MeshDataGroup();

			for (int y = Position.y; y < Position.y + Chunk.Size; y++) 
			{
				for (int z = Position.z; z < Position.z + Chunk.Size; z++)
				{
					for (int x = Position.x; x < Position.x + Chunk.Size; x++)
					{
						Block block = Map.GetBlock(x, y, z);

						if (block.ID != BlockID.Air)
							block.Build(x, y, z, group.GetData(block.MeshIndex()));
					}
				}
			}

			invisible = true;
			PreparedMeshInfo info = new PreparedMeshInfo(group, this);
			Map.ProcessMeshData(info);
		}
		catch (System.Exception e)
		{
			Logger.LogError("Error while building meshes.", e.Message, e.StackTrace);
			Engine.SignalQuit();
		}
	}
	
	public void SetMesh(Mesh mesh, int index)
	{
		if (meshes[index] != null)
			GameObject.Destroy(meshes[index]);

		if (mesh != null)
			invisible = false;
		
		meshes[index] = mesh;
	}

	public void DrawMeshes()
	{
		if (invisible) return;

		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i] != null)
				Graphics.DrawMesh(meshes[i], Position.ToVector3(), Quaternion.identity, MaterialManager.GetMaterial(i), 0);
		}
	}
}
