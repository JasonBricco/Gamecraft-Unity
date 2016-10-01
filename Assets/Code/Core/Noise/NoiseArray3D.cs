using UnityEngine;

class NoiseArray3D 
{
	private const int step = 8;
	private Vector3i offset;
	private float scale = 1.0f;

	private float[,,] map = new float[Chunk.Size, Map.Height, Chunk.Size];
	
	public NoiseArray3D(float scale) 
	{
		this.scale = scale;
	}
	
	public void GenerateNoise(Vector3i offset) 
	{
		this.offset = offset;
		
		for (int x = 0; x < Chunk.Size; x += step) 
		{
			for (int y = 0; y < Map.Height; y += step) 
			{
				for (int z = 0; z < Chunk.Size; z += step) 
				{
					Vector3i a = new Vector3i(x, y, z) + offset;
					Vector3i b = a + new Vector3i(step, step, step);
					
					float a1 = Perlin3D.Noise(a.x, a.y, a.z, scale);
					float a2 = Perlin3D.Noise(b.x, a.y, a.z, scale);
					float a3 = Perlin3D.Noise(a.x, b.y, a.z, scale);
					float a4 = Perlin3D.Noise(b.x, b.y, a.z, scale);
					
					float b1 = Perlin3D.Noise(a.x, a.y, b.z, scale);
					float b2 = Perlin3D.Noise(b.x, a.y, b.z, scale);
					float b3 = Perlin3D.Noise(a.x, b.y, b.z, scale);
					float b4 = Perlin3D.Noise(b.x, b.y, b.z, scale);
					
					for (int tx = 0; tx < step && x + tx < Chunk.Size; tx++) 
					{
						for (int ty = 0; ty < step && y + ty < Map.Height; ty++) 
						{
							for (int tz = 0; tz < step && z + tz < Chunk.Size; tz++) 
							{
								float fx = (float)tx / step;
                        		float fy = (float)ty / step;
								float fz = (float)tz / step;

                        		float ta1 = Mathf.Lerp(a1, a2, fx);
                        		float ta2 = Mathf.Lerp(a3, a4, fx);
								float ta3 = Mathf.Lerp(ta1, ta2, fy);
								
								float tb1 = Mathf.Lerp(b1, b2, fx);
                        		float tb2 = Mathf.Lerp(b3, b4, fx);
								float tb3 = Mathf.Lerp(tb1, tb2, fy);
								
								float val = Mathf.Lerp(ta3, tb3, fz);
                        		map[x + tx, y + ty, z + tz] = val;
							}
                    	}
                	}
				}
            }
        }
    }

	public float GetNoise(int x, int y, int z) 
	{
		x -= offset.x;
		y -= offset.y;
		z -= offset.z;

		return map[x, y, z];
	}
}
