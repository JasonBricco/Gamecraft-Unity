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
	}

	private void OnPostprocessModel(GameObject obj)
	{
		foreach (Transform t in obj.transform)
		{
			MeshFilter filter = t.GetComponent<MeshFilter>();

			if (filter != null)
			{
				Mesh mesh = filter.sharedMesh;
				AssetDatabase.CreateAsset(mesh, "Assets/Models/" + filter.name + ".asset");
			}
		}
	}
}
