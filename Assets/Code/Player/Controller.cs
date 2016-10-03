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

		Vector3 pos = player.transform.position;

		if (pos.y < 0.0f || pos.y > 512.0f || pos.x < -50.0f || pos.x > Map.Size + 50.0f || pos.z < -50.0f || pos.z > Map.Size + 50.0f)
			player.Kill();
	}
}
