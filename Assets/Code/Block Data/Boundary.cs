using UnityEngine;

public class Boundary : Block 
{
	public Boundary()
	{
		solid = false;
		blockParticles = false;
		surface = false;
		transparent = false;
		ignoreRaycast = true;
	}

	public override void Build(int x, int y, int z, MeshData data)
	{
	}

	public override CullType GetCullType(int face)
	{
		return CullType.Solid;
	}
}
