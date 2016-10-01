using UnityEngine;

public class FlyingController : Controller 
{
	[SerializeField] private float speed = 12.0f;
	[SerializeField] private float verticalSpeed = 8.0f;
	private float thrust = 0.0f;

	private void Awake()
	{
		EventManager.OnCommand += (command, args) => 
		{
			if (command == CommandType.FlySpeed)
			{
				speed = Mathf.Clamp(int.Parse(args[1]), 0, 50);
				verticalSpeed = Mathf.Clamp(int.Parse(args[2]), 0, 50);
			}
		};
	}

	private void OnEnable()
	{
		thrust = 5.0f;
	}

	private void Update()
	{
		if (Engine.CurrentState != GameState.Playing) return;

		Vector3 moveVector = new Vector3(Input.GetAxis("FlyingX"), 0.0f, Input.GetAxis("FlyingY"));
		moveVector *= speed * Time.deltaTime;

		player.ProcessBlocksInside(player.GetSurroundingBlock(1, 1, 1), player.GetSurroundingBlock(1, 2, 1));

		thrust -= 8.0f * Time.deltaTime;
		thrust = Mathf.Max(thrust, 0.0f);

		moveVector.y += thrust * Time.deltaTime;

		if (thrust < 0.1f)
		{
			float ascend = Input.GetAxis("Ascend");
			float descend = Input.GetAxis("Descend");

			moveVector.y = (ascend + descend) * verticalSpeed * Time.deltaTime;
		}

		Move(moveVector);

		if ((controller.collisionFlags & CollisionFlags.Below) != 0)
			player.SetMovementState(MoveState.Standard);
	}
}
