using UnityEngine;
using UnityEngine.Events;

public class BarrierAdjust : MonoBehaviour 
{
	[SerializeField] private UnityEvent barrierEvent;

	private void Awake()
	{
		barrierEvent.Invoke();
	}

	public void HandleLeft()
	{
		GetComponent<BoxCollider>().center = new Vector3(-1, 128, (Map.Size / 2));
		GetComponent<BoxCollider>().size = new Vector3(1, 256, Map.Size + Chunk.Size);
	}

	public void HandleRight()
	{
		GetComponent<BoxCollider>().center = new Vector3(Map.Size, 128, (Map.Size / 2));
		GetComponent<BoxCollider>().size = new Vector3(1, 256, Map.Size + Chunk.Size);
	}

	public void HandleBack()
	{
		GetComponent<BoxCollider>().center = new Vector3((Map.Size / 2), 128, -1);
		GetComponent<BoxCollider>().size = new Vector3(Map.Size + Chunk.Size, 256, 1);
	}

	public void HandleFront()
	{
		GetComponent<BoxCollider>().center = new Vector3((Map.Size / 2), 128, Map.Size);
		GetComponent<BoxCollider>().size = new Vector3(Map.Size + Chunk.Size, 256, 1);
	}

	public void HandleTop()
	{
		int n = (Map.Size / 2);
		GetComponent<BoxCollider>().center = new Vector3(n, 256, n);
		GetComponent<BoxCollider>().size = new Vector3(Map.Size, 1, Map.Size);
	}
}
