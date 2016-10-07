using UnityEngine;

public sealed class MeshManager 
{
	public const int MeshCount = 4;

	private MeshData[] meshData = new MeshData[MeshCount];

	public MeshData GetData(int index)
	{
		if (meshData[index] == null)
			meshData[index] = new MeshData();

		return meshData[index];
	}

	public Mesh GetMesh(int index)
	{
		return meshData[index] != null ? meshData[index].GetMesh() : null;
	}

	public void Reset()
	{
		for (int i = 0; i < meshData.Length; i++)
			meshData[i] = null;
	}
}
