using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class MeshData 
{
	public const int MeshCount = 4;

	private List<Vector3>[] vertices = new List<Vector3>[MeshCount];
	private List<Vector3>[] uv = new List<Vector3>[MeshCount];
	private List<int>[] triangles = new List<int>[MeshCount];
	private List<Color32>[] colors = new List<Color32>[MeshCount];

	public MeshData()
	{
		for (int i = 0; i < MeshCount; i++)
		{
			vertices[i] = new List<Vector3>(32768);
			uv[i] = new List<Vector3>(32768);
			triangles[i] = new List<int>(65536);
			colors[i] = new List<Color32>(32768);
		}
	}

	public void AddVertex(int index, Vector3 vertex, int x, int y, int z) 
	{
		vertex.x += x;
		vertex.y += y;
		vertex.z += z;
		
		vertices[index].Add(vertex);
	}

	public void AddUV(int index, Vector3 uv)
	{
		this.uv[index].Add(uv);
	}

	public void SetUVs(int index, Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool half)
	{
		uv[index].Add(a);
		uv[index].Add(b);
		uv[index].Add(c);
		
		if (!half) uv[index].Add(d);
	}

	public List<int> GetTriangles(int index)
	{
		return triangles[index];
	}

	public int GetOffset(int index)
	{
		return vertices[index].Count;
	}

	public void AddColors(int index, Color32 a, Color32 b, Color32 c)
	{
		colors[index].Add(a);
		colors[index].Add(b);
		colors[index].Add(c);
	}

	public void AddColors(int index, Color32 a, Color32 b, Color32 c, Color32 d)
	{
		colors[index].Add(a);
		colors[index].Add(b);
		colors[index].Add(c);
		colors[index].Add(d);
	}

	public Mesh GetMesh(int index) 
	{
		Mesh mesh = new Mesh();

		if (vertices[index].Count == 0 || vertices[index].Count > 65000)
			return mesh;

		mesh.SetVertices(vertices[index]);
		mesh.SetUVs(0, uv[index]);
		mesh.SetColors(colors[index]);
		mesh.SetTriangles(triangles[index], 0);

		return mesh;
	}
}
