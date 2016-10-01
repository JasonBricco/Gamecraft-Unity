using UnityEngine;

public class StandardController : Controller 
{
	[SerializeField] private float speed = 6.0f;
	private float gravity = 5.0f;
	private float jumpSpeed = 0.0f;
	private float gravModifier = 1.0f;
	private float canFly = 0.5f;
	private float swimUpForce = 0;
	private float waterForce = 1.0f;

	private void Awake()
	{
		EventManager.OnCommand += (command, args) => 
		{
			if (command == CommandType.WalkSpeed)
				speed = Mathf.Clamp(int.Parse(args[1]), 0, 50);
		};
	}

	private void Update()
	{
		if (Engine.CurrentState != GameState.Playing) return;

		canFly -= Time.deltaTime;
		canFly = Mathf.Max(canFly, 0.0f);

		Vector3 move = new Vector3(Input.GetAxis("StandardX"), 0.0f, Input.GetAxis("StandardY"));
		move *= speed * Time.deltaTime;

		ushort legsBlock = player.GetSurroundingBlock(1, 1, 1);
		ushort headBlock = player.GetSurroundingBlock(1, 2, 1);

		player.ProcessBlocksInside(legsBlock, headBlock);

		bool legsInWater = BlockRegistry.GetBlock(legsBlock).IsFluid;
		bool headInWater = BlockRegistry.GetBlock(headBlock).IsFluid;

		if (legsInWater || headInWater)
		{
			waterForce -= 5 * Time.deltaTime;
			waterForce = Mathf.Max(waterForce, 0.5f);

			gravModifier -= 5 * Time.deltaTime;
			gravModifier = Mathf.Max(gravModifier, 1.0f);

			move.x *= waterForce;
			move.z *= waterForce;
		}

		if (!legsInWater && headInWater)
		{
			swimUpForce = 0;
			
			if (controller.isGrounded && Input.GetKey(KeyCode.Space))
				jumpSpeed = 10.3f;
		}

		if (legsInWater && !headInWater)
		{
			swimUpForce = 0;
			
			if (waterForce < 0.52f && Input.GetKey(KeyCode.Space))
				jumpSpeed = 10.3f;
		}

		if (legsInWater && headInWater)
		{
			if (waterForce < 0.52f && Input.GetKey(KeyCode.Space))
			{
				if (swimUpForce < gravity * Time.deltaTime * 2.5f)
					swimUpForce += gravity * Time.deltaTime * 0.1f;
			}
			else
			{
				swimUpForce -= gravity * Time.deltaTime * 0.1f;
				
				if (swimUpForce < 0)
					swimUpForce = 0;
			}
			
			move.y += swimUpForce;
		}

		jumpSpeed -= gravity * gravModifier * Time.deltaTime;
		jumpSpeed = Mathf.Max(jumpSpeed, 0);
		move.y += jumpSpeed * Time.deltaTime;

		if (!legsInWater && !headInWater)
		{
			swimUpForce = 0;
			waterForce = 1.0f;

			gravModifier += 1.0f * Time.deltaTime;
			gravModifier = Mathf.Min(gravModifier, 10.0f);
			
			if (controller.isGrounded)
			{
				jumpSpeed = 0;
				gravModifier = 1.00f;

				if (Input.GetKey(KeyCode.Space))
				{
					canFly = 0.5f;
					jumpSpeed = 10.3f;
				}
			}
			else
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					if (canFly > 0.0f)
					{
						player.SetMovementState(MoveState.Flying);
						return;
					}
					
					canFly = 0.5f;
				}
			}
		}

		move.y -= gravity * gravModifier * Time.deltaTime;
		move.y *= waterForce;
		Move(move);
	}

	private void OnDisable()
	{
		jumpSpeed = 0.0f;
		gravModifier = 1.0f;
	}
}
