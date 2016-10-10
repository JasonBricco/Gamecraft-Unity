using UnityEngine;

public enum CullType 
{ 
	Solid, Transparent, Cutout, Unculled
}

public enum BlockID : byte
{
	Air, Dirt, Grass, Sand, Stone, Water, TreeTrunk, Leaves, TallGrass, GlowStone, Pumpkin, StoneSlope, 
	Ladder, GrassSlope, Boundary, Cloud
}

public struct Block
{
	public BlockID ID;
	public byte data;

	public Block(BlockID ID, byte data = 0)
	{
		this.ID = ID;
		this.data = data;
	}

	public Block(BlockID ID, int data)
	{
		this.ID = ID;
		this.data = (byte)data;
	}

	public int FluidLevel
	{
		get { return data; }
		set { data = (byte)value; }
	}

	public int BlockDirection
	{
		get { return data; }
		set { data = (byte)value; }
	}

	public string Name()
	{
		switch (ID)
		{
		case BlockID.Air:
			return "Air";

		case BlockID.Cloud:
			return "Cloud";

		case BlockID.Dirt:
			return "Dirt";

		case BlockID.Grass:
			return "Grass";

		case BlockID.Leaves:
			return "Leaves";

		case BlockID.Sand:
			return "Sand";

		case BlockID.TreeTrunk:
			return "Tree Trunk";

		case BlockID.Stone:
			return "Stone";

		case BlockID.GlowStone:
			return "Glowstone";

		case BlockID.Water:
			return "Water";

		case BlockID.TallGrass:
			return "Tall Grass";

		case BlockID.Pumpkin:
			return "Pumpkin";

		case BlockID.Ladder:
			return "Ladder";

		case BlockID.StoneSlope:
			return "Stone Slope";

		case BlockID.GrassSlope:
			return "Grass Slope";

		default:
			return "None";
		}
	}

	public void Build(int x, int y, int z, MeshData data)
	{
		switch (ID)
		{
		case BlockID.Air:
		case BlockID.Boundary:
			return;

		case BlockID.Water:
			MeshBuilder.instance.BuildFluid(this, x, y, z, data);
			break;

		case BlockID.TallGrass:
			MeshBuilder.instance.BuildSquareCutout(this, x, y, z, data);
			break;

		case BlockID.Ladder:
			MeshBuilder.instance.BuildLadder(this, x, y, z, data);
			break;

		case BlockID.StoneSlope:
		case BlockID.GrassSlope:
			switch (BlockDirection)
			{
			case Direction.Left:
				MeshBuilder.instance.BuildLeftSlope(this, x, y, z, data);
				break;

			case Direction.Right:
				MeshBuilder.instance.BuildRightSlope(this, x, y, z, data);
				break;

			case Direction.Front:
				MeshBuilder.instance.BuildFrontSlope(this, x, y, z, data);
				break;

			case Direction.Back:
				MeshBuilder.instance.BuildBackSlope(this, x, y, z, data);
				break;
				
			} break;

		default:
			MeshBuilder.instance.BuildCube(this, x, y, z, data);
			break;
		}
	}

	public Mesh BuildCollider(CollisionMeshData data)
	{
		switch (ID)
		{
		case BlockID.StoneSlope:
		case BlockID.GrassSlope:
			switch (BlockDirection)
			{
			case Direction.Left:
				return MeshBuilder.instance.BuildLeftSlopeCollider(data);

			case Direction.Right:
				return MeshBuilder.instance.BuildRightSlopeCollider(data);

			case Direction.Front:
				return MeshBuilder.instance.BuildFrontSlopeCollider(data);

			case Direction.Back:
				return MeshBuilder.instance.BuildBackSlopeCollider(data);

			default:
				return null;
			}

		default:
			return null;;
		}
	}

	public CollisionType GetCollisionType()
	{
		switch (ID)
		{
		case BlockID.StoneSlope:
		case BlockID.GrassSlope:
			switch (BlockDirection)
			{
			case Direction.Left:
				return CollisionType.SlopeLeft;

			case Direction.Right:
				return CollisionType.SlopeRight;

			case Direction.Front:
				return CollisionType.SlopeFront;

			case Direction.Back:
				return CollisionType.SlopeBack;

			default:
				return CollisionType.None;
			}

		default:
			return CollisionType.Cube;
		}
	}

