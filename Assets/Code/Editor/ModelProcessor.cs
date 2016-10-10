using UnityEngine;
using UnityEditor;

public sealed class ModelProcessor  : AssetPostprocessor
{
	private void OnPreprocessModel()
	{
		ModelImporter importer = assetImporter as ModelImporter;
		importer.addCollider = false;
		importer.animationType = ModelImporterAnimationType.None;
		importer.isReadable = false;
		importer.importMaterials = false;
		importer.optimizeMesh = true;
		importer.globalScale = 100.0f;
	}

	// Removes all extra objects created by Unity when importing from Blender,
	// and extracts the mesh as an independent asset.
	private void OnPostprocessModel(GameObject obj)
	{
		foreach (Transform t in obj.transform)
		{
			MeshFilter filter = t.GetComponent<MeshFilter>();

			if (filter != null)
			{
				Mesh mesh = filter.sharedMesh;

				// Rotate vertices to correct the mesh's orientation when imported from Blender.
				Vector3 center = Vector3.zero;
				Quaternion newRot = new Quaternion();
				newRot.eulerAngles = new Vector3(-90.0f, 90.0f, 0.0f);

				Vector3[] vertices = mesh.vertices;
				Vector3[] newVertices = new Vector3[vertices.Length];

				for (int i = 0; i < vertices.Length; i++)
					newVertices[i] = newRot * (vertices[i] - center) + center;

				mesh.vertices = newVertices;

				AssetDatabase.CreateAsset(mesh, "Assets/Meshes/" + filter.name + ".asset");
			}
		}
	}
}
