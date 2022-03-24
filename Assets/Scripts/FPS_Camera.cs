using UnityEngine;

public class FPS_Camera : MonoBehaviour
{
	public float sensitivity = 250f;

	public Transform playerBody;

	private float xRotation;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		Look();
	}

	private void Look()
	{
		float num = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
		float num2 = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
		xRotation -= num2;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);
		base.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
		playerBody.Rotate(Vector3.up * num);
	}
}