	public bool IsSolid()
	{
		switch (ID)
		{
		case BlockID.Air:
		case BlockID.Boundary:
		case BlockID.Cloud:
		case BlockID.Water:
		case BlockID.TallGrass:
			return false;

		default:
			return true;
		}
	}

	public bool IsFluid()
	{
		switch (ID)
		{
		case BlockID.Water:
			return true;

		default:
			return false;
		}
	}

	public bool BlockParticles()
	{
		switch (ID)
		{
		case BlockID.Air:
		case BlockID.Boundary:
		case BlockID.Cloud:
		case BlockID.TallGrass:
		case BlockID.Ladder:
			return false;

		default:
			return true;
		}
	}

	public bool IsSurface()
	{
		switch (ID)
		{
		case BlockID.Air:
		case BlockID.Boundary:
		case BlockID.Cloud:
			return false;

		default:
			return true;
		}
	}

	public bool IsTransparent()
	{
		switch (ID)
		{
		case BlockID.Air:
		case BlockID.Cloud:
		case BlockID.Water:
		case BlockID.TallGrass:
		case BlockID.Ladder:
			return true;

		default:
			return false;
		}
	}

	public bool IgnoreRaycast()
	{
		switch (ID)
		{
		case BlockID.Air:
		case BlockID.Boundary:
			return true;

		default:
			return false;
		}
	}

	public bool AllowOverwrite()
	{
		switch (ID)
		{
		case BlockID.Air:
		case BlockID.TallGrass:
			return true;

		case BlockID.Water:
			return FluidLevel < FluidSimulator.MaxFluidLevel;

		default:
			return false;
		}
	}

	public CullType GetCullType(int face)
	{
		switch (ID)
		{
		case BlockID.Air:
		case BlockID.TallGrass:
		case BlockID.Ladder:
			return CullType.Unculled;

		case BlockID.Cloud:
		case BlockID.Water:
			return CullType.Transparent;

		case BlockID.Leaves:
			return CullType.Cutout;

		case BlockID.StoneSlope:
		case BlockID.GrassSlope:
			switch (BlockDirection)
			{
			case Direction.Left:
				if (face == Direction.Up || face == Direction.Left || face == Direction.Front || face == Direction.Back)
					return CullType.Unculled;

				return CullType.Solid;

			case Direction.Right:
				if (face == Direction.Up || face == Direction.Right || face == Direction.Front || face == Direction.Back)
					return CullType.Unculled;

				return CullType.Solid;

			case Direction.Front:
				if (face == Direction.Up || face == Direction.Front || face == Direction.Left || face == Direction.Right)
					return CullType.Unculled;
				
				return CullType.Solid;

			case Direction.Back:
				if (face == Direction.Up || face == Direction.Back || face == Direction.Left || face == Direction.Right)
					return CullType.Unculled;
				
				return CullType.Solid;

			default:
				return CullType.Solid;
			}

		default:
			return CullType.Solid;
		}
	}

	public int MeshIndex()
	{
		switch (ID)
		{
		case BlockID.Water:
			return 1;

		case BlockID.TallGrass:
		case BlockID.Ladder:
			return 2;

		case BlockID.Cloud:
			return 3;

		default:
			return 0;
		}
	}

	public float GetTexture(int face)
	{
		switch (ID)
		{
		case BlockID.Dirt:
			return 3.0f;

		case BlockID.Grass:
		case BlockID.GrassSlope:
			if (face == Direction.Up)
				return 1.0f;
			
			if (face == Direction.Down)
				return 3.0f;
			
			return 2.0f;

		case BlockID.Leaves:
			return 12.0f;

		case BlockID.Sand:
			return 4.0f;

		case BlockID.TreeTrunk:
			if (face == Direction.Up || face == Direction.Down)
				return 7.0f;
			
			return 6.0f;

		case BlockID.Stone:
		case BlockID.StoneSlope:
			return 5.0f;

		case BlockID.GlowStone:
			return 8.0f;

		case BlockID.Pumpkin:
			switch (data)
			{
			case Direction.Left:
				if (face == Direction.Left) return 9.0f;
				if (face == Direction.Up) return 11.0f;
				return 10.0f;

			case Direction.Right:
				if (face == Direction.Right) return 9.0f;
				if (face == Direction.Up) return 11.0f;
				return 10.0f;

			case Direction.Front:
				if (face == Direction.Front) return 9.0f;
				if (face == Direction.Up) return 11.0f;
				return 10.0f;

			case Direction.Back:
				if (face == Direction.Back) return 9.0f;
				if (face == Direction.Up) return 11.0f;
				return 10.0f;

			default:
				return 0.0f;
			}
				
		case BlockID.Ladder:
			return 1.0f;

		default:
			return 0.0f;
		}
	}

