using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
	
public class FluidSimulator : ScriptableObject, IUpdatable
{
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
		
	public static void AddFluidAndFlow(int x, int y, int z, ushort block)
	{
		flowQueue.Enqueue(new BlockInstance(block, x, y, z));
	}
		
	public static void RemoveFluidAndUnflow(int x, int y, int z, ushort block)
	{
		unflowQueue.Enqueue(new BlockInstance(block, x, y, z));
	}
		
	public static float GetOffset(int level)
	{
		return -0.5f + (level * 0.2f);
	}
		
	private static void FlowFluid(BlockInstance block)
	{
		Index curIndex = new Index(block.x, block.y, block.z);
		int fluidLevel = BlockRegistry.GetBlock(block.ID).FluidLevel;

		bool success = FlowIfPossible(block.x, block.y - 1, block.z, curIndex, 5);
		if (success) return;

		if (fluidLevel == 1) return;

		FlowIfPossible(block.x - 1, block.y, block.z, curIndex, fluidLevel);
		FlowIfPossible(block.x + 1, block.y, block.z, curIndex, fluidLevel);
		FlowIfPossible(block.x, block.y, block.z - 1, curIndex, fluidLevel);
		FlowIfPossible(block.x, block.y, block.z + 1, curIndex, fluidLevel);
	}

	private static void UnflowFluid(BlockInstance block)
	{
		Index curIndex = new Index(block.x, block.y, block.z);
		int fluidLevel = BlockRegistry.GetBlock(block.ID).FluidLevel;

		if (fluidLevel == 1) return;

		UnflowIfPossible(block.x, block.y - 1, block.z, curIndex, fluidLevel + 1);
		UnflowIfPossible(block.x - 1, block.y, block.z, curIndex, fluidLevel);
		UnflowIfPossible(block.x + 1, block.y, block.z, curIndex, fluidLevel);
		UnflowIfPossible(block.x, block.y, block.z - 1, curIndex, fluidLevel);
		UnflowIfPossible(block.x, block.y, block.z + 1, curIndex, fluidLevel);
	}
		
	private static bool FlowIfPossible(int nX, int nY, int nZ, Index curIndex, int fluidLevel)
	{
		if (!Map.IsInMap(nX, nY, nZ)) return false;

		ushort nID = Map.GetBlock(nX, nY, nZ);
		bool canFlow = false;

		Block nBlock = BlockRegistry.GetBlock(nID);

		if (nBlock.FluidLevel == 5) return true;
		canFlow = nBlock.IsFluid ? nBlock.FluidLevel < fluidLevel - 1 : nBlock.CanFlowOver();

		if (canFlow)
		{
			ushort newID = (ushort)(fluidLevel + 3);
			Map.SetBlock(nX, nY, nZ, newID);
			blocksToFlow.Add(new BlockInstance(newID, nX, nY, nZ));
			return true;
		}

		return false;
	}
		
	private static void UnflowIfPossible(int nX, int nY, int nZ, Index curIndex, int fluidLevel)
	{
		if (Map.IsInMap(nX, nY, nZ))
		{
			ushort nID = Map.GetBlock(nX, nY, nZ);
			Block nBlock = BlockRegistry.GetBlock(nID);

			if (nBlock.IsFluid)
			{
				if (nBlock.FluidLevel < fluidLevel)
				{
					blocksToUnflow.Add(new BlockInstance(nID, nX, nY, nZ));
					Map.SetBlock(nX, nY, nZ, BlockType.Air);
					TryFlowSurrounding(nX, nY, nZ);
				}
			}
		}
	}
		
	public static void TryFlowSurrounding(int x, int y, int z)
	{
		Index index = new Index(x, y, z);

		Block leftBlock = BlockRegistry.GetBlock(Map.GetBlockSafe(x - 1, y, z));
		Block rightBlock = BlockRegistry.GetBlock(Map.GetBlockSafe(x + 1, y, z));
		Block backBlock = BlockRegistry.GetBlock(Map.GetBlockSafe(x, y, z - 1));
		Block frontBlock = BlockRegistry.GetBlock(Map.GetBlockSafe(x, y, z + 1));
		Block topBlock = BlockRegistry.GetBlock(Map.GetBlock(x, y + 1, z));

		if (leftBlock.IsFluid) FlowIfPossible(x, y, z, index, leftBlock.FluidLevel);
		if (rightBlock.IsFluid) FlowIfPossible(x, y, z, index, rightBlock.FluidLevel);
		if (backBlock.IsFluid) FlowIfPossible(x, y, z, index, backBlock.FluidLevel);
		if (frontBlock.IsFluid) FlowIfPossible(x, y, z, index, frontBlock.FluidLevel);
		if (topBlock.IsFluid) FlowIfPossible(x, y, z, index, topBlock.FluidLevel);
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
