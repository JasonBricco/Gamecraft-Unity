using UnityEngine;

public class GrassSlope : Slope 
{
	public GrassSlope()
	{
		genericID = BlockType.GrassSlope;
		name = "Grass Slope";
		elements.SetLayered(1.0f, 2.0f, 3.0f);
	}
}

public class GrassSlopeLeft : GrassSlope 
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

public class GrassSlopeRight : GrassSlope 
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

public class GrassSlopeFront : GrassSlope 
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

public class GrassSlopeBack : GrassSlope 
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
