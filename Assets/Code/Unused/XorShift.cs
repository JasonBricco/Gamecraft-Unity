#if false

using System;
using System.Diagnostics;

public class XorShift
{
	private UInt32 x, y, z, w, v;
	private double randInv;

	public XorShift(int a, int b, int c, int d, int e)
	{
		randInv = 1.0 / ((double)UInt32.MaxValue + 2);
		Seed(a, b, c, d, e);
	}

	public void Seed(int a, int b, int c, int d, int e)
	{
		x = (UInt32)a;
		y = (UInt32)b;
		z = (UInt32)c;
		w = (UInt32)d;
		v = (UInt32)e;
	}

	private double Uniform()
	{
		double rand = (double)Rand() + 1;
		double value = rand * randInv;
		
		return value;
	}

	// Returns a random number between 0 and RandMax.
	public UInt32 Rand()
	{
		UInt32 t = (x ^ (x >> 7));
		
		x = y;
		y = z;
		z = w;
		w = v;
		
		v = (v ^ (v << 6)) ^ (t ^ (t << 13));
		
		UInt32 retVal = (y + y + 1) * v;
		
		return retVal;
	}

	public bool Bool(float chance)
	{
		return Uniform() <= chance;
	}

	public UInt32 RandChunkPos(int min, int max)
	{
		UInt32 result = Rand() & 15;

		while (result < min || result > max)
			result = Rand() & 15;

		return result;
	}
}

#endif
