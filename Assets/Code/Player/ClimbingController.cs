using UnityEngine;

public class ClimbingController : Controller 
{
	[SerializeField] private float speed = 3.0f;

	private void Update()
	{
		if (Engine.CurrentState != GameState.Playing) return;

		ushort legsBlock = player.GetSurroundingBlock(1, 1, 1);
		ushort headBlock = player.GetSurroundingBlock(1, 2, 1);

		player.ProcessBlocksInside(legsBlock, headBlock);

		int belowBlockDir = BlockRegistry.GetBlock(player.GetSurroundingBlock(1, 0, 1)).BlockDirection;
		int legsBlockDir = BlockRegistry.GetBlock(legsBlock).BlockDirection;
		int headBlockDir = BlockRegistry.GetBlock(headBlock).BlockDirection;
		int playerDir = Player.GetRotation();

		Vector3 move = Vector3.zero;

		if (Direction.IsOpposite(playerDir, legsBlockDir) || Direction.IsOpposite(playerDir, headBlockDir) || 
		    Direction.IsOpposite(playerDir, belowBlockDir))
			move = new Vector3(Input.GetAxis("StandardX"), Input.GetAxis("StandardY"), 0.0f);
		else
			move = new Vector3(Input.GetAxis("StandardX"), 0.0f, Input.GetAxis("StandardY"));

		move *= speed * Time.deltaTime;

		if (Input.GetKey(KeyCode.Space))
			move.y *= 2.0f;
		
		if (controller.isGrounded)
		{
			if (Input.GetAxis("StandardY") == -1.0f)
			{
				player.SetMovementState(MoveState.Standard);
				return;
			}
		}
		else
		{
			bool endClimb = true;

			for (int i = 0; i < 3; i++)
			{
				if (BlockRegistry.GetBlock(player.GetSurroundingBlock(1, i, 1)).MoveState == MoveState.Climbing)
					endClimb = false;
			}

			if (endClimb)
			{
				player.SetMovementState(MoveState.Standard);
				return;
			}
		}

		Move(move);
	}
}
