using UnityEngine;

public sealed class Prefab
{
	private GameObject prefab;

	public Prefab(string path)
	{
		prefab = Resources.Load<GameObject>(path);
	}

	public GameObject Instantiate()
	{
		return GameObject.Instantiate<GameObject>(prefab);
	}
}
