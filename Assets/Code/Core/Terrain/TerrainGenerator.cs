using UnityEngine;

public class TerrainGenerator 
{
	public const float MaxDist = 115000.0f;
	public const float MinDist = 50000.0f;

	public virtual void Initialize(int x, int z, ChunkType type)
	{
	}

	protected void Generate(int worldX, int worldZ, ChunkType type)
	{
		switch (type)
		{
		case ChunkType.Normal:
			GenerateNormal(worldX, worldZ);
			break;
			
		case ChunkType.Side:
			GenerateSide(worldX, worldZ);
			break;
			
		case ChunkType.Corner:
			GenerateCorner(worldX, worldZ);
			break;
		}
	}

	private void GenerateNormal(int worldX, int worldZ)
	{
		for (int x = worldX; x < worldX + Chunk.Size; x++) 
		{
			for (int z = worldZ; z < worldZ + Chunk.Size; z++) 
				GenerateColumn(x, z, 0);
		}
	}
	
	private void GenerateSide(int worldX, int worldZ)
	{
		int cX = ChunkManager.ToChunkX(worldX);
		int cZ = ChunkManager.ToChunkZ(worldZ);
		
		int offset = 1;
		
		if (cX == 0)
		{
			for (int x = (worldX + Chunk.Size) - 1; x >= 0; x--) 
			{
				for (int z = worldZ; z < worldZ + Chunk.Size; z++) 
					GenerateColumn(x, z, offset);
				
				offset++;
			}
		}
		else if (cX == Map.WidthChunks - 1)
		{
			for (int x = worldX; x < worldX + Chunk.Size; x++) 
			{
				for (int z = worldZ; z < worldZ + Chunk.Size; z++) 
					GenerateColumn(x, z, offset);
				
				offset++;
			}
		}
		else if (cZ == 0)
		{
			for (int z = (worldZ + Chunk.Size) - 1; z >= 0; z--) 
			{
				for (int x = worldX; x < worldX + Chunk.Size; x++) 
					GenerateColumn(x, z, offset);
				
				offset++;
			}
		}
		else
		{
			for (int z = worldZ; z < worldZ + Chunk.Size; z++) 
			{
				for (int x = worldX; x < worldX + Chunk.Size; x++) 
					GenerateColumn(x, z, offset);
				
				offset++;
			}
		}
	}
	
	private void GenerateCorner(int worldX, int worldZ)
	{
		int cX = ChunkManager.ToChunkX(worldX);
		int cZ = ChunkManager.ToChunkZ(worldZ);
		
		int offset = 1;
		
		if (cX == 0 && cZ == 0)
		{
			for (int z = worldZ; z < worldZ + Chunk.Size; z++)
			{
				for (int x = worldX; x < worldX + Chunk.Size; x++)
				{
					offset = Chunk.Size - Mathf.Min(x, z);
					GenerateColumn(x, z, offset);
				}
			}
		}
		else if (cX == 0 && cZ == Map.WidthChunks - 1)
		{
			for (int z = worldZ; z < worldZ + Chunk.Size; z++) 
			{
				for (int x = worldX; x < worldX + Chunk.Size; x++) 
				{
					int xOffset = Chunk.Size - x;
					int zOffset = Chunk.Size - (Map.Size - z);

					offset = zOffset >= xOffset ? zOffset + 1 : xOffset;
					GenerateColumn(x, z, offset);
				}
			}
		}
		else if (cX == Map.WidthChunks - 1 && cZ == 0)
		{
			for (int z = worldZ; z < worldZ + Chunk.Size; z++) 
			{
				for (int x = worldX; x < worldX + Chunk.Size; x++) 
				{
					int xOffset = Chunk.Size - (Map.Size - x);
					int zOffset = Chunk.Size - z;
					
					offset = xOffset >= zOffset ? xOffset + 1 : zOffset;
					GenerateColumn(x, z, offset);
				}
			}
		}
		else
		{
			for (int z = worldZ; z < worldZ + Chunk.Size; z++) 
			{
				for (int x = worldX; x < worldX + Chunk.Size; x++) 
				{
					offset = (Mathf.Max(x, z) - (Map.Size - Chunk.Size)) + 1;
					GenerateColumn(x, z, offset);
				}
			}
		}
	}

	protected virtual void GenerateColumn(int x, int z, int offset)
	{
	}

	protected bool IsInRange(float val, float min, float max) 
	{
		return val >= min && val <= max;
	}

	protected int ComputeEdgeFalloff(int x, int z, float height)
	{
		return (int)height;
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
