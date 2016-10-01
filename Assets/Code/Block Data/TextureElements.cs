using UnityEngine;

public sealed class TextureElements 
{
	private float[] elements = new float[6];

	public void SetAll(float value)
	{
		for (int i = 0; i < elements.Length; i++)
			elements[i] = value;
	}

	public void SetFacingWithTop(float front, float top, float others)
	{
		SetAll(others);
		SetFront(front);
		SetTop(top);
	}

	public void SetLayered(float top, float sides, float bottom)
	{
		SetAll(sides);
		SetTop(top);
		SetBottom(bottom);
	}

	public void SetFront(float front)
	{
		elements[Direction.Front] = front;
	}

	public void SetBack(float back)
	{
		elements[Direction.Back] = back;
	}

	public void SetRight(float right)
	{
		elements[Direction.Right] = right;
	}

	public void SetLeft(float left)
	{
		elements[Direction.Left] = left;
	}

	public void SetTop(float top)
	{
		elements[Direction.Up] = top;
	}

	public void SetBottom(float bottom)
	{
		elements[Direction.Down] = bottom;
	}

	public float this[int index]
	{
		get { return elements[index]; }
	}
}
