using UnityEngine;
using System.Collections;

public sealed class Player : Entity, IUpdatable
{
	private Vector3 spawnPoint;

	private float canFly = 0.0f;
	private float jumpVel = 15.0f;
	private float flyThrust = 40.0f;

	public override void Init(EntityManager manager, int ID)
	{
		base.Init(manager, ID);

		speed = 50.0f;
		SetFlag(EntityFlags.Friendly);

		EventManager.OnGameEvent += GameEventHandler;
		EventManager.OnCommand += CommandHandler;

		Updater.Register(this);
	}

	private void GameEventHandler(GameEventType type)
	{
		switch (type)
		{
		case GameEventType.BeginPlay:
			Spawn();
			break;

		case GameEventType.SaveWorld:
			MapData.GetData().playerPos = transform.position;
			break;
		}
	}

	private void CommandHandler(CommandType command, string[] args)
	{
		if (command == CommandType.Teleport)
		{
			int x = Mathf.Clamp(int.Parse(args[1]), 0, 511);
			int y = Mathf.Clamp(int.Parse(args[2]), 0, 255);
			int z = Mathf.Clamp(int.Parse(args[3]), 0, 511);

			transform.position = new Vector3(x, y, z);
		}
	}

	public void UpdateTick()
	{
		if (Engine.CurrentState != GameState.Playing)
			return;
		
		SetColliders(transform.position);

		canFly -= Time.deltaTime;
		canFly = Mathf.Max(canFly, 0.0f);

		Block legsBlock = GetSurroundingBlock(1, 1, 1);
		Block headBlock = GetSurroundingBlock(1, 2, 1);

		ProcessBlocksInside(legsBlock, headBlock);

		Vector3 accel = new Vector3(Input.GetAxisRaw("X") * speed, Map.Gravity, Input.GetAxisRaw("Y") * speed);

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

			if (controller.isGrounded)
			{
				if (Input.GetKey(KeyCode.Space))
				{
					canFly = 0.5f;
					velocity.y = jumpVel;
				}
			}
			else
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					if (canFly > 0.0f)
					{
						state = MoveState.Flying;
						return;
					}

					canFly = 0.5f;
				}
			}

			break;

		case MoveState.Flying:
			drag = -2.0f;

			if (Input.GetKey(KeyCode.Space))
				accel.y = flyThrust;
			else accel.y = Input.GetAxisRaw("Descend") * flyThrust;

			accel.y += velocity.y * drag;

			if (controller.isGrounded)
				state = MoveState.Standard;

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

			if (Input.GetKey(KeyCode.Space))
			{
				if (inFluidAbove) accel.y = speed;
				else 
				{
					// Allow jumping out of the fluid if the upper half of the player is above fluid.
					velocity.y = jumpVel;
					canFly = 0.5f;
				}
			}

			accel.y += velocity.y * drag;
			break;

		case MoveState.Climbing:
			drag = -8.0f;

			int belowBlockDir = GetSurroundingBlock(1, 0, 1).BlockDirection;
			int legsBlockDir = legsBlock.BlockDirection;
			int headBlockDir = headBlock.BlockDirection;

			int playerDir = Player.GetRotation();

			// True if the player is facing the ladder.
			if (Direction.IsOpposite(playerDir, legsBlockDir) || Direction.IsOpposite(playerDir, headBlockDir) || 
				Direction.IsOpposite(playerDir, belowBlockDir))
			{
				// Use our forward movement to move upward instead.
				accel.y = accel.z;
				accel.z = 0.0f;

				accel *= 0.75f;
				accel.y += (velocity.y * drag);
			}

			if (controller.isGrounded)
			{
				// Exit climbing state if we back away from the ladder on the ground.
				if (Input.GetAxisRaw("Y") == -1.0f)
				{
					state = MoveState.Standard;
					return;
				}
			}
			else
			{
				bool endClimb = true;

				for (int i = 0; i < 3; i++)
				{
					// Exit climbing state if no blocks surrounding the player trigger climbing state.
					if (GetSurroundingBlock(1, i, 1).TriggerState() == MoveState.Climbing)
						endClimb = false;
				}

				if (endClimb)
				{
					state = MoveState.Standard;
					return;
				}
			}

			break;
		}

		accel.x += (velocity.x * drag);
		accel.z += (velocity.z * drag);

		// Using the following equations of motion:

		// - p' = 1/2at^2 + vt + p.
		// - v' = at + v.
		// - a = specified by input.

		// Where a = acceleration, v = velocity, and p = position.
		// v' and p' denote new versions, while non-prime denotes old.

		// These are found by integrating up from acceleration to velocity. Use derivation
		// to go from position down to velocity and then down to acceleration to see how 
		// we can integrate back up.
		Vector3 delta = accel * 0.5f * Utils.Square(Time.deltaTime) + velocity * Time.deltaTime;
		velocity = accel * Time.deltaTime + velocity;

		// Transform our direction from world space to local space.
		delta = t.TransformDirection(delta);

		colFlags = controller.Move(delta);

		Vector3 pos = t.position;

		if (pos.y < 0.0f || pos.y > 512.0f || pos.x < -50.0f || pos.x > Map.Size + 50.0f || pos.z < -50.0f || pos.z > Map.Size + 50.0f)
			Kill();
	}

	public override void Kill()
	{
		transform.position = spawnPoint;
	}
	
	private void Spawn()
	{
		Vector3 position = MapData.GetData().playerPos;
		
		if (!Mathf.Approximately(position.x, -1.0f))
		{
			transform.position = position;
			return;
		}

		Vector3 spawn = TryFindLand(Map.GetWorldCenter());

		spawn.y += 0.45f;
		transform.position = spawn;
		spawnPoint = spawn;
	}

	public static int GetRotation()
	{
		float y = Camera.main.transform.rotation.eulerAngles.y;
		
		if (y >= 315 || y < 45)
			return Direction.Front;
		else if (y >= 45 && y < 135)
			return Direction.Right;
		else if (y >= 135 && y < 225)
			return Direction.Back;
		else
			return Direction.Left;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if ((colFlags & CollisionFlags.Sides) != 0)
		{
			Vector3i pos = Utils.GetBlockPos(hit.transform.position);
			Block block = Map.GetBlockSafe(pos.x, pos.y, pos.z);

			if (block.TriggerState() == MoveState.Climbing)
				state = MoveState.Climbing;
		}
	}
}