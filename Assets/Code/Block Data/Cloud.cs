using UnityEngine;

public class Cloud : Block 
{
	private ShaderType currentShader = ShaderType.TransparentCulled;

	public Cloud()
	{
		name = "Cloud";
		genericID = BlockType.Cloud;
		elements.SetAll(0.0f);
		solid = false;
		blockParticles = false;
		surface = false;
		meshIndex = 3;
		transparent = true;
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
			ScreenFader.SetFade(1.0f, 1.0f, 1.0f, 0.5f);
			SetShader(ShaderType.Transparent);
			PlayerInteraction.DisallowReticle();
		}
	}
	
	public override void OnExit(bool head)
	{
		if (head)
		{
			ScreenFader.SetFade(0.0f, 0.0f, 0.0f, 0.0f);
			SetShader(ShaderType.TransparentCulled);
			PlayerInteraction.AllowReticle();
		}
	}
	
	public override bool IsFaceVisible(ushort neighbor, int face)
	{
		Block nBlock = BlockRegistry.GetBlock(neighbor);

		CullType cull = nBlock.GetCullType(face);
		
		if (cull == CullType.Solid || cull == CullType.Cutout)
			return false;

		if (cull == CullType.Transparent)
			return nBlock.IsFluid;
		
		return true;
	}
}
