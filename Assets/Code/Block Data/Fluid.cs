using UnityEngine;

public class Fluid : Block 
{
	private ShaderType currentShader;

	public Fluid()
	{
		solid = false;
		fluid = true;
		meshIndex = 1;
		transparent = true;
		SetShader(ShaderType.LiquidCulled);
	}

	public override void Build(int x, int y, int z, MeshData data)
	{
		builder.BuildFluid(this, x, y, z, data);
	}

	private void SetShader(ShaderType type)
	{
		if (currentShader == type) return;

		currentShader = type;
		MaterialManager.SetShader(meshIndex, type);
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
			SetShader(ShaderType.Liquid);
			PlayerInteraction.DisallowReticle();
		}
	}

	public override void OnExit(bool head)
	{
		if (head)
		{
			ScreenFader.SetFade(0.0f, 0.0f, 0.0f, 0.0f);
			SetShader(ShaderType.LiquidCulled);
			PlayerInteraction.AllowReticle();
		}
	}
}
