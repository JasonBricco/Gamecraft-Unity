using UnityEngine;
using System.Collections.Generic;

public sealed class EntityManager : ScriptableObject 
{
	private Prefab collider;
	private Queue<BlockCollider> colliders = new Queue<BlockCollider>();

	private List<Entity> entities = new List<Entity>();

	private Prefab enemy;

	private void Awake()
	{
		collider = new Prefab("Prefabs/Collider");
		enemy = new Prefab("Entities/Enemy");

		entities.Add(new Prefab("Entities/Player").Instantiate().GetComponent<Entity>());
		entities[0].Init(this, 0);

		EventManager.OnGameEvent += GameEventHandler;
	}

	private void GameEventHandler(GameEventType type)
	{
		switch (type)
		{
		case GameEventType.BeginPlay:
			entities.Add(enemy.Instantiate().GetComponent<Entity>());

			for (int i = 1; i < entities.Count; i++)
				entities[i].Init(this, i);

			break;
		}
	}

	public BlockCollider GetCollider()
	{
		if (colliders.Count > 0)
			return colliders.Dequeue();

		GameObject col = collider.Instantiate();
		return col.GetComponent<BlockCollider>();
	}

	public void ReturnCollider(BlockCollider col)
	{
		col.Disable();
		colliders.Enqueue(col);
	}

	public List<Entity> GetAllEntities()
	{
		return entities;
	}

	public void RemoveEntity(Entity entity)
	{
		entities.Remove(entity);
	}
}
