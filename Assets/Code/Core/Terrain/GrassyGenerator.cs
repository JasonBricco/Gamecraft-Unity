using UnityEngine;

public enum ChunkType
{
	Normal,
	Side,
	Corner
}

public sealed class GrassyGenerator : TerrainGenerator 
{
	private NoiseArray2D terrainNoise, islandNoise;
	private NoiseArray3D terrainNoise3D, islandNoise3D, caveNoise3D;

	private System.Random rand;

	public GrassyGenerator()
	{
		terrainNoise = new NoiseArray2D(1.0f / 100.0f, 0.5f, 1);
		terrainNoise3D = new NoiseArray3D(1.0f / 30.0f);
		islandNoise = new NoiseArray2D(1.0f / 200.0f, 0.5f, 3);
		islandNoise3D = new NoiseArray3D(1.0f / 80.0f);
		caveNoise3D = new NoiseArray3D(1.0f / 80.0f);
	}

	public override void Initialize(int worldX, int worldZ, ChunkType type)
	{
		rand = new System.Random(worldX * 10 + worldZ * 1000);
		terrainNoise.GenerateNoise(worldX, worldZ);
		islandNoise.GenerateNoise(worldX, worldZ);

		Vector3i worldPos = new Vector3i(worldX, 0, worldZ);

		terrainNoise3D.GenerateNoise(worldPos);
		islandNoise3D.GenerateNoise(worldPos);
		caveNoise3D.GenerateNoise(worldPos);

		Generate(worldX, worldZ, type);
	}

	protected override void GenerateColumn(int x, int z, int offset)
	{
		int terrainHeight = Mathf.Max(GetTerrainHeight(x, z) - offset, 1);
		int islandHeight = Mathf.Max(GetIslandHeight(x, z) - offset, 1);
		bool forest = Voronoi.GetValue(x, z, 0.005, 1.0) <= 0.0;

		for (int y = 0; y < Map.Height; y++) 
		{
			if (y <= Map.SeaLevel) Map.SetBlock(x, y, z, BlockType.Water5);
			
			if (y <= terrainHeight) 
			{
				GenerateBase(x, y, z);
				continue;
			}
			
			if (y <= islandHeight) 
			{
				GenerateIsland(x, y, z, islandHeight - y, islandHeight);
				continue;
			}
		}

		double val = rand.NextDouble();

		if (forest)
		{
			if (val <= 0.05)
				TreeGenerator.BoxyTreeTerrain(x, islandHeight, z);
		}
		else
		{
			if (Map.GetBlock(x, islandHeight, z) == BlockType.Grass && !BlockRegistry.GetBlock(Map.GetBlock(x, islandHeight + 1, z)).IsFluid)
			{
				if (val <= 0.2)
					Map.SetBlock(x, islandHeight + 1, z, BlockType.TallGrass);
			}
		}
	}

	private int GetTerrainHeight(int x, int z)
	{
		float height = terrainNoise.GetNoise(x, z) * 10;
		return ComputeEdgeFalloff(x, z, height + 20.0f);
	}

	private int GetIslandHeight(int x, int z) 
	{
		float height = islandNoise.GetNoise(x, z) * 50;
		return ComputeEdgeFalloff(x, z, height + 20.0f);
	}

	private void GenerateBase(int x, int y, int z) 
	{
		float noise = terrainNoise3D.GetNoise(x, y, z);
		ushort block = 0;
		
		if (IsInRange(noise, 0.0f, 0.6f))
			block = BlockType.Dirt;

		if (IsInRange(noise, 0.6f, 1.0f)) block = BlockType.Stone;
		
		if (block != 0) 
			Map.SetBlock(x, y, z, block);
	}
	
	private void GenerateIsland(int x, int y, int z, int deep, int height) 
	{
		if (caveNoise3D.GetNoise(x, y, z) > 0.7f) return;
		
		float noise = islandNoise3D.GetNoise(x, y, z);
		ushort block = 0;
		
		if (IsInRange(noise, 0.0f, 0.6f)) 
			block = y == height ? BlockType.Grass : BlockType.Dirt;

		if (IsInRange(noise, 0.6f, 1.0f)) block = BlockType.Stone;
		
		if (block != 0) 
			Map.SetBlock(x, y, z, block);
	}
}
