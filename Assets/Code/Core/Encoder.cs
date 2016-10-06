using UnityEngine;
using System.Collections.Generic;

public class Encoder
{
	public static void Encode(SerializableData data) 
	{ 
		int i = 0;
		int length = Map.GetBlockCount();

		int runCount = 0;
		Block currentBlock = new Block(BlockID.Air);

		for (i = 0; i < length; i++) 
		{
			Block nextBlock = Map.GetBlockDirect(i);
		
			if (nextBlock.ID != currentBlock.ID || nextBlock.data != currentBlock.data) 
			{
				if (i != 0) 
				{
					data.countList.Add(runCount);
					data.dataList.Add((ushort)((byte)currentBlock.ID << 8 | currentBlock.data));
				}

				runCount = 1;
				currentBlock = nextBlock;
			}
			else
				runCount++;

			if (i == length - 1) 
			{
				data.countList.Add(runCount);
				data.dataList.Add((ushort)((byte)currentBlock.ID << 8 | currentBlock.data));
			}
		}
	}

	public static void Decode(SerializableData data) 
	{
		int cur = 0;

		for (int run = 0; run < data.countList.Count; run++)
		{
			for (int i = 0; i < data.countList[run]; i++)
			{
				ushort block = data.dataList[run];
				Map.SetBlockDirect(cur, new Block((BlockID)(block >> 8), block));
				cur++;
			}
		}

		data.countList.Clear();
		data.dataList.Clear();
	}
}
