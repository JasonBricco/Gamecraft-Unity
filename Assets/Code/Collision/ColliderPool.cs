using UnityEngine;
using System.Collections.Generic;

public enum CollisionType
{
	None,
	Cube,
	SlopeLeft,
	SlopeRight,
	SlopeBack,
	SlopeFront
}

public sealed class ColliderPool : MonoBehaviour
{
	private GameObject colliderPrefab;

	private Queue<BlockCollider> colliders = new Queue<BlockCollider>();

	private void Awake()
	{
		colliderPrefab = (GameObject)Resources.Load("Prefabs/Collider");
	}

	public BlockCollider GetCollider()
	{
		if (colliders.Count > 0)
			return colliders.Dequeue();

		GameObject col = GameObject.Instantiate(colliderPrefab) as GameObject;
		return col.GetComponent<BlockCollider>();
	}

	public void ReturnCollider(BlockCollider col)
	{
		col.Disable();
		colliders.Enqueue(col);
	}
}
