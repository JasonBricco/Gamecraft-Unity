using UnityEngine;
using System.Collections.Generic;

public class OuterWaterBuilder : MonoBehaviour 
{
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvs = new List<Vector2>();

	private int offset;

	private void Reset()
	{
		offset = 0;

		if (gameObject.name != "Mesh Asset Builder")
		{
			Debug.Log("Place on a Mesh Asset Builder object.");
			DestroyImmediate(this);
			return;
		}

		Mesh mesh = new Mesh();

		int unit = 512;
		int width = unit * 3;

		vertices.Add(new Vector3(0, width - unit));
		vertices.Add(new Vector3(unit, width));
		vertices.Add(new Vector3(unit, width - unit));
		vertices.Add(new Vector3(0, width));

		vertices.Add(new Vector3(unit, width - unit));
		vertices.Add(new Vector3(unit * 2, width));
		vertices.Add(new Vector3(unit * 2, width - unit));
		vertices.Add(new Vector3(unit, width));

		vertices.Add(new Vector3(unit * 2, width - unit));
		vertices.Add(new Vector3(width, width));
		vertices.Add(new Vector3(width, width - unit));
		vertices.Add(new Vector3(unit * 2, width));

		vertices.Add(new Vector3(0, unit));
		vertices.Add(new Vector3(unit, width - unit));
		vertices.Add(new Vector3(unit, unit));
		vertices.Add(new Vector3(0, width - unit));

		vertices.Add(new Vector3(unit * 2, unit));
		vertices.Add(new Vector3(width, width - unit));
		vertices.Add(new Vector3(width, unit));
		vertices.Add(new Vector3(unit * 2, width - unit));

		vertices.Add(new Vector3(0, 0));
		vertices.Add(new Vector3(unit, unit));
		vertices.Add(new Vector3(unit, 0));
		vertices.Add(new Vector3(0, unit));
	
		vertices.Add(new Vector3(unit, 0));
		vertices.Add(new Vector3(unit * 2, unit));
		vertices.Add(new Vector3(unit * 2, 0));
		vertices.Add(new Vector3(unit, unit));

		vertices.Add(new Vector3(unit * 2, 0));
		vertices.Add(new Vector3(width, unit));
		vertices.Add(new Vector3(width, 0));
		vertices.Add(new Vector3(unit * 2, unit));

		for (int i = 0; i < 8; i++)
			AddTriangles();

		for (int i = 0; i < 8; i++)
		{
			uvs.Add(new Vector2(0, 0));
			uvs.Add(new Vector2(1, 1));
			uvs.Add(new Vector2(1, 0));
			uvs.Add(new Vector2(0, 1));
		}

		mesh.SetVertices(vertices);
		mesh.SetUVs(0, uvs);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();

		GameObject createdMesh = new GameObject("OuterWaterMesh");
		createdMesh.tag = "CreatedMesh";
		createdMesh.AddComponent(typeof(MeshFilter));
		createdMesh.AddComponent(typeof(MeshRenderer));

		createdMesh.GetComponent<MeshFilter>().sharedMesh = mesh;
	}

	private void AddTriangles()
	{
		triangles.Add(offset);
		triangles.Add(offset + 1);
		triangles.Add(offset + 2);

		triangles.Add(offset + 1);
		triangles.Add(offset);
		triangles.Add(offset + 3);

		offset += 4;
	}
}
