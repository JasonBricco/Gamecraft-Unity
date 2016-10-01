﻿using UnityEngine;
using System.Collections.Generic;

public class Encoder
{
	public static void Encode(SerializableData data) 
	{ 
		int i = 0;
		int length = Map.GetBlockCount();

		int currentCount = 0;
		ushort currentData = 0;

		for (i = 0; i < length; i++) 
		{
			ushort thisData = Map.GetBlockDirect(i);

			if (thisData != currentData) 
			{
				if (i != 0) 
				{
					data.countList.Add(currentCount);
					data.dataList.Add(currentData);
				}

				currentCount = 1;
				currentData = thisData;
			}
			else
				currentCount++;

			if (i == length - 1) 
			{
				data.countList.Add(currentCount);
				data.dataList.Add(currentData);
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
				Map.SetBlockDirect(cur, data.dataList[run]);
				cur++;
			}
		}

		data.countList.Clear();
		data.dataList.Clear();
	}
}