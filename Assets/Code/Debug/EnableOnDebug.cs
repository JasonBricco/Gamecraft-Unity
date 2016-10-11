using UnityEngine;

public sealed class EnableOnDebug : MonoBehaviour 
{
	private void Awake()
	{
		this.gameObject.SetActive(Debug.isDebugBuild);
	}
}
