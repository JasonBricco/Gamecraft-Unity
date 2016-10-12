using UnityEngine;

public class TerrainGenerator 
{
	public const int IslandStart = (Map.WidthChunks / 4) * Chunk.Size;
	public const int IslandEnd = (IslandStart + ((Map.WidthChunks / 2) * Chunk.Size) - 1);

	public void Generate(int cX, int cZ) 
	{
		if (cX >= IslandStart && cZ >= IslandStart && cX <= IslandEnd && cZ <= IslandEnd)
			Setup(cX, cZ);
			
		GenerateChunk(cX, cZ);
	}

	protected void GenerateChunk(int worldX, int worldZ)
	{
		for (int z = worldZ; z < worldZ + Chunk.Size; z++)
		{
			for (int x = worldX; x < worldX + Chunk.Size; x++) 
			{
				int valueInCircle = Utils.Square(x - Map.Center.x) + Utils.Square(z - Map.Center.z);
				int beginFalloff = Map.SqRadius - 8192;

				if (valueInCircle < Map.SqRadius)
				{
					if (valueInCircle > beginFalloff)
					{
						float p = ((valueInCircle - beginFalloff) / (float)(Map.SqRadius - beginFalloff)) * 15.0f;
						GenerateColumn(x, z, (int)Mathf.Pow(p, 1.5f));
					}
					else GenerateColumn(x, z, 0);
				}
				else GenerateOuter(x, z);
			}
		}
	}

	protected virtual void Setup(int x, int z) {}

	// Generate a terrain column within the island (standard terrain).
	protected virtual void GenerateColumn(int x, int z, int offset) {}

	// Generate a terrain column outside of the island (typically ocean).
	protected virtual void GenerateOuter(int x, int z) {}

	protected bool IsInRange(float val, float min, float max) 
	{
		return val >= min && val <= max;
	}

	public static TerrainGenerator GetGenerator(int ID)
	{
		switch (ID)
		{
		case 0:
			return new GrassyGenerator();

		case 1:
			return new FlatGenerator();

		default:
			return new FlatGenerator();
		}
	}
}
