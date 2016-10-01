using UnityEngine;

public class FlatGenerator : TerrainGenerator 
{
	public override void Initialize(int worldX, int worldZ, ChunkType type)
	{
		Generate(worldX, worldZ, type);
	}

	protected override void GenerateColumn(int x, int z, int offset)
	{
		int height = 60 - offset;

		for (int y = 0; y < Map.Height; y++)
		{
			if (y < height - 30) Map.SetBlock(x, y, z, BlockType.Stone);
			else if (y < height) Map.SetBlock(x, y, z, BlockType.Dirt);
			else if (y == height) Map.SetBlock(x, y, z, BlockType.Grass);

			if (y <= Map.SeaLevel && Map.GetBlock(x, y, z) == BlockType.Air)
				Map.SetBlock(x, y, z, BlockType.Water5);
		}
	}
}
