using UnityEngine;
using System.Collections.Generic;

public class CollisionMeshData
{
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();

	public void AddVertex(Vector3 vertex) 
	{
		vertices.Add(vertex);
	}

	public List<int> GetTriangles()
	{
		return triangles;
	}

	public int GetOffset()
	{
		return vertices.Count;
	}

	public Mesh GetMesh() 
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();

		return mesh;
	}
}
