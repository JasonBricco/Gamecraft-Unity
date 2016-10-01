using UnityEngine;
using UnityEditor;
using System;

public class MeshAssetController : Editor 
{
	[MenuItem("Assets/Enable Mesh Asset Building")]
	private static void BuildMeshAssets()
	{
		SceneView.onSceneGUIDelegate += OnScene;
		GameObject meshBuilder = new GameObject("Mesh Asset Builder");
		meshBuilder.tag = "MeshAssetBuilder";
	}
	
	[MenuItem("Assets/Disable Mesh Asset Building")]
	private static void DisableMeshAssets()
	{
		if (SceneView.onSceneGUIDelegate != null)
		{
			Delegate[] listeners = SceneView.onSceneGUIDelegate.GetInvocationList();

			foreach (var item in listeners)
				SceneView.onSceneGUIDelegate -= (item as SceneView.OnSceneFunc);
		}

		GameObject[] objects = GameObject.FindGameObjectsWithTag("MeshAssetBuilder");

		for (int i = 0; i < objects.Length; i++)
			DestroyImmediate(objects[i]);

		GameObject[] meshes = GameObject.FindGameObjectsWithTag("CreatedMesh");

		for (int i = 0; i < meshes.Length; i++)
			DestroyImmediate(meshes[i]);
	}

	private static void OnScene(SceneView view)
	{
		Handles.BeginGUI();

		GUILayout.Label("Place a mesh asset builder on the Mesh Asset Builder object in the hierarchy to build the mesh.");
		GUILayout.Label("Mesh generation code should exist within the MonoBehaviour's Reset() method.");
		GUILayout.Label("Once the mesh is built, click \"Save as Asset\" to save it to the Meshes folder.");

		GUILayout.BeginArea(new Rect(0, Screen.height - 70, Screen.width / 2, 20));

		if (GUILayout.Button("Save as Asset"))
		{
			GameObject[] meshes = GameObject.FindGameObjectsWithTag("CreatedMesh");

			for (int i = 0; i < meshes.Length; i++)
			{
				Mesh mesh = meshes[i].GetComponent<MeshFilter>().sharedMesh;
				AssetDatabase.CreateAsset(mesh, "Assets/Meshes/" + meshes[i].name + ".asset");
			}

			AssetDatabase.Refresh();
			DisableMeshAssets();
		}

		GUILayout.EndArea();

		Handles.EndGUI();
	}
}
