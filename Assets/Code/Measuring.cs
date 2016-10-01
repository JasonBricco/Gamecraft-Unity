using UnityEngine;
using System.Collections.Generic;

public class Measuring : MonoBehaviour 
{
	private void Awake()
	{
		System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		// Code to test.

		watch.Stop();
		Debug.Log("Milliseconds: " + watch.ElapsedMilliseconds);
		Debug.Log("Milliseconds: " + watch.ElapsedTicks);

		watch.Reset();
		watch.Start();

		// Code to test.

		watch.Stop();
		Debug.Log("Milliseconds: " + watch.ElapsedMilliseconds);
		Debug.Log("Milliseconds: " + watch.ElapsedTicks);
	}
}