	public void SetShaderCulling(bool cull)
	{
		switch (ID)
		{
		case BlockID.Cloud:
			MaterialManager.SetShader(MeshIndex(), MaterialManager.Transparent, cull);
			break;

		case BlockID.Water:
			MaterialManager.SetShader(MeshIndex(), MaterialManager.Liquid, cull);
			break;

		default:
			return;
		}
	}

	public byte LightEmitted()
	{
		switch (ID)
		{
		case BlockID.GlowStone:
			return LightUtils.MaxLight;

		default:
			return LightUtils.MinLight;
		}
	}

	public int GetLightStep() 
	{
		switch (ID)
		{
		case BlockID.Air:
		case BlockID.Ladder:
			return 1;

		default:
			return 2;
		}
	}

	public void OnEnter(bool head)
	{
		switch (ID)
		{
		case BlockID.Cloud:
			if (head)
			{
				ScreenFader.SetFade(1.0f, 1.0f, 1.0f, 0.5f);
				SetShaderCulling(false);
				MapInteraction.DisallowReticle();
			} break;

		case BlockID.Water:
			if (head)
			{
				ScreenFader.SetFade(0.0f, 0.0f, 1.0f, 0.3f);
				SetShaderCulling(false);
				MapInteraction.DisallowReticle();
			} break;

		default:
			return;
		}
	}

	public void OnExit(bool head)
	{
		switch (ID)
		{
		case BlockID.Cloud:
			if (head)
			{
				ScreenFader.SetFade(0.0f, 0.0f, 0.0f, 0.0f);
				SetShaderCulling(true);
				MapInteraction.AllowReticle();
			} break;

		case BlockID.Water:
			if (head)
			{
				ScreenFader.SetFade(0.0f, 0.0f, 0.0f, 0.0f);
				SetShaderCulling(true);
				MapInteraction.AllowReticle();
			} break;

		default:
			return;
		}
	}

	public bool IsFaceVisible(Block neighbor, int face)
	{
		switch (ID)
		{
		case BlockID.TallGrass:
			return true;

		case BlockID.Cloud:
			CullType cull = neighbor.GetCullType(face);

			if (cull == CullType.Solid || cull == CullType.Cutout)
				return false;

			if (cull == CullType.Transparent)
				return neighbor.IsFluid();

			return true;

		case BlockID.Leaves:
			if (neighbor.ID == BlockID.Leaves)
			{
				if (face == Direction.Left || face == Direction.Back || face == Direction.Down) 
					return false;

				return true;
			}

			goto default;

		default:
			return neighbor.GetCullType(face) != CullType.Solid;
		}
	}

	public bool CanDelete()
	{
		switch (ID)
		{
		case BlockID.Water:
			return data == FluidSimulator.MaxFluidLevel;

		default:
			return true;
		}
	}

	public bool CanPlace(Vector3i normal, int x, int y, int z)
	{
		switch (ID)
		{
		case BlockID.TallGrass:
			return !Map.GetBlockSafe(x, y - 1, z).IsTransparent();

		case BlockID.Ladder:
			if (normal == Vector3i.up || normal == Vector3i.down) 
				return false;
			
			BlockDirection = Direction.GetOpposite(normal.GetNormalDirection());

			switch (BlockDirection)
			{
			case Direction.Left:
				return Map.GetBlockSafe(x + 1, y, z).CanHaveAttachments();

			case Direction.Right:
				return Map.GetBlockSafe(x - 1, y, z).CanHaveAttachments();

			case Direction.Front:
				return Map.GetBlockSafe(x, y, z - 1).CanHaveAttachments();

			case Direction.Back:
				return Map.GetBlockSafe(x, y, z + 1).CanHaveAttachments();

			default:
				return false;
			}

		default:
			return true;
		}
	}
		
