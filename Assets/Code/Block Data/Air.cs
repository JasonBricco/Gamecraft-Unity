using UnityEngine;

public class Air : Block 
{
	public Air()
	{
		name = "Air";
		genericID = BlockType.Air;
		solid = false;
		blockParticles = false;
		surface = false;
		transparent = true;
		ignoreRaycast = true;
		overwrite = true;
	}

	public override void Build(int x, int y, int z, MeshData data)
	{
	}

	public override CullType GetCullType(int face)
	{
		return CullType.Unculled;
	}
}
