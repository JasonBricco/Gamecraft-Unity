using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading;

public delegate void ThreadWork(object data);

public class ThreadManager : ScriptableObject, IUpdatable 
{
	private static Thread worker;

	private static AutoResetEvent handle = new AutoResetEvent(false);

	private static Queue<ThreadWork> work = new Queue<ThreadWork>(512);
	private static Queue<ThreadWork> priority = new Queue<ThreadWork>(512);
	private static Queue<UnityAction> mainThreadWork = new Queue<UnityAction>();

	private static bool runThread = true;

	public void Awake()
	{
		Updater.Register(this);

		ThreadStart start = new ThreadStart(WorkMonitor);
		worker = new Thread(start);
		worker.Start();
	}

	public void UpdateTick()
	{
		if (mainThreadWork.Count > 0)
			mainThreadWork.Dequeue().Invoke();

		if (work.Count > 0 || priority.Count > 0)
			handle.Set();
	}

	public static void QueueWork(ThreadWork task, bool isPriority)
	{
		if (!isPriority)
			work.Enqueue(task);
		else 
			priority.Enqueue(task);
	}

	public static void QueueForMainThread(UnityAction method)
	{
		mainThreadWork.Enqueue(method);
	}

	private static void WorkMonitor()
	{
		while (runThread)
		{
			while (work.Count > 0 || priority.Count > 0)
			{
				if (!runThread) break;

				if (priority.Count > 0)
					priority.Dequeue().Invoke(null);

				if (work.Count > 0)
					work.Dequeue().Invoke(null);
			}

			handle.WaitOne();
		}
	}

	private void OnDestroy()
	{
		runThread = false;
	}
}
