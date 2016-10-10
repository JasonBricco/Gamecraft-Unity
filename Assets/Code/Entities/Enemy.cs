using UnityEngine;
using System.Collections.Generic;

public sealed class Enemy : Entity, IUpdatable
{
	private Transform target;

	private float detectRange = Utils.Square(16.0f);
	private float jumpVel = 10.0f;

	public override void Init(EntityManager manager, int ID)
	{
		base.Init(manager, ID);
		Updater.Register(this);
		speed = 25.0f;

		Spawn();
	}

	public override void Kill()
	{
		for (int i = 0; i < colliders.Length; i++)
			manager.ReturnCollider(colliders[i]);

		Updater.Unregister(this);
		Destroy(this.gameObject);
	}

	public void UpdateTick()
	{
		if (Engine.CurrentState != GameState.Playing)
			return;

		SetColliders(transform.position);

		Block legsBlock = GetSurroundingBlock(1, 1, 1);
		Block headBlock = GetSurroundingBlock(1, 2, 1);

		ProcessBlocksInside(legsBlock, headBlock);

		Vector3 accel = new Vector3(0.0f, Map.Gravity, 0.0f);

		bool targetFound = TryFindTarget();

		if (targetFound)
		{
			t.LookAt(target);
			accel.z = speed;
		}

		bool inFluidBelow = legsBlock.IsFluid();
		bool inFluidAbove = headBlock.IsFluid();

		bool touchingFluid = inFluidBelow || inFluidAbove;
		if (touchingFluid) state = MoveState.Swimming;

		switch (state)
		{
		case MoveState.Standard:
			if (controller.isGrounded)
				velocity.y = 0.0f;

			drag = -8.0f;
			break;

		case MoveState.Swimming:
			if (!touchingFluid)
			{
				state = MoveState.Standard;
				break;
			}

			if (controller.isGrounded)
				velocity.y = 0.0f;

			drag = -16.0f;

			if (inFluidAbove) accel.y = speed;
			else velocity.y = jumpVel;

			accel.y += velocity.y * drag;
			break;
		}

		accel.x += (velocity.x * drag);
		accel.z += (velocity.z * drag);

		Vector3 delta = accel * 0.5f * Utils.Square(Time.deltaTime) + velocity * Time.deltaTime;
		velocity = accel * Time.deltaTime + velocity;

		delta = t.TransformDirection(delta);
		colFlags = controller.Move(delta);

		Vector3 pos = t.position;

		if (pos.y < 0.0f || pos.y > 512.0f || pos.x < -50.0f || pos.x > Map.Size + 50.0f || pos.z < -50.0f || pos.z > Map.Size + 50.0f)
			Kill();
	}

	private bool TryFindTarget()
	{
		if (target != null) return true;

		List<Entity> entities = manager.GetAllEntities();

		for (int i = 0; i < entities.Count; i++)
		{
			if (entities[i].IsSet(EntityFlags.Friendly))
			{
				Transform otherT = entities[i].GetTransform();
				Vector3 pos = otherT.position;

				if ((pos - t.position).sqrMagnitude < detectRange)
				{
					target = entities[i].GetTransform();
					return true;
				}
			}
		}

		return false;
	}

	private void Spawn()
	{
		Vector3i startPos = Map.GetWorldCenter();
		Vector3 spawn = TryFindLand(new Vector3i(startPos.x + 32, startPos.y, startPos.z + 32));

		spawn.y += 0.45f;
		transform.position = spawn;
	}
}
