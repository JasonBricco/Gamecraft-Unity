using UnityEngine;

public class Ladder : Block
{
	public Ladder()
	{
		name = "Ladder";
		genericID = BlockType.Ladder;
		elements.SetAll(1.0f);
		moveState = MoveState.Climbing;
		transparent = true;
		blockParticles = false;
		canAttach = false;
		meshIndex = 2;
	}

	public override void Build(int x, int y, int z, MeshData data)
	{
		builder.BuildLadder(this, x, y, z, data);
	}

	public override ushort TryGetNonGenericID(Vector3i dir, int x, int y, int z)
	{
		if (dir == Vector3i.up || dir == Vector3i.down) 
		{
			Map.SetBlock(x, y, z, 0);
			return 0;
		}

		return SetRotatedID(x, y, z, dir);
	}

	public override CullType GetCullType(int face)
	{
		return CullType.Unculled;
	}
}

public class LadderLeft : Ladder
{
	public LadderLeft()
	{
		direction = Direction.Left;
	}

	public override void OnPlace(Vector3i dir, int x, int y, int z)
	{
		if (!BlockRegistry.GetBlock(Map.GetBlockSafe(x + 1, y, z)).CanAttach)
		{
			Map.SetBlock(x, y, z, 0);
			return;
		}
	}

	public override void SetCollision(BlockCollider collider, float x, float y, float z)
	{
		collider.EnableBox(x + 0.45f, y, z, 0.1f, 1.0f, 1.0f);
	}

	public override bool IsAttached(out int dir)
	{
		dir = Direction.Left;
		return true;
	}
}

public class LadderRight : Ladder
{
	public LadderRight()
	{
		direction = Direction.Right;
	}

	public override void OnPlace(Vector3i dir, int x, int y, int z)
	{
		if (!BlockRegistry.GetBlock(Map.GetBlockSafe(x - 1, y, z)).CanAttach)
		{
			Map.SetBlock(x, y, z, 0);
			return;
		}
	}

	public override void SetCollision(BlockCollider collider, float x, float y, float z)
	{
		collider.EnableBox(x - 0.45f, y, z, 0.1f, 1.0f, 1.0f);
	}

	public override bool IsAttached(out int dir)
	{
		dir = Direction.Right;
		return true;
	}
}

public class LadderBack : Ladder
{
	public LadderBack()
	{
		direction = Direction.Back;
	}

	public override void OnPlace(Vector3i dir, int x, int y, int z)
	{
		if (!BlockRegistry.GetBlock(Map.GetBlockSafe(x, y, z + 1)).CanAttach)
		{
			Map.SetBlock(x, y, z, 0);
			return;
		}
	}

	public override void SetCollision(BlockCollider collider, float x, float y, float z)
	{
		collider.EnableBox(x, y, z + 0.45f, 1.0f, 1.0f, 0.1f);
	}

	public override bool IsAttached(out int dir)
	{
		dir = Direction.Back;
		return true;
	}
}

public class LadderFront : Ladder
{
	public LadderFront()
	{
		direction = Direction.Front;
	}

	public override void OnPlace(Vector3i dir, int x, int y, int z)
	{
		if (!BlockRegistry.GetBlock(Map.GetBlockSafe(x, y, z - 1)).CanAttach)
		{
			Map.SetBlock(x, y, z, 0);
			return;
		}
	}

	public override void SetCollision(BlockCollider collider, float x, float y, float z)
	{
		collider.EnableBox(x, y, z - 0.45f, 1.0f, 1.0f, 0.1f);
	}

	public override bool IsAttached(out int dir)
	{
		dir = Direction.Front;
		return true;
	}
}
