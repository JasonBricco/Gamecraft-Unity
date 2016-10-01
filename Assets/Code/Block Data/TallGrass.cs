using UnityEngine;

public class TallGrass : Block 
{
	public TallGrass()
	{
		name = "Tall Grass";
		genericID = BlockType.TallGrass;
		elements.SetAll(0.0f);
		solid = false;
		blockParticles = false;
		meshIndex = 2;
		transparent = true;
		overwrite = true;
	}

	public override void Build(int x, int y, int z, MeshData data)
	{
		builder.BuildSquareCutout(this, x, y, z, data);
	}

	public override CullType GetCullType(int face)
	{
		return CullType.Unculled;
	}

	public override bool IsFaceVisible(ushort neighbor, int face)
	{
		return true;
	}

	public override bool IsAttached(out int dir)
	{
		dir = Direction.Up;
		return true;
	}

	public override void OnPlace(Vector3i dir, int x, int y, int z)
	{
		if (BlockRegistry.GetBlock(Map.GetBlockSafe(x, y - 1, z)).IsTransparent)
		{
			Map.SetBlock(x, y, z, 0);
			return;
		}
	}
}
