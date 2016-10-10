using UnityEngine;
using System;
using System.Collections.Generic;

public enum MoveState { Standard, Swimming, Flying, Climbing }

[Flags] 
public enum EntityFlags
{
	Friendly = 1
}

public class Entity : MonoBehaviour, IEquatable<Entity>
{
	protected EntityManager manager;

	protected CharacterController controller;
	protected Transform t;

	protected int ID;

	protected EntityFlags flags;

	protected float speed;
	protected float drag;
	protected Vector3 velocity;

	protected MoveState state = MoveState.Standard;

	protected Block[,,] surrounding = new Block[3, 4, 3];
	protected BlockCollider[] colliders = new BlockCollider[36];

	protected CollisionFlags colFlags;

	protected Block prevLegsBlock;
	protected Block prevHeadBlock;

	public virtual void Init(EntityManager manager, int ID)
	{
		this.manager = manager;
		this.ID = ID;

		controller = GetComponent<CharacterController>();
		t = GetComponent<Transform>();

		for (int i = 0; i < colliders.Length; i++)
			colliders[i] = manager.GetCollider();
	}

	public bool Equals(Entity other)
	{
		return ID == other.ID;
	}

	public Transform GetTransform()
	{
		return t;
	}

	public bool IsSet(EntityFlags flag)
	{
		return (flags & flag) != 0;
	}

	protected void SetFlag(EntityFlags flag)
	{
		flags |= flag;
	}

	public Block GetSurroundingBlock(int x, int y, int z)
	{
		return surrounding[x, y, z];
	}

	protected void SetColliders(Vector3 pos)
	{
		Vector3i groundBlock = Utils.GetBlockPos(new Vector3(pos.x, pos.y - 1.4f, pos.z));
		Vector3i blockPos;
		int index = 0;

		for (int y = 0; y < 4; y++)
		{
			for (int x = -1; x <= 1; x++)
			{
				for (int z = -1; z <= 1; z++)
				{
					blockPos = new Vector3i(groundBlock.x + x, groundBlock.y + y, groundBlock.z + z);
					Block block = Map.GetBlockSafe(blockPos.x, blockPos.y, blockPos.z);
					surrounding[x + 1, y, z + 1] = block;
					block.SetCollision(colliders[index], blockPos.x, blockPos.y, blockPos.z);

					index++;
				}
			}
		}
	}

	public virtual void Kill() {}

	protected void ProcessBlocksInside(Block legsBlock, Block headBlock)
	{
		if (legsBlock.ID != prevLegsBlock.ID)
		{
			prevLegsBlock.OnExit(false);
			legsBlock.OnEnter(false);
		}

		if (headBlock.ID != prevHeadBlock.ID)
		{
			prevHeadBlock.OnExit(true);
			headBlock.OnEnter(true);
		}

		prevLegsBlock = legsBlock;
		prevHeadBlock = headBlock;
	}

	protected Vector3 TryFindLand(Vector3i center)
	{
		Vector3i? land = null;

		for (int x = center.x - 20; x <= center.x + 20; x++)
		{
			for (int z = center.z - 20; z <= center.z + 20; z++)
			{
				int height = MapLight.GetRaySafe(x, z);
				Block surface = Map.GetBlockSafe(x, height - 1, z);

				if (!surface.IsFluid())
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
}
