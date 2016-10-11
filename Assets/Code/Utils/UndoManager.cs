using UnityEngine;
using System.Collections.Generic;

public sealed class Modification
{
	public List<BlockInstance> prevBlocks = new List<BlockInstance>();
	public List<BlockInstance> newBlocks = new List<BlockInstance>();

	public Modification(List<BlockInstance> prevBlocks, List<BlockInstance> newBlocks)
	{
		this.prevBlocks = prevBlocks;
		this.newBlocks = newBlocks;
	}
}

public sealed class UndoManager 
{
	private static UndoStack<Modification> undoStack = new UndoStack<Modification>(5);
	private static UndoStack<Modification> redoStack = new UndoStack<Modification>(5);

	public static void RegisterAction(BlockInstance prevBlock, BlockInstance newBlock)
	{
		List<BlockInstance> prevBlocks = new List<BlockInstance>();
		List<BlockInstance> newBlocks = new List<BlockInstance>();

		prevBlocks.Add(prevBlock);
		newBlocks.Add(newBlock);

		Modification mod = new Modification(prevBlocks, newBlocks);
		undoStack.Push(mod);

		redoStack.Clear();
	}

	public static void RegisterAction(List<BlockInstance> prevBlocks, List<BlockInstance> newBlocks)
	{
		Modification mod = new Modification(prevBlocks, newBlocks);
		undoStack.Push(mod);

		redoStack.Clear();
	}

	public static void Undo()
	{
		if (undoStack.Count > 0)
		{
			Modification mod = undoStack.Pop();
			Map.SetBlocksAdvanced(mod.prevBlocks, false);
			redoStack.Push(mod);
		}
	}

	public static void Redo()
	{
		if (redoStack.Count > 0)
		{
			Modification mod = redoStack.Pop();
			Map.SetBlocksAdvanced(mod.newBlocks, false);
			undoStack.Push(mod);
		}
	}
}
