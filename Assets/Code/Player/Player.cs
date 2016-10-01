using UnityEngine;
using System.Collections;

public enum MoveState { Standard, Flying, Climbing }

public class Player : MonoBehaviour
{
	[SerializeField] private GameObject boxCollider;
	private CharacterController charController;
	
	private Controller current;
	private StandardController standardController;
	private FlyingController flyingController;
	private ClimbingController climbingController;

	private BlockCollision collision;

	private CollisionFlags colFlags;

	private ushort prevLegsBlock;
	private ushort prevHeadBlock;

	public CollisionFlags ColFlags 
	{ 
		get { return colFlags; }
		set { colFlags = value; }
	}

	private void Awake()
	{
		collision = new BlockCollision(boxCollider);

		charController = GetComponent<CharacterController>();
		standardController = GetComponent<StandardController>();
		flyingController = GetComponent<FlyingController>();
		climbingController = GetComponent<ClimbingController>();

		standardController.Initialize(charController, this);
		flyingController.Initialize(charController, this);
		climbingController.Initialize(charController, this);

		current = standardController;
		SetMovementState(MoveState.Standard);

		EventManager.OnGameEvent += (type) =>
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
		};

		EventManager.OnCommand += (command, args) => 
		{
			if (command == CommandType.Teleport)
			{
				int x = Mathf.Clamp(int.Parse(args[1]), 0, 511);
				int y = Mathf.Clamp(int.Parse(args[2]), 0, 255);
				int z = Mathf.Clamp(int.Parse(args[3]), 0, 511);

				transform.position = new Vector3(x, y, z);
			}
		};
	}
	
	private void Spawn()
	{
		Vector3 position = MapData.GetData().playerPos;
		
		if (!Mathf.Approximately(position.x, -1.0f))
		{
			transform.position = position;
			return;
		}

		Vector3 spawn = TryFindLand();

		spawn.y += 0.45f;
		transform.position = spawn;
	}
	
	private Vector3 TryFindLand()
	{
		Vector3i? land = null;
		Vector3i center = new Vector3i(Map.GetWorldCenter());

		for (int x = center.x - 20; x <= center.x + 20; x++)
		{
			for (int z = center.z - 20; z <= center.z + 20; z++)
			{
				int height = MapLight.GetRaySafe(x, z);
				ushort surface = Map.GetBlockSafe(x, height - 1, z);
				
				if (!BlockRegistry.GetBlock(surface).IsFluid)
				{
					Vector3i current = new Vector3i(x, height, z);

					if (!land.HasValue)
						land = current;
					else
					{
						int dis = center.DistanceSquared(current);
						int oldDis = center.DistanceSquared(land.Value);

						if (dis < oldDis) land = current;
					}
				}
			}
		}

		if (land.HasValue)
			return land.Value.ToVector3();

		return new Vector3(center.x, MapLight.GetRay(center.x, center.z), center.z);
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
	
	private void FixedUpdate()
	{
		if (Engine.CurrentState == GameState.Playing)
			collision.SetColliders(transform.position);
	}
	
	public void SetMovementState(MoveState state)
	{
		current.enabled = false;

		switch (state)
		{
		case MoveState.Standard:
			current = standardController;
			break;

		case MoveState.Flying:
			current = flyingController;
			break;

		case MoveState.Climbing:
			current = climbingController;
			break;
		}

		current.enabled = true;
	}

	public void ProcessBlocksInside(ushort legsBlock, ushort headBlock)
	{
		if (legsBlock != prevLegsBlock)
		{
			BlockRegistry.GetBlock(prevLegsBlock).OnExit(false);
			BlockRegistry.GetBlock(legsBlock).OnEnter(false);
		}

		if (headBlock != prevHeadBlock)
		{
			BlockRegistry.GetBlock(prevHeadBlock).OnExit(true);
			BlockRegistry.GetBlock(headBlock).OnEnter(true);
		}

		prevLegsBlock = legsBlock;
		prevHeadBlock = headBlock;
	}
	
	public ushort GetSurroundingBlock(int x, int y, int z)
	{
		return collision.GetSurroundingBlock(x, y, z);
	}
	
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if ((colFlags & CollisionFlags.Sides) != 0)
		{
			Vector3i pos = Utils.GetBlockPos(hit.transform.position);
			ushort block = Map.GetBlockSafe(pos.x, pos.y, pos.z);

			if (BlockRegistry.GetBlock(block).MoveState == MoveState.Climbing)
				SetMovementState(MoveState.Climbing);
		}
	}
}