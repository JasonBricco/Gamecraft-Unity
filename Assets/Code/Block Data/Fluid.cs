using UnityEngine;

public class Fluid : Block 
{
	private ShaderType currentShader = ShaderType.FluidCulled;

	public Fluid()
	{
		solid = false;
		fluid = true;
		meshIndex = 1;
		transparent = true;
	}

	public override void Build(int x, int y, int z, MeshData data)
	{
		builder.BuildFluid(this, x, y, z, data);
	}

	private void SetShader(ShaderType type)
	{
		if (currentShader == type) return;

		currentShader = type;
		MaterialManager.GetMaterial(meshIndex).shader = MaterialManager.GetShader(type);
	}

	public override CullType GetCullType(int face)
	{
		return CullType.Transparent;
	}

	public override void OnEnter(bool head)
	{
		if (head)
		{
			ScreenFader.SetFade(0.0f, 0.0f, 1.0f, 0.3f);
			SetShader(ShaderType.Fluid);
			PlayerInteraction.DisallowReticle();
		}
	}

	public override void OnExit(bool head)
	{
		if (head)
		{
			ScreenFader.SetFade(0.0f, 0.0f, 0.0f, 0.0f);
			SetShader(ShaderType.FluidCulled);
			PlayerInteraction.AllowReticle();
		}
	}
}
