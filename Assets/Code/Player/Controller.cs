using UnityEngine;

public class Controller : MonoBehaviour 
{
	protected CharacterController controller;
	protected Player player;

	public void Initialize(CharacterController controller, Player player)
	{
		this.controller = controller;
		this.player = player;
	}

	protected void Move(Vector3 moveVector)
	{
		moveVector = transform.TransformDirection(moveVector);
		player.ColFlags = controller.Move(moveVector);

		Vector3 playerPos = player.transform.position;

		if (playerPos.y <= 0)
			player.transform.position = new Vector3(playerPos.x, 130, playerPos.z);
	}
}
