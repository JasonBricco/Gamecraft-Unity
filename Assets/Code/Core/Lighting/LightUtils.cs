using UnityEngine;
using System.Collections.Generic;

public static class LightUtils
{
	public const byte MaxLight = 15;
	public const byte MinLight = 1;

	// Look up table for light values. This is computed by normalizing the value range [1, 15] to [0, 1], taking the square
	// root, and then taking the value to the 2.2 power. 
	public static readonly byte[] lightOutput = { 0, 20, 65, 93, 114, 131, 147, 161, 174, 186, 197, 208, 218, 228, 237, 246 };

	public static int GetLightStep(ushort block) 
	{
		return block == 0 ? 1 : 2;
	}
		
	public static Color32 GetBlockLight(Vector3i pos)
	{
		byte light = lightOutput[MapLight.GetLightSafe(pos.x, pos.y, pos.z)];
		byte sun = lightOutput[MapLight.GetSunlightSafe(pos.x, pos.y, pos.z)];

		return new Color32(light, light, light, sun);
	}

	public static Color32 GetBlockLight(int x, int y, int z) 
	{
		byte light = lightOutput[MapLight.GetLightSafe(x, y, z)];
		byte sun = lightOutput[MapLight.GetSunlightSafe(x, y, z)];

		return new Color32(light, light, light, sun);
	}
	
	public static Color32 Average(Color32 first, Color32 second, Color32 third, Color32 fourth)
	{
		int r = (first.r + second.r + third.r + fourth.r) >> 2;
		int b = (first.b + second.b + third.b + fourth.b) >> 2;
		int g = (first.g + second.g + third.g + fourth.g) >> 2;
		int a = (first.a + second.a + third.a + fourth.a) >> 2;
		
		return new Color32((byte)r, (byte)b, (byte)g, (byte)a);
	}
	
	public static Color32 Average(Color32 first, Color32 second, Color32 third)
	{
		int r = (first.r + second.r + third.r) / 3;
		int b = (first.b + second.b + third.b) / 3;
		int g = (first.g + second.g + third.g) / 3;
		int a = (first.a + second.a + third.a) / 3;
		
		return new Color32((byte)r, (byte)b, (byte)g, (byte)a);
	}
	
	public static Color32 Average(Color32 first, Color32 second)
	{
		int r = (first.r + second.r) / 2;
		int b = (first.b + second.b) / 2;
		int g = (first.g + second.g) / 2;
		int a = (first.a + second.a) / 2;
		
		return new Color32((byte)r, (byte)b, (byte)g, (byte)a);
	}
}