	public void OnPlace(Vector3i normal, int x, int y, int z)
	{
		switch (ID)
		{
		case BlockID.Water:
			FluidLevel = FluidSimulator.MaxFluidLevel;
			FluidSimulator.AddFluidAndFlow(x, y, z, this);
			break;

		case BlockID.Pumpkin:
		case BlockID.StoneSlope:
		case BlockID.GrassSlope:
			BlockDirection = Direction.GetOpposite(Player.GetRotation());
			break;

		default:
			return;
		}
	}

	public void OnDelete(int x, int y, int z)
	{
		switch (ID)
		{
		case BlockID.Water:
			if (Map.GetBlockSafe(x, y + 1, z).IsFluid())
			{
				Map.SetBlock(x, y, z, new Block(BlockID.Water, FluidSimulator.MaxFluidLevel));
				return;
			}
			else
			{
				FluidSimulator.RemoveFluidAndUnflow(x, y, z, this);
				break;
			}

		default:
			for (int i = 0; i < Vector3i.directions.Length; i++)
			{
				Vector3i dir = Vector3i.directions[i];

				int facing;

				Block block = Map.GetBlockSafe(x + dir.x, y + dir.y, z + dir.z);

				if (block.IsAttached(out facing))
				{
					if (dir.GetNormalDirection() == facing)
						Map.SetBlock(x + dir.x, y + dir.y, z + dir.z, new Block(BlockID.Air));
				}
			}

			FluidSimulator.TryFlowSurrounding(x, y, z);
			break;
		}
	}

	public bool IsAttached(out int dir)
	{
		switch (ID)
		{
		case BlockID.TallGrass:
			dir = Direction.Up;
			return true;

		case BlockID.Ladder:
			switch (BlockDirection)
			{
			case Direction.Left:
				dir = Direction.Left;
				return true;

			case Direction.Right:
				dir = Direction.Right;
				return true;

			case Direction.Front:
				dir = Direction.Front;
				return true;

			case Direction.Back:
				dir = Direction.Back;
				return true;

			default:
				dir = -1;
				return false;
			}

		default:
			dir = -1;
			return false;
		}
	}

	public bool CanHaveAttachments()
	{
		switch (ID)
		{
		case BlockID.TallGrass:
		case BlockID.Ladder:
		case BlockID.StoneSlope:
		case BlockID.GrassSlope:
			return false;

		default:
			return true;
		}
	}

	public MoveState TriggerState()
	{
		switch (ID)
		{
		case BlockID.Ladder:
			return MoveState.Climbing;

		default:
			return MoveState.Standard;
		}
	}

	public void SetCollision(BlockCollider collider, float x, float y, float z)
	{
		switch (ID)
		{
		case BlockID.Ladder:
			switch (BlockDirection)
			{
			case Direction.Left:
				collider.EnableBox(x + 0.45f, y, z, 0.1f, 1.0f, 1.0f);
				break;

			case Direction.Right:
				collider.EnableBox(x - 0.45f, y, z, 0.1f, 1.0f, 1.0f);
				break;

			case Direction.Front:
				collider.EnableBox(x, y, z - 0.45f, 1.0f, 1.0f, 0.1f);
				break;

			case Direction.Back:
				collider.EnableBox(x, y, z + 0.45f, 1.0f, 1.0f, 0.1f);
				break;
			} break;

		case BlockID.StoneSlope:
		case BlockID.GrassSlope:
			CollisionType colType = GetCollisionType();

			if (collider.Type != colType)
				collider.SetMesh(BuildCollider(new CollisionMeshData()), colType, x, y, z);
			else
				collider.EnableMesh(x, y, z);
			break;
			
		default:
			if (IsSolid()) collider.EnableBox(x, y, z, 1.0f, 1.0f, 1.0f);
			else collider.Disable();
			break;
		}
	}

	public void KillParticle(float y, ref ParticleSystem.Particle particle)
	{
		switch (ID)
		{
		default:
			particle.velocity = Vector3.zero;
			particle.position = new Vector3(particle.position.x, y + 0.5f, particle.position.z);
			particle.lifetime = 0;
			break;
		}
	}
}
