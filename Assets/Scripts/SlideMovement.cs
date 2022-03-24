using UnityEngine;

public class SlideMovement : MonoBehaviour
{
	public float launchSpeed;

	public float tiltAngle;

	public float tiltSmooth;

	public Transform crouchPosition;

	private Vector3 slideDir;

	public bool isSliding;

	private Rigidbody rb;

	public CapsuleCollider innerCol;

	public CapsuleCollider outerCol;

	private PlayerMovement movementScript;

	public GameObject cam;

	private Quaternion initialRotation;

	private Quaternion tiltRotation;

	private bool angleSet;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		movementScript = GetComponent<PlayerMovement>();
		initialRotation = cam.transform.localRotation;
	}

	private void Update()
	{
		Slide();
		if (isSliding)
		{
			Tilt();
		}
		else
		{
			ResetTilt();
		}
	}

	private void Slide()
	{
		if (Input.GetKey(KeyCode.LeftControl))
		{
			StartSlide();
		}
		else
		{
			FinishSlide();
		}
	}

	private void FixedUpdate()
	{
		Sliding();
	}

	private void StartSlide()
	{
		if (!isSliding)
		{
			slideDir = base.transform.forward;
			innerCol.height /= 2f;
			outerCol.height /= 2f;
			base.transform.position = crouchPosition.position;
			rb.AddForce(slideDir * launchSpeed * 1.5f);
			isSliding = true;
			//AudioManager.instance.Play("sliding");
		}
	}

	private void Sliding()
	{
		if (movementScript.grounded && isSliding)
		{
			Vector3 to = Vector3.zero;
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position, Vector3.down, out hitInfo, innerCol.height / 2f * 3f))
			{
				to = hitInfo.normal;
			}
			float num = Vector3.Angle(Vector3.up, to);
			rb.AddForce(slideDir * num / 5f);
			rb.AddForce(Vector3.down * 10f);
		}
	}

	private void FinishSlide()
	{
		if (isSliding)
		{
			innerCol.height *= 2f;
			outerCol.height *= 2f;
			isSliding = false;
			slideDir = Vector3.zero;
			//AudioManager.instance.Stop("sliding");
		}
	}

	private void changeHeight(float newHeight)
	{
		Vector3 localScale = base.transform.localScale;
		localScale.y = newHeight;
		base.transform.localScale = localScale;
	}

	private void Tilt()
	{
		float x = movementScript.CurVelocityRelativeToLook().x;
		if (!angleSet)
		{
			if (x < 0f)
			{
				tiltRotation = Quaternion.Euler(0f, 0f, 0f - tiltAngle);
			}
			else
			{
				tiltRotation = Quaternion.Euler(0f, 0f, tiltAngle);
			}
			angleSet = true;
		}
		cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, tiltRotation, Time.deltaTime * tiltSmooth);
	}

	private void ResetTilt()
	{
		cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, initialRotation, Time.deltaTime * tiltSmooth);
		angleSet = false;
	}
}
