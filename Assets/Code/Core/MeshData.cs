using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class MeshData 
{
	private List<Vector3> vertices = new List<Vector3>(32768);
	private List<Vector3> uv = new List<Vector3>(32768);
	private List<int> triangles = new List<int>(65536);
	private List<Color32> colors = new List<Color32>(32768);

	public List<int> Triangles
	{
		get { return triangles; }
	}

	public void AddVertex(Vector3 vertex, int x, int y, int z) 
	{
		vertex.x += x;
		vertex.y += y;
		vertex.z += z;
		
		vertices.Add(vertex);
	}

	public void AddUV(Vector3 uv)
	{
		this.uv.Add(uv);
	}

	public void SetUVs(Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool half)
	{
		uv.Add(a);
		uv.Add(b);
		uv.Add(c);
		
		if (!half) uv.Add(d);
	}

	public int GetOffset()
	{
		return vertices.Count;
	}

	public void AddColors(Color32 a, Color32 b, Color32 c)
	{
		colors.Add(a);
		colors.Add(b);
		colors.Add(c);
	}

	public void AddColors(Color32 a, Color32 b, Color32 c, Color32 d)
	{
		colors.Add(a);
		colors.Add(b);
		colors.Add(c);
		colors.Add(d);
	}

	public Mesh GetMesh()
	{
		if (vertices.Count > 65000)
			return null;

		Mesh mesh = new Mesh();

		mesh.SetVertices(vertices);
		mesh.SetUVs(0, uv);
		mesh.SetColors(colors);
		mesh.SetTriangles(triangles, 0);

		return mesh;
	}
}
