using UnityEngine;

public sealed class FlatGenerator : TerrainGenerator 
{
	protected override void GenerateColumn(int x, int z, int offset)
	{
		int height = 60 - offset;

		for (int y = 0; y < Map.Height; y++)
		{
			if (y < height - 30) Map.SetBlock(x, y, z, new Block(BlockID.Stone));
			else if (y < height) Map.SetBlock(x, y, z, new Block(BlockID.Dirt));
			else if (y == height) Map.SetBlock(x, y, z, new Block(BlockID.Grass));
			else if (y <= Map.SeaLevel) Map.SetBlock(x, y, z, new Block(BlockID.Water, FluidSimulator.MaxFluidLevel));
		}
	}

	protected override void GenerateOuter(int x, int z)
	{
		for (int y = 0; y < Map.Height; y++)
		{
			if (y == 0) Map.SetBlock(x, y, z, new Block(BlockID.Stone));
			else 
			{
				if (y <= Map.SeaLevel) 
					Map.SetBlock(x, y, z, new Block(BlockID.Water, FluidSimulator.MaxFluidLevel));
			}
		}
	}
}
