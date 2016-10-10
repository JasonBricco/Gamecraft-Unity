using UnityEngine;

public class NoiseArray2D 
{	
	private const int step = 8;
	private Vector3i offset;

	private float scale = 0.0f;
	private float persistence = 0.5f;
	private int octaves = 5;

	private float[,] map = new float[Chunk.Size, Chunk.Size];
	
	public NoiseArray2D(float scale, float persistence, int octaves) 
	{
		this.scale = scale;
		this.persistence = persistence;
		this.octaves = octaves;
	}
	
	public void GenerateNoise(int offsetX, int offsetY) 
	{
		GenerateNoise(new Vector3i(offsetX, 0, offsetY));
	}
	
	public void GenerateNoise(Vector3i offset) 
	{
		this.offset = offset;
		
		int sizeX = map.GetLength(0);
		int sizeZ = map.GetLength(1);

        for (int x = 0; x < sizeX; x += step) 
		{
			for (int z = 0; z < sizeZ; z += step) 
			{
				Vector3i a = new Vector3i(x, 0, z) + offset;
				Vector3i b = a + new Vector3i(step, 0, step);
                
                float v1 = Perlin2D.Noise(a.x, a.z, scale, persistence, octaves);
				float v2 = Perlin2D.Noise(b.x, a.z, scale, persistence, octaves);
				float v3 = Perlin2D.Noise(a.x, b.z, scale, persistence, octaves);
				float v4 = Perlin2D.Noise(b.x, b.z, scale, persistence, octaves);
				
				for (int tx = 0; tx < step && x + tx < sizeX; tx++) 
				{
					for (int tz = 0; tz < step && z + tz < sizeZ; tz++) 
					{
						float fx = (float)tx / step;
                        float fy = (float)tz / step;
                        float i1 = Mathf.Lerp(v1, v2, fx);
                        float i2 = Mathf.Lerp(v3, v4, fx);
                        map[x + tx, z + tz] = Mathf.Lerp(i1, i2, fy);
					}
                }
            }
        }
    }
	
	public float GetNoise(int x, int z) 
	{
		x -= offset.x;
		z -= offset.z;

		return map[x, z];
	}
}
