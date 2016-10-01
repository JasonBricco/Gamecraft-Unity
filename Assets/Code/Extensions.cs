using UnityEngine;

public static class Extensions 
{
	public static GameObject FindChild(this GameObject obj, string name)
	{
		Transform target = obj.transform.Find(name);
		return target != null ? target.gameObject : null;
	}

	public static void SetParent(this GameObject obj, GameObject parent)
	{
		obj.transform.SetParent(parent.transform);
	}
}
