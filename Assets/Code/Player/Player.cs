using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IUpdatable
{
	[SerializeField] private GameObject boxCollider;

	private BlockCollision collision;

	private ushort prevLegsBlock;
	private ushort prevHeadBlock;

	private Vector3 spawnPoint;

	private void Awake()
	{
		Updater.Register(this);

		collision = new BlockCollision(boxCollider);

		//SetMovementState(MoveState.Standard);

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

	public void Kill()
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

		Vector3 spawn = TryFindLand();

		spawn.y += 0.45f;
		transform.position = spawn;
		spawnPoint = spawn;
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
	
	public void UpdateTick()
	{
		if (Engine.CurrentState == GameState.Playing)
			collision.SetColliders(transform.position);
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
}