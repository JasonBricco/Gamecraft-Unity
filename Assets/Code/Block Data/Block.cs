using UnityEngine;
using System.Collections.Generic;

public enum CullType 
{ 
	Solid,
	Transparent,
	Cutout,
	Unculled
}
	
public class Block 
{
	public const float TileSize = 0.0625f;

	protected static MeshBuilder builder = new MeshBuilder();

	protected ushort genericID;
	protected string name = "Unknown";
	protected int direction = Direction.None;
	protected byte lightEmitted = LightUtils.MinLight;
	protected bool transparent = false;
	protected bool solid = true;
	protected bool blockParticles = true;
	protected bool surface = true;
	protected bool canAttach = true;
	protected bool canDelete = true;
	protected bool ignoreRaycast = false;
	protected bool overwrite = false;
	protected MoveState moveState = MoveState.Standard;
	protected int meshIndex = 0;
	protected TextureElements elements = new TextureElements();
	protected bool fluid = false;
	protected int fluidLevel = 1;

	public ushort GenericID
	{
		get { return genericID; }
	}

	public string Name
	{
		get { return name; }
	}

	public int BlockDirection
	{
		get { return direction; }
	}

	public virtual byte LightEmitted
	{
		get { return lightEmitted; }
	}

	public bool IsTransparent
	{
		get { return transparent; }
	}

	public bool IsSolid
	{
		get { return solid; }
	}

	public bool IsFluid
	{
		get { return fluid; }
	}

	public int FluidLevel
	{
		get { return fluidLevel; }
	}

	public bool BlockParticles
	{
		get { return blockParticles; }
	}

	public bool IsSurface
	{
		get { return surface; }
	}

	public bool CanAttach
	{
		get { return canAttach; }
	}

	public bool CanDelete
	{
		get { return canDelete; }
	}

	public bool IgnoreRaycast
	{
		get { return ignoreRaycast; }
		set { ignoreRaycast = value; }
	}

	public bool Overwrite
	{
		get { return overwrite; }
	}

	public MoveState MoveState
	{
		get { return moveState; }
	}

	public int MeshIndex
	{
		get { return meshIndex; }
	}

	public float GetTexture(int face)
	{
		return elements[face];
	}

	public virtual ushort TryGetNonGenericID(Vector3i dir, int x, int y, int z)
	{
		return genericID;
	}

	public virtual void Build(int x, int y, int z, MeshData data)
	{
		builder.BuildCube(this, x, y, z, data);
	}

	public virtual void SetCollision(BlockCollider collider, float x, float y, float z)
	{
		if (solid)
			collider.EnableBox(x, y, z, 1.0f, 1.0f, 1.0f);
		else
			collider.Disable();
	}

	protected ushort SetRotatedID(int x, int y, int z)
	{
		int direction = Player.GetRotation();
		ushort newID = GetCorrectID(direction);
		Map.SetBlock(x, y, z, newID);

		return newID;
	}

	protected ushort SetRotatedID(int x, int y, int z, Vector3i dir)
	{
		int face = dir.GetNormalDirection();
		ushort newID = GetCorrectID(face);
		Map.SetBlock(x, y, z, newID);

		return newID;
	}
	
	private ushort GetCorrectID(int direction)
	{
		switch (direction)
		{
		case Direction.Left:
			return (ushort)(genericID + 2);
			
		case Direction.Right:
			return (ushort)(genericID + 1);
			
		case Direction.Front:
			return (ushort)(genericID + 3);
			
		case Direction.Back:
			return (ushort)(genericID + 4);
		}

		return genericID;
	}

	public virtual bool IsFaceVisible(ushort neighbor, int face)
	{
		return BlockRegistry.GetBlock(neighbor).GetCullType(face) != CullType.Solid;
	}

	public virtual CullType GetCullType(int face)
	{
		return CullType.Solid;
	}

	public virtual bool CanFlowOver()
	{
		return overwrite;
	}
	
	public virtual void KillParticle(float y, ref ParticleSystem.Particle particle)
	{
		particle.position = new Vector3(particle.position.x, y + 0.5f, particle.position.z);
		particle.lifetime = 0;
	}
	
	public virtual bool IsAttached(out int dir)
	{
		dir = -1;
		return false;
	}
	
	public virtual void OnEnter(bool head)
	{
	}
	
	public virtual void OnExit(bool head)
	{
	}

	public virtual void OnPlace(Vector3i normal, int x, int y, int z)
	{
	}
	
	public virtual void OnDelete(int x, int y, int z)
	{
		for (int i = 0; i < Vector3i.directions.Length; i++)
		{
			Vector3i dir = Vector3i.directions[i];

			int facing;

			ushort block = Map.GetBlockSafe(x + dir.x, y + dir.y, z + dir.z);

			if (BlockRegistry.GetBlock(block).IsAttached(out facing))
			{
				if (dir.GetNormalDirection() == facing)
					Map.SetBlock(x + dir.x, y + dir.y, z + dir.z, 0);
			}
		}
			
		FluidSimulator.TryFlowSurrounding(x, y, z);
	}
}
