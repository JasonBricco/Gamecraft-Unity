using UnityEngine;
using System.Collections.Generic;

public class Encoder
{
	public static void Encode(SerializableData data) 
	{ 
		int i = 0;
		int length = Map.GetBlockCount();

		int currentCount = 0;
		Block currentData = new Block(BlockID.Air);

		for (i = 0; i < length; i++) 
		{
			Block thisData = Map.GetBlockDirect(i);

			if (thisData.ID != currentData.ID) 
			{
				if (i != 0) 
				{
					data.countList.Add(currentCount);
					data.idList.Add((byte)currentData.ID);
					data.dataList.Add(currentData.data);
				}

				currentCount = 1;
				currentData = thisData;
			}
			else
				currentCount++;

			if (i == length - 1) 
			{
				data.countList.Add(currentCount);
				data.idList.Add((byte)currentData.ID);
				data.dataList.Add(currentData.data);
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
				Map.SetBlockDirect(cur, new Block((BlockID)data.idList[run], data.dataList[run]));
				cur++;
			}
		}

		data.countList.Clear();
		data.dataList.Clear();
	}
}
