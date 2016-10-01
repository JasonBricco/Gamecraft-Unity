using UnityEngine;

public enum ShaderType
{
	Diffuse,
	AlphaDiffuse,
	Transparent,
	TransparentCulled,
	Fluid,
	FluidCulled
}

public sealed class MaterialManager : ScriptableObject
{
	private static Material[] materials = new Material[4];
	private static Shader[] shaders = new Shader[6];

	private void Awake()
	{
		materials[0] = (Material)Resources.Load("Materials/Diffuse");
		materials[1] = (Material)Resources.Load("Materials/Liquid");
		materials[2] = (Material)Resources.Load("Materials/Cutout");
		materials[3] = (Material)Resources.Load("Materials/Transparent");

		shaders[0] = (Shader)Resources.Load("Shaders/Diffuse");
		shaders[1] = (Shader)Resources.Load("Shaders/Cutout");
		shaders[2] = (Shader)Resources.Load("Shaders/Transparent");
		shaders[3] = (Shader)Resources.Load("Shaders/TransparentCulled");
		shaders[4] = (Shader)Resources.Load("Shaders/Liquid");
		shaders[5] = (Shader)Resources.Load("Shaders/LiquidCulled");

		ErrorHandling.VerifyCollection(materials, "Materials");
		ErrorHandling.VerifyCollection(shaders, "Shaders");
	}

	public static Material GetMaterial(int meshIndex)
	{
		return materials[meshIndex];
	}
	
	public static Shader GetShader(ShaderType type)
	{
		return shaders[(int)type];
	}
}
