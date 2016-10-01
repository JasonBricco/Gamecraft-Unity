using UnityEngine;
using UnityEngine.UI;

public class BlockUIData : MonoBehaviour
{
	[SerializeField] private PlayerInteraction interactionClass;
	[SerializeField] private int blockID;

	public PlayerInteraction InteractionClass
	{
		get { return interactionClass; }
	}

	public int BlockID
	{
		get { return blockID; }
	}
}
