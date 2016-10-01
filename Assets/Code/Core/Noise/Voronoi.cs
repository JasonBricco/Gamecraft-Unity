using System;
using UnityEngine;

public static class Voronoi
{
	private const int GeneratorNoiseX = 1619;
	private const int GeneratorNoiseZ = 6971;
	private const int GeneratorSeed = 1013;
	
	private static int seed;

	public static void Initialize() 
	{
		seed = UnityEngine.Random.Range(-5000, 5000);
	}

	public static float Distance { get; private set; }

	public static double GetValue(double x, double z, double frequency, double displacement, bool getDistance = false)
	{
		x *= frequency;
		z *= frequency;
		
		var intX = (int)x;
		var intZ = (int)z;
		var maxDistance = 2147483647.0;
		double seedCenterX = 0;
		double seedCenterZ = 0;
		
		for (var zcu = intZ - 2; zcu <= intZ + 2; zcu++)
		{
			for (var xcu = intX - 2; xcu <= intX + 2; xcu++)
			{
				var xp = xcu + ValueNoise(xcu, zcu, seed);
				var zp = zcu + ValueNoise(xcu, zcu, seed + 2);
				var xDistance = xp - x;
				var zDistance = zp - z;
				var distance = xDistance * xDistance + zDistance * zDistance;
				
				if (distance < maxDistance)
				{
					maxDistance = distance;
					seedCenterX = xp;
					seedCenterZ = zp;
				}
			}
		}

		if (getDistance)
		{
			var xDist = seedCenterX - x;
			var zDist = seedCenterZ - z;
			Distance = (float)(Math.Sqrt(xDist * xDist + zDist * zDist));
		}
		
		return (displacement * ValueNoise((int)Math.Floor(seedCenterX), (int)Math.Floor(seedCenterZ), 0));
	}

	private static double ValueNoise(int x, int z, int seed)
	{
		return 1.0 - (ValueNoiseInt(x, z, seed) / 1073741824.0);
	}

	private static long ValueNoiseInt(int x, int z, int seed)
	{
		long n = (GeneratorNoiseX * x + GeneratorNoiseZ * z + GeneratorSeed * seed) & 0x7fffffff;
		n = (n >> 13) ^ n;
		return (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
	}
}
