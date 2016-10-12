using UnityEngine;

public static class Perlin2D
{
	private static int seed;
	
	public static void Initialize() 
	{
		Events.OnSave += (data) => { data.seed2D = seed; };

		if (MapData.LoadedData == null)
			seed = Random.Range(-5000, 5000);
		else seed = MapData.LoadedData.seed2D;
	}

	public static float Noise(float x, float y, float scale, float persistence, int octaves) 
	{
		x = (x * scale) + seed;
		y = (y * scale) + seed;

		float total = 0;
		float frq = 1, amp = 1;
		
		for (int i = 0; i < octaves; i++) 
		{
			if (i >= 1) 
			{
				frq *= 2;
				amp *= persistence;
			}
			
			total += Mathf.PerlinNoise(x * frq, y * frq) * amp;
		}
		
		return total;
	}
}
