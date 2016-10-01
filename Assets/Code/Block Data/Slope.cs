using UnityEngine;

public class Slope : Block 
{
	public Slope()
	{
		canAttach = false;
	}

	protected virtual Mesh BuildCollider(CollisionMeshData data)
	{
		return null;
	}
	
	public override void SetCollision(BlockCollider collider, float x, float y, float z)
	{
		CollisionType colType = GetCollisionType();
		
		if (collider.Type != colType)
			collider.SetMesh(BuildCollider(new CollisionMeshData()), colType, x, y, z);
		else
			collider.EnableMesh(x, y, z);
	}

	protected virtual CollisionType GetCollisionType()
	{
		return CollisionType.None;
	}

	public override ushort TryGetNonGenericID(Vector3i dir, int x, int y, int z)
	{
		return SetRotatedID(x, y, z);
	}
}
