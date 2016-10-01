using UnityEngine;
using System.Collections.Generic;

public class LightUtils 
{
	public const byte MaxLight = 15;
	public const byte MinLight = 1;

	public static int GetLightStep(ushort block) 
	{
		return block == 0 ? 1 : 2;
	}

	public static Color32 GetBlockLight(Vector3i pos)
	{
		byte light = (byte)(MapLight.GetLightSafe(pos.x, pos.y, pos.z) * 17);
		byte sun = (byte)(MapLight.GetSunlightSafe(pos.x, pos.y, pos.z) * 17);
		
		return new Color32(light, light, light, sun);
	}
	
	public static Color32 GetBlockLight(int x, int y, int z) 
	{
		byte light = (byte)(MapLight.GetLightSafe(x, y, z) * 17);
		byte sun = (byte)(MapLight.GetSunlightSafe(x, y, z) * 17);
		
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
