using UnityEngine;

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

	protected override void Setup(int wX, int wZ)
	{
		rand = new System.Random(wX * 10 + wZ * 1000);
		terrainNoise.GenerateNoise(wX, wZ);
		islandNoise.GenerateNoise(wX, wZ);

		Vector3i worldPos = new Vector3i(wX, 0, wZ);

		terrainNoise3D.GenerateNoise(worldPos);
		islandNoise3D.GenerateNoise(worldPos);
		caveNoise3D.GenerateNoise(worldPos);
	}

	protected override void GenerateColumn(int wX, int wZ, int offset)
	{
		int terrainHeight = Mathf.Max(GetTerrainHeight(wX, wZ) - offset, 1);
		int islandHeight = Mathf.Max(GetIslandHeight(wX, wZ) - offset, 1);

		bool forest = Voronoi.GetValue(wX, wZ, 0.005f, 1.0f) <= 0.0f;

		for (int y = 0; y < Map.Height; y++) 
		{
			if (y <= Map.SeaLevel) Map.SetBlock(wX, y, wZ, new Block(BlockID.Water, FluidSimulator.MaxFluidLevel));
			
			if (y <= terrainHeight) 
			{
				GenerateBase(wX, y, wZ);
				continue;
			}
			
			if (y <= islandHeight) 
			{
				GenerateIsland(wX, y, wZ, islandHeight - y, islandHeight);
				continue;
			}
		}

		double val = rand.NextDouble();

		if (forest)
		{
			if (val <= 0.05)
				TreeGenerator.BoxyTreeTerrain(wX, islandHeight, wZ);
		}
		else
		{
			if (Map.GetBlock(wX, islandHeight, wZ).ID == BlockID.Grass && !Map.GetBlock(wX, islandHeight + 1, wZ).IsFluid())
			{
				if (val <= 0.2)
					Map.SetBlock(wX, islandHeight + 1, wZ, new Block(BlockID.TallGrass));
			}
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

	private int GetTerrainHeight(int x, int z)
	{
		float height = terrainNoise.GetNoise(x, z) * 10;
		return (int)(height + 20.0f);
	}

	private int GetIslandHeight(int x, int z) 
	{
		float height = islandNoise.GetNoise(x, z) * 50;
		return (int)(height + 20.0f);
	}

	private void GenerateBase(int x, int y, int z) 
	{
		float noise = terrainNoise3D.GetNoise(x, y, z);
		Block block = new Block(BlockID.Air);
		
		if (IsInRange(noise, 0.0f, 0.6f))
			block.ID = BlockID.Dirt;

		if (IsInRange(noise, 0.6f, 1.0f)) block.ID = BlockID.Stone;
		
		if (block.ID != BlockID.Air) 
			Map.SetBlock(x, y, z, block);
	}
	
	private void GenerateIsland(int x, int y, int z, int deep, int height) 
	{
		if (caveNoise3D.GetNoise(x, y, z) > 0.7f) return;
		
		float noise = islandNoise3D.GetNoise(x, y, z);
		Block block = new Block(BlockID.Air);
		
		if (IsInRange(noise, 0.0f, 0.6f)) 
			block.ID = y == height ? BlockID.Grass : BlockID.Dirt;

		if (IsInRange(noise, 0.6f, 1.0f)) block.ID = BlockID.Stone;

		if (block.ID != BlockID.Air) 
			Map.SetBlock(x, y, z, block);
	}
}
