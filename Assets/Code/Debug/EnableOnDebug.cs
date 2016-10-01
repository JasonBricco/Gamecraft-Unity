using UnityEngine;

public class EnableOnDebug : MonoBehaviour 
{
	private void Awake()
	{
		this.gameObject.SetActive(Debug.isDebugBuild);
	}
}
