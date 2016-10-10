using UnityEngine;
using System.Collections.Generic;

public class UndoStack<T>
{
	private List<T> items = new List<T>();
	private int limit;

	public UndoStack(int limit)
	{
		this.limit = limit;
	}

	public int Count
	{
		get { return items.Count; }
	}

	public void Push(T item)
	{
		items.Add(item);

		if (items.Count > limit)
			items.RemoveAt(0);
	}

	public T Pop()
	{
		if (items.Count > 0)
		{
			T temp = items[items.Count - 1];
			items.RemoveAt(items.Count - 1);
			return temp;
		}
		else
			return default(T);
	}

	public void Clear()
	{
		items.Clear();
	}
}
