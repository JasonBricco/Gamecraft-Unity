using UnityEngine;

public static class BlockRegistry 
{
	private static Block[] blocks = 
	{
		new Air(),
		new Dirt(),
		new Grass(),
		new Sand(),
		new Stone(),
		new Water(BlockType.Water1),
		new Water(BlockType.Water2),
		new Water(BlockType.Water3),
		new Water(BlockType.Water4),
		new Water(BlockType.Water5),
		new TreeTrunk(),
		new Leaves(),
		new TallGrass(),
		new Glowstone(),
		new Pumpkin(),
		new PumpkinLeft(),
		new PumpkinRight(),
		new PumpkinBack(),
		new PumpkinFront(),
		new StoneSlope(),
		new StoneSlopeLeft(),
		new StoneSlopeRight(),
		new StoneSlopeBack(),
		new StoneSlopeFront(),
		new Ladder(),
		new LadderLeft(),
		new LadderRight(),
		new LadderBack(),
		new LadderFront(),
		new GrassSlope(),
		new GrassSlopeLeft(),
		new GrassSlopeRight(),
		new GrassSlopeBack(),
		new GrassSlopeFront(),
		new Boundary(),
		new Cloud()
	};

	public static Block GetBlock(int ID)
	{
		return blocks[ID];
	}
}
