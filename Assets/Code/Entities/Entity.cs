using UnityEngine;
using System.Collections.Generic;

public enum MoveState { Standard, Swimming, Flying, Climbing }

public class Entity : MonoBehaviour 
{
	protected CharacterController controller;
	protected Transform t;

	protected float speed;
	protected float drag;
	protected Vector3 velocity;

	protected MoveState state = MoveState.Standard;

	protected ColliderPool pool;
	protected Block[,,] surrounding = new Block[3, 4, 3];
	protected BlockCollider[] colliders = new BlockCollider[36];

	protected CollisionFlags colFlags;

	protected void Init()
	{
		controller = GetComponent<CharacterController>();
		t = GetComponent<Transform>();

		pool = GameObject.FindWithTag("Engine").GetComponent<ColliderPool>();

		for (int i = 0; i < colliders.Length; i++)
			colliders[i] = pool.GetCollider();
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
}
