using UnityEngine;

public sealed class ShaderType
{
	public int queue;
	public Shader culled;
	public Shader unculled = null;

	public bool currentlyCulled = false;

	public ShaderType(int queue, Shader culled, Shader unculled = null)
	{
		this.queue = queue;
		this.culled = culled;
		this.unculled = unculled;
	}
}

public sealed class MaterialManager : ScriptableObject
{
	private static Material[] materials = new Material[4];

	private static ShaderType diffuse;
	private static ShaderType cutout;
	private static ShaderType transparent;
	private static ShaderType liquid;

	public static ShaderType Diffuse
	{
		get { return diffuse; }
	}

	public static ShaderType Cutout
	{
		get { return cutout; }
	}

	public static ShaderType Transparent
	{
		get { return transparent; }
	}

	public static ShaderType Liquid
	{
		get { return liquid; }
	}

	private void Awake()
	{
		materials[0] = (Material)Resources.Load("Materials/Diffuse");
		materials[1] = (Material)Resources.Load("Materials/Liquid");
		materials[2] = (Material)Resources.Load("Materials/Cutout");
		materials[3] = (Material)Resources.Load("Materials/Transparent");

		Logger.VerifyCollection(materials, "Materials");

		Shader sDiffuse = (Shader)Resources.Load("Shaders/Diffuse");
		Shader sCutout = (Shader)Resources.Load("Shaders/Cutout");
		Shader sTransparent = (Shader)Resources.Load("Shaders/Transparent");
		Shader sTransparentCulled = (Shader)Resources.Load("Shaders/TransparentCulled");
		Shader sLiquid = (Shader)Resources.Load("Shaders/Liquid");
		Shader sLiquidCulled = (Shader)Resources.Load("Shaders/LiquidCulled");

		diffuse = new ShaderType(2000, sDiffuse);
		cutout = new ShaderType(2000, sCutout);
		transparent = new ShaderType(3000, sTransparentCulled, sTransparent);
		liquid = new ShaderType(3000, sLiquidCulled, sLiquid); 

		SetShader(1, liquid, true);
		SetShader(3, transparent, true);
	}

	public static Material GetMaterial(int meshIndex)
	{
		return materials[meshIndex];
	}
	
	public static void SetShader(int meshIndex, ShaderType type, bool culled = true)
	{
		if (type.currentlyCulled != culled)
		{
			type.currentlyCulled = culled;
			materials[meshIndex].shader = culled ? type.culled : type.unculled;
			materials[meshIndex].renderQueue = type.queue;
		}
	}
}
