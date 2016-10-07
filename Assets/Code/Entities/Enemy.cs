using UnityEngine;

public sealed class Enemy : Entity, IUpdatable
{
	private void Awake()
	{
		Init();
		Updater.Register(this);
		speed = 25.0f;
	}

	public override void Kill()
	{
		for (int i = 0; i < colliders.Length; i++)
			pool.ReturnCollider(colliders[i]);

		Destroy(this.gameObject);
	}

	public void UpdateTick()
	{
		if (Engine.CurrentState == GameState.Playing)
			SetColliders(transform.position);
	}
}
