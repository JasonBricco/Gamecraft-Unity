using UnityEngine;

public sealed class CameraControl : MonoBehaviour 
{
	[SerializeField] private enum RotationAxis { X, Y }
	[SerializeField] private RotationAxis axis = RotationAxis.X;

	[SerializeField] private float minimumY = -90.0f;
	[SerializeField] private float maximumY = 90.0f;

	private Vector2 angles = Vector2.zero;

	private void Start()
	{
		angles = transform.eulerAngles;
	}

	private void LateUpdate()
	{
		if (Engine.CurrentState != GameState.Playing) return;

		float x = Input.GetAxis("Mouse X");
		float y = -Input.GetAxis("Mouse Y");
		Vector2 delta = new Vector2(y, x);
		
		angles += delta * Settings.CameraSensitivity;
		angles.x = Mathf.Clamp(angles.x, minimumY, maximumY);
		
		if (axis == RotationAxis.Y) angles.x = transform.localEulerAngles.x;
		if (axis == RotationAxis.X) angles.y = transform.localEulerAngles.y;
		
		Quaternion targetRotation = Quaternion.Euler(angles);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 25 * Time.deltaTime);
	}
}
