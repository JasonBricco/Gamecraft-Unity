using UnityEngine;

public class TerrainGenerator 
{
	private Vector2i center;
	private int radius;

	public virtual void Initialize(int x, int z) 
	{
		int halfSize = Map.Size / 2;
		center = new Vector2i(halfSize, halfSize);
		radius = halfSize;
	}

	protected void GenerateChunk(int worldX, int worldZ)
	{
		for (int x = worldX; x < worldX + Chunk.Size; x++) 
		{
			for (int z = worldZ; z < worldZ + Chunk.Size; z++)
			{
				int valueInCircle = Utils.Square(x - center.x) + Utils.Square(z - center.z);

				int edge = Utils.Square(radius);
				int beginFalloff = edge - 8192;

				if (valueInCircle < edge)
				{
					if (valueInCircle > beginFalloff)
					{
						float p = ((valueInCircle - beginFalloff) / (float)(edge - beginFalloff)) * 15.0f;
						GenerateColumn(x, z, (int)Mathf.Pow(p, 1.5f));
					}
					else GenerateColumn(x, z, 0);
				}
			}
		}
	}

	protected virtual void GenerateColumn(int x, int z, int offset) {}

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
