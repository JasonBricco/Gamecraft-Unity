using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class FluidSimulator : ScriptableObject, IUpdatable
{
	public const int MinFluidLevel = 1;
	public const int MaxFluidLevel = 5;

	private static Queue<BlockInstance> flowQueue = new Queue<BlockInstance>(128);
	private static Queue<BlockInstance> unflowQueue = new Queue<BlockInstance>(128);

	private static List<BlockInstance> blocksToFlow = new List<BlockInstance>(128);
	private static List<BlockInstance> blocksToUnflow = new List<BlockInstance>(128);

	private float tickTime = 0.0f;

	private void Awake()
	{
		Updater.Register(this);
	}

	public void UpdateTick()
	{
		float tickSpeed = Settings.TickSpeed;
		tickTime += Time.deltaTime;

		if (tickTime >= tickSpeed)
		{
			UpdateBlocks();
			tickTime -= tickSpeed;
		}
	}
		
	private void UpdateBlocks()
	{
		while (flowQueue.Count > 0)
			FlowFluid(flowQueue.Dequeue());

		while (unflowQueue.Count > 0)
			UnflowFluid(unflowQueue.Dequeue());

		if (blocksToFlow.Count > 0)
			QueueBlocksForFlowing();

		if (blocksToUnflow.Count > 0)
			QueueBlocksForUnflowing();
	}
		
	public static void AddFluidAndFlow(int x, int y, int z, Block block)
	{
		flowQueue.Enqueue(new BlockInstance(block, x, y, z));
	}
		
	public static void RemoveFluidAndUnflow(int x, int y, int z, Block block)
	{
		unflowQueue.Enqueue(new BlockInstance(block, x, y, z));
	}
		
	public static float GetOffset(int level)
	{
		return -0.5f + (level * 0.2f);
	}
		
	private static void FlowFluid(BlockInstance blockInst)
	{
		Index curIndex = new Index(blockInst.x, blockInst.y, blockInst.z);
		int fluidLevel = blockInst.block.FluidLevel;

		// Flowing fluid reduces the fluid level by 1. Add 1 to MaxFluidLevel so that when we flow fluid 
		// downward, it remains at the max level.
		bool success = FlowIfPossible(blockInst.x, blockInst.y - 1, blockInst.z, curIndex, MaxFluidLevel + 1, true);
		if (success) return;

		if (fluidLevel <= MinFluidLevel) return;

		FlowIfPossible(blockInst.x - 1, blockInst.y, blockInst.z, curIndex, fluidLevel);
		FlowIfPossible(blockInst.x + 1, blockInst.y, blockInst.z, curIndex, fluidLevel);
		FlowIfPossible(blockInst.x, blockInst.y, blockInst.z - 1, curIndex, fluidLevel);
		FlowIfPossible(blockInst.x, blockInst.y, blockInst.z + 1, curIndex, fluidLevel);
	}

	private static void UnflowFluid(BlockInstance blockInst)
	{
		Index curIndex = new Index(blockInst.x, blockInst.y, blockInst.z);
		int fluidLevel = blockInst.block.FluidLevel;

		if (fluidLevel <= MinFluidLevel) return;

		UnflowIfPossible(blockInst.x, blockInst.y - 1, blockInst.z, curIndex, fluidLevel + 1);
		UnflowIfPossible(blockInst.x - 1, blockInst.y, blockInst.z, curIndex, fluidLevel);
		UnflowIfPossible(blockInst.x + 1, blockInst.y, blockInst.z, curIndex, fluidLevel);
		UnflowIfPossible(blockInst.x, blockInst.y, blockInst.z - 1, curIndex, fluidLevel);
		UnflowIfPossible(blockInst.x, blockInst.y, blockInst.z + 1, curIndex, fluidLevel);
	}
		
	private static bool FlowIfPossible(int nX, int nY, int nZ, Index curIndex, int fluidLevel, bool down = false)
	{
		if (!Map.IsInMap(nX, nY, nZ)) return false;

		Block neighbor = Map.GetBlock(nX, nY, nZ);
		bool canFlow = false;

		if (down)
		{
			if (neighbor.IsFluid())
			{
				if (neighbor.FluidLevel < MaxFluidLevel)
					canFlow = true;
				else return true;
			}
			else canFlow = neighbor.AllowOverwrite();
		}
		else canFlow = neighbor.IsFluid() ? neighbor.FluidLevel < fluidLevel - 1 : neighbor.AllowOverwrite();

		if (canFlow)
		{
			Block newFluid = new Block(BlockID.Water, fluidLevel - 1);
			Map.SetBlock(nX, nY, nZ, newFluid);
			blocksToFlow.Add(new BlockInstance(newFluid, nX, nY, nZ));
			return true;
		}

		return false;
	}
		
	private static void UnflowIfPossible(int nX, int nY, int nZ, Index curIndex, int fluidLevel)
	{
		if (Map.IsInMap(nX, nY, nZ))
		{
			Block neighbor = Map.GetBlock(nX, nY, nZ);

			if (neighbor.IsFluid())
			{
				if (neighbor.FluidLevel < fluidLevel)
				{
					blocksToUnflow.Add(new BlockInstance(neighbor, nX, nY, nZ));
					Map.SetBlock(nX, nY, nZ, new Block(BlockID.Air));
					TryFlowSurrounding(nX, nY, nZ);
				}
			}
		}
	}
		
	public static void TryFlowSurrounding(int x, int y, int z)
	{
		Index index = new Index(x, y, z);

		Block leftBlock = Map.GetBlockSafe(x - 1, y, z);
		Block rightBlock = Map.GetBlockSafe(x + 1, y, z);
		Block backBlock = Map.GetBlockSafe(x, y, z - 1);
		Block frontBlock = Map.GetBlockSafe(x, y, z + 1);
		Block topBlock = Map.GetBlock(x, y + 1, z);

		if (leftBlock.IsFluid() && leftBlock.FluidLevel > MinFluidLevel) 
			FlowIfPossible(x, y, z, index, leftBlock.FluidLevel);
		
		if (rightBlock.IsFluid() && rightBlock.FluidLevel > MinFluidLevel) 
			FlowIfPossible(x, y, z, index, rightBlock.FluidLevel);
		
		if (backBlock.IsFluid() && backBlock.FluidLevel > MinFluidLevel) 
			FlowIfPossible(x, y, z, index, backBlock.FluidLevel);
		
		if (frontBlock.IsFluid() && frontBlock.FluidLevel > MinFluidLevel) 
			FlowIfPossible(x, y, z, index, frontBlock.FluidLevel);
		
		if (topBlock.IsFluid() && topBlock.FluidLevel > MinFluidLevel) 
			FlowIfPossible(x, y, z, index, topBlock.FluidLevel);
	}
		
	private void QueueBlocksForFlowing()
	{
		for (int i = 0; i < blocksToFlow.Count; i++)
		{
			BlockInstance b = blocksToFlow[i];
			MapLight.RecomputeLighting(b.x, b.y, b.z);
			ChunkManager.FlagChunkForUpdate(b.x, b.z);

			flowQueue.Enqueue(blocksToFlow[i]);
		}

		ChunkManager.UpdateMeshes();
		blocksToFlow = new List<BlockInstance>(128);
	}
		
	private void QueueBlocksForUnflowing()
	{
		for (int i = 0; i < blocksToUnflow.Count; i++)
		{
			BlockInstance b = blocksToUnflow[i];
			MapLight.RecomputeLighting(b.x, b.y, b.z);
			ChunkManager.FlagChunkForUpdate(b.x, b.z);

			unflowQueue.Enqueue(blocksToUnflow[i]);
		}

		ChunkManager.UpdateMeshes();
		blocksToUnflow = new List<BlockInstance>(128);
	}
}
