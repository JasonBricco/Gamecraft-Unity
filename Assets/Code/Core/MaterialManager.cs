using UnityEngine;
using System;

public sealed class ShaderType : IEquatable<ShaderType>
{
	public static readonly ShaderType Diffuse = new ShaderType(2000, 0);
	public static readonly ShaderType AlphaDiffuse = new ShaderType(2000, 1);
	public static readonly ShaderType Transparent = new ShaderType(3000, 2);
	public static readonly ShaderType TransparentCulled = new ShaderType(3000, 3);
	public static readonly ShaderType Liquid = new ShaderType(3000, 4);
	public static readonly ShaderType LiquidCulled = new ShaderType(3000, 5);

	public int queue;
	public int ID;

	public ShaderType(int queue, int ID)
	{
		this.queue = queue;
		this.ID = ID;
	}

	public bool Equals(ShaderType other)
	{
		return this.ID == other.ID;
	}
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
	
	public static void SetShader(int meshIndex, ShaderType type)
	{
		materials[meshIndex].shader = shaders[type.ID];
		materials[meshIndex].renderQueue = type.queue;
	}
}
