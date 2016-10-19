using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public delegate void ThreadWork(object data);

public sealed class ThreadManager : ScriptableObject, IUpdatable 
{
	private static Queue<UnityAction> mainThreadWork = new Queue<UnityAction>();
	private static WorkerThread[] threads;

	private static int next = 0;

	public void Awake()
	{
		Updater.Register(this);

		threads = new WorkerThread[Environment.ProcessorCount];

		Logger.Print("Creating " + threads.Length + " threads.");

		for (int i = 0; i < threads.Length; i++)
			threads[i] = new WorkerThread();
	}

	public void UpdateTick()
	{
		if (mainThreadWork.Count > 0)
			mainThreadWork.Dequeue().Invoke();

		for (int i = 0; i < threads.Length; i++)
			threads[i].TrySetHandle();
	}

	public static void QueueWork(ThreadWork task, object arg, bool isPriority)
	{
		next = (next + 1) % threads.Length;
		threads[next].QueueWork(task, arg, isPriority);
	}

	public static void QueueForMainThread(UnityAction method)
	{
		mainThreadWork.Enqueue(method);
	}

	private void OnDestroy()
	{
		for (int i = 0; i < threads.Length; i++)
			threads[i].Stop();
	}
}
