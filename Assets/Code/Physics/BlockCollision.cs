using UnityEngine;

public enum CollisionType
{
	None,
	Cube,
	SlopeLeft,
	SlopeRight,
	SlopeBack,
	SlopeFront
}

public class BlockCollision 
{
	private BlockCollider[] colliders = new BlockCollider[36];
	private Block[,,] surrounding = new Block[3, 4, 3];

	public BlockCollision(GameObject collider)
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			GameObject colObj = GameObject.Instantiate(collider) as GameObject;
			colliders[i] = colObj.GetComponent<BlockCollider>();
		}
	}

	public Block GetSurroundingBlock(int x, int y, int z)
	{
		return surrounding[x, y, z];
	}

	public void SetColliders(Vector3 pos)
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
}
