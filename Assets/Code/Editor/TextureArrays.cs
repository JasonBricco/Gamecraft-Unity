using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[Serializable]
public sealed class TexArrayData
{
	public string name;
	public TextureFormat format;
	public List<string> textures = new List<string>();
}

public sealed class TextureArrays : EditorWindow
{
	private List<TexArrayData> data = new List<TexArrayData>();

	private Vector2 scrollPos;

	private void Awake()
	{
		if (EditorPrefs.HasKey("ArrayCount"))
		{
			int count = EditorPrefs.GetInt("ArrayCount");

			if (count == 0) return;

			for (int i = 0; i < count; i++)
			{
				string json = EditorPrefs.GetString("TextureArray" + i);
				data.Add(JsonUtility.FromJson<TexArrayData>(json));
			}
		}
	}

	[MenuItem("Tools/Texture Arrays")]
	public static void OpenTexArrayWindow()
	{
		EditorWindow.GetWindow(typeof(TextureArrays), false, "Texture Arrays", true);
	}

	private void DrawDivider()
	{
		GUILayout.Box(String.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(4));
	}

	private void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		for (int i = 0; i < data.Count; i++)
		{
			EditorGUILayout.Space();

			TexArrayData item = data[i];

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Name");
			item.name = EditorGUILayout.TextField(item.name);

			EditorGUILayout.EndHorizontal();

			item.format = (TextureFormat)EditorGUILayout.EnumPopup("Format", item.format);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Textures");

			for (int j = 0; j < item.textures.Count; j++)
			{
				EditorGUILayout.BeginHorizontal();

				item.textures[j] = EditorGUILayout.TextField(item.textures[j]);

				EditorGUILayout.LabelField(j.ToString());

				if (GUILayout.Button("Delete"))
				{
					item.textures.RemoveAt(j);
					j--;
				}
					
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();

			if (GUILayout.Button("Add Texture"))
				item.textures.Add(String.Empty);

			if (GUILayout.Button("Delete Array"))
			{
				if (EditorUtility.DisplayDialog("Delete Texture Array", "Are you sure?", "Yes", "No"))
				{
					data.RemoveAt(i);
					i--;
				}
			}

			EditorGUILayout.Space();
			DrawDivider();
		}

		EditorGUILayout.Space();

		if (GUILayout.Button("New Texture Array"))
		{
			TexArrayData item = new TexArrayData();
			data.Add(item);
		}

		if (GUILayout.Button("Generate Arrays"))
			BuildTextureArrays();

		EditorGUILayout.EndScrollView();
	}

	private void OnDestroy()
	{
		Save();
	}

	private void BuildTextureArrays()
	{
		if (data.Count == 0) return;

		if (!AssetDatabase.IsValidFolder("Assets/Textures/Texture Arrays"))
			AssetDatabase.CreateFolder("Assets/Textures", "Texture Arrays");

		for (int i = 0; i < data.Count; i++)
		{
			TexArrayData item = data[i];

			int texCount = item.textures.Count;

			if (texCount == 0) continue;

			Texture2DArray texArray = new Texture2DArray(32, 32, texCount, item.format, true);
			texArray.filterMode = FilterMode.Point;
			texArray.mipMapBias = -0.3f;

			for (int t = 0; t < texCount; t++)
			{
				Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Blocks/" + item.textures[t] + ".png");

				if (tex == null)
				{
					Debug.LogError("Failed to find texture with the name: " + item.textures[t] + ".");
					return;
				}

				for (int mip = 0; mip < tex.mipmapCount; mip++)
					Graphics.CopyTexture(tex, 0, mip, texArray, t, mip);
			}

			AssetDatabase.CreateAsset(texArray, "Assets/Textures/Texture Arrays/" + item.name + ".asset");

			Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/" + item.name + ".mat");

			if (mat == null)
				Debug.LogError("No material was found with the name: " + item.name);
			else mat.SetTexture("_TexArray", texArray);

			AssetDatabase.Refresh();
		}

		Save();
		Close();
	}

	private void Save()
	{
		EditorPrefs.SetInt("ArrayCount", data.Count);

		for (int i = 0; i < data.Count; i++)
		{
			string json = JsonUtility.ToJson(data[i]);
			EditorPrefs.SetString("TextureArray" + i, json);
		}
	}
}
