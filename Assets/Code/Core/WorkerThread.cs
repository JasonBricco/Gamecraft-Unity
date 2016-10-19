using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public sealed class WorkerThread 
{
	private AutoResetEvent handle = new AutoResetEvent(false);

	private Queue<ThreadWork> work = new Queue<ThreadWork>();
	private Queue<ThreadWork> priority = new Queue<ThreadWork>();

	private Queue<object> data = new Queue<object>();

	private bool run = true;

	public WorkerThread()
	{
		ThreadStart start = new ThreadStart(WorkMonitor);
		Thread worker = new Thread(start);
		worker.Start();
	}

	public void QueueWork(ThreadWork task, object arg, bool isPriority)
	{
		if (!isPriority)
			work.Enqueue(task);
		else priority.Enqueue(task);

		data.Enqueue(arg);
	}

	public void TrySetHandle()
	{
		if (work.Count > 0 || priority.Count > 0)
			handle.Set();
	}

	private void WorkMonitor()
	{
		while (run)
		{
			while (work.Count > 0 || priority.Count > 0)
			{
				if (!run) break;

				if (priority.Count > 0) priority.Dequeue().Invoke(data.Dequeue());
				if (work.Count > 0) work.Dequeue().Invoke(data.Dequeue());
			}

			handle.WaitOne();
		}
	}

	public void Stop()
	{
		run = false;
	}
}
