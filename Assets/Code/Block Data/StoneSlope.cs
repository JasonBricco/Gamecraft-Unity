using UnityEngine;

public class StoneSlope : Slope 
{
	public StoneSlope()
	{
		genericID = BlockType.StoneSlope;
		name = "Stone Slope";
		elements.SetAll(5.0f);
	}
}

public class StoneSlopeLeft : StoneSlope 
{
	public override void Build(int x, int y, int z, MeshData data)
	{
		builder.BuildLeftSlope(this, x, y, z, data);
	}

	protected override Mesh BuildCollider(CollisionMeshData data)
	{
		return builder.BuildLeftSlopeCollider(data);
	}

	protected override CollisionType GetCollisionType()
	{
		return CollisionType.SlopeLeft;
	}

	public override CullType GetCullType(int face)
	{
		if (face == Direction.Up || face == Direction.Left || face == Direction.Front || face == Direction.Back)
			return CullType.Unculled;

		return CullType.Solid;
	}
}

public class StoneSlopeRight : StoneSlope 
{
	public override void Build(int x, int y, int z, MeshData data)
	{
		builder.BuildRightSlope(this, x, y, z, data);
	}

	protected override Mesh BuildCollider(CollisionMeshData data)
	{
		return builder.BuildRightSlopeCollider(data);
	}

	protected override CollisionType GetCollisionType()
	{
		return CollisionType.SlopeRight;
	}

	public override CullType GetCullType(int face)
	{
		if (face == Direction.Up || face == Direction.Right || face == Direction.Front || face == Direction.Back)
			return CullType.Unculled;
	
		return CullType.Solid;
	}
}

public class StoneSlopeFront : StoneSlope 
{
	public override void Build(int x, int y, int z, MeshData data)
	{
		builder.BuildFrontSlope(this, x, y, z, data);
	}

	protected override Mesh BuildCollider(CollisionMeshData data)
	{
		return builder.BuildFrontSlopeCollider(data);
	}

	protected override CollisionType GetCollisionType()
	{
		return CollisionType.SlopeFront;
	}

	public override CullType GetCullType(int face)
	{
		if (face == Direction.Up || face == Direction.Front || face == Direction.Left || face == Direction.Right)
			return CullType.Unculled;
		
		return CullType.Solid;
	}
}

public class StoneSlopeBack : StoneSlope 
{
	public override void Build(int x, int y, int z, MeshData data)
	{
		builder.BuildBackSlope(this, x, y, z, data);
	}

	protected override Mesh BuildCollider(CollisionMeshData data)
	{
		return builder.BuildBackSlopeCollider(data);
	}

	protected override CollisionType GetCollisionType()
	{
		return CollisionType.SlopeBack;
	}

	public override CullType GetCullType(int face)
	{
		if (face == Direction.Up || face == Direction.Back || face == Direction.Left || face == Direction.Right)
			return CullType.Unculled;
		
		return CullType.Solid;
	}
}
	