using UnityEngine;
using UnityEngine.UI;

// Used to pass data about a block selection from the UI to the game code.
public sealed class BlockUIData : MonoBehaviour
{
	public MapInteraction interactionClass;
	public int blockID;
}
