using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Properties")]
	public float moveSpeed = 10f;

	public float maxSpeed;

	public float jumpStrength = 10f;

	private float numOfJumps = 2f;

	public float dashSpeed;

	public float dashDuration = 0.15f;

	public float turnSpeed;

	public float extraGravity;

	[Header("WallRunning")]
	public float tiltAngle;

	public float tiltSmooth;

	public float wallrunForce;

	public float wallHopForce;

	public float speedMultiplier = 1.3f;

	private bool isWallRight;

	private bool isWallLeft;

	private bool isWallFront;

	private bool isWallBack;

	private bool isWallRunning;

	private bool touchingWall;

	private Quaternion initialRotation;

	[Header("Drag")]
	public float AIR_DRAG;

	public float COUNTER_DRAG;

	public float SLIDING_DRAG;

	[Header("GroundCheck")]
	public Transform groundCheck;

	public float checkRadius;

	public LayerMask whatIsGround;

	public bool grounded;

	[Header("E.t.c.")]
	//public ParticleSystem dashParticles;

	//public Animator anim;

	public GameObject cam;

	private SlideMovement slideScript;

	private bool isSliding;

	//private VisualEffects visualFX;

	private Vector2 vel;

	private Rigidbody rb;

	[HideInInspector]
	public bool walking;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		slideScript = GetComponent<SlideMovement>();
		//visualFX = GetComponent<VisualEffects>();
		initialRotation = cam.transform.localRotation;
	}

	private void Update()
	{
		grounded = Physics.CheckSphere(groundCheck.position, checkRadius, whatIsGround);
		isSliding = slideScript.isSliding;
		if (isWallRunning && isWallRight)
		{
			TiltCamera(1f);
		}
		else if (isWallRunning && isWallLeft)
		{
			TiltCamera(-1f);
		}
		else
		{
			ResetTilt();
		}
		if (Input.GetButtonDown("Jump"))
		{
			Jump();
		}
		if (grounded && numOfJumps < 2f)
		{
			numOfJumps = 2f;
		}
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			StartCoroutine(Dash());
		}
		float axisRaw = Input.GetAxisRaw("Horizontal");
		float axisRaw2 = Input.GetAxisRaw("Vertical");
		if (grounded && (axisRaw != 0f || axisRaw2 != 0f))
		{
			walking = true;
		}
		else
		{
			walking = false;
		}
	}

	private void FixedUpdate()
	{
		rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
		CheckForWall();
		if (!grounded && touchingWall && !isWallRunning)
		{
			StartWallRun();
		}
		WallRun();
		vel = CurVelocityRelativeToLook();
		Move();
	}

	private void Move()
	{
		float num = Input.GetAxisRaw("Horizontal");
		float num2 = Input.GetAxisRaw("Vertical");
		float x = vel.x;
		float y = vel.y;
		if (num > 0f && x > maxSpeed)
		{
			num = 0f;
		}
		if (num < 0f && x < 0f - maxSpeed)
		{
			num = 0f;
		}
		if (num2 > 0f && y > maxSpeed)
		{
			num2 = 0f;
		}
		if (num2 < 0f && y < 0f - maxSpeed)
		{
			num2 = 0f;
		}
		Vector2 vector = new Vector2(rb.velocity.x, rb.velocity.z);
		if (Mathf.Abs(vector.magnitude) > maxSpeed || isSliding)
		{
			num = 0f;
			num2 = 0f;
		}
		CounterMovement(num, num2);
		rb.AddForce(base.transform.forward * num2 * moveSpeed);
		rb.AddForce(base.transform.right * num * moveSpeed);
		if (y > 0.2f && num2 < 0f)
		{
			rb.AddForce(-base.transform.forward * turnSpeed);
		}
		if (y < -0.2f && num2 > 0f)
		{
			rb.AddForce(base.transform.forward * turnSpeed);
		}
		if (x > 0.2f && num < 0f)
		{
			rb.AddForce(-base.transform.right * turnSpeed);
		}
		if (x < -0.2f && num > 0f)
		{
			rb.AddForce(base.transform.right * turnSpeed);
		}
		MonoBehaviour.print(vector.magnitude);
	}

	private void Jump()
	{
		if (numOfJumps == 0f)
		{
			return;
		}
		//anim.enabled = true;
		//if (!anim.GetBool("reloading"))
		//{
		//	anim.SetTrigger("jump");
		//}
		if (isWallRunning)
		{
			if (isWallRight)
			{
				rb.AddForce(-base.transform.right * wallHopForce);
			}
			if (isWallLeft)
			{
				rb.AddForce(base.transform.right * wallHopForce);
			}
			if (isWallFront)
			{
				rb.AddForce(-base.transform.forward * wallHopForce * 1.13f);
			}
			if (isWallBack)
			{
				rb.AddForce(base.transform.forward * wallHopForce * 1.13f);
			}
		}
		if (grounded)
		{
			rb.AddForce(base.transform.up * jumpStrength);
			numOfJumps -= 1f;
			return;
		}
		Vector3 velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		rb.velocity = velocity;
		rb.AddForce(base.transform.up * jumpStrength);
		numOfJumps = 0f;
	}

	private void CounterMovement(float x, float z)
	{
		bool flag = x == 0f && z == 0f;
		if (grounded && isSliding)
		{
			Drag(SLIDING_DRAG);
		}
		else if (!grounded && isSliding)
		{
			Drag(0f);
		}
		else if (grounded && flag)
		{
			Drag(COUNTER_DRAG);
		}
		else if (!grounded && flag)
		{
			Drag(AIR_DRAG);
		}
		else
		{
			Drag(0f);
		}
	}

	private void Drag(float dragStrength)
	{
		float x = vel.x;
		float y = vel.y;
		if (y > 0.2f)
		{
			rb.AddForce(-base.transform.forward * dragStrength);
		}
		if (y < -0.2f)
		{
			rb.AddForce(base.transform.forward * dragStrength);
		}
		if (x > 0.2f)
		{
			rb.AddForce(-base.transform.right * dragStrength);
		}
		if (x < -0.2f)
		{
			rb.AddForce(base.transform.right * dragStrength);
		}
	}

	public Vector2 CurVelocityRelativeToLook()
	{
		float x = base.transform.InverseTransformDirection(rb.velocity).x;
		float z = base.transform.InverseTransformDirection(rb.velocity).z;
		return new Vector2(x, z);
	}

	private IEnumerator Dash()
	{
		float axisRaw = Input.GetAxisRaw("Horizontal");
		float axisRaw2 = Input.GetAxisRaw("Vertical");
		Vector2 vel2 = vel;
		Vector2 vel3 = vel;
		if (axisRaw == 0f && axisRaw2 == 0f)
		{
			rb.velocity = base.transform.forward * dashSpeed;
		}
		if (axisRaw2 > 0f && axisRaw > 0f)
		{
			rb.velocity = (base.transform.forward + base.transform.right).normalized * dashSpeed;
		}
		else if (axisRaw2 > 0f && axisRaw < 0f)
		{
			rb.velocity = (base.transform.forward - base.transform.right).normalized * dashSpeed;
		}
		else if (axisRaw2 < 0f && axisRaw < 0f)
		{
			rb.velocity = (-base.transform.forward - base.transform.right).normalized * dashSpeed;
		}
		else if (axisRaw2 < 0f && axisRaw > 0f)
		{
			rb.velocity = (-base.transform.forward + base.transform.right).normalized * dashSpeed;
		}
		else if (axisRaw2 > 0f)
		{
			rb.velocity = base.transform.forward * dashSpeed;
		}
		else if (axisRaw2 < 0f)
		{
			rb.velocity = -base.transform.forward * dashSpeed;
		}
		else if (axisRaw > 0f)
		{
			rb.velocity = base.transform.right * dashSpeed;
		}
		else if (axisRaw < 0f)
		{
			rb.velocity = -base.transform.right * dashSpeed;
		}
		rb.useGravity = false;
		//dashParticles.Play();
		//visualFX.dashFX(dashDuration);
		yield return new WaitForSeconds(dashDuration);
		EndDash();
	}

	private void EndDash()
	{
		//dashParticles.Stop();
		rb.useGravity = true;
		float maxLength = maxSpeed + 5f;
		Vector3 vector = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0f, rb.velocity.z), maxLength);
		rb.velocity = vector + new Vector3(0f, rb.velocity.y, 0f);
	}

	public void KnockBack(Vector3 knockDirection)
	{
		rb.AddForce(knockDirection, ForceMode.Impulse);
	}

	private void OnCollisionEnter(Collision col)
	{
		if (Mathf.Abs(Vector3.Dot(col.contacts[0].normal, Vector3.up)) < 0.1f)
		{
			touchingWall = true;
		}
	}

	private void StartWallRun()
	{
		if (rb.velocity.y < 2f)
		{
			rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
			rb.AddForce(base.transform.up * 300f);
		}
		rb.useGravity = false;
		isWallRunning = true;
		maxSpeed *= speedMultiplier;
		if (numOfJumps <= 0f)
		{
			numOfJumps = 1f;
		}
	}

	private void WallRun()
	{
		if (isWallRunning)
		{
			if (isWallRight)
			{
				rb.AddForce(base.transform.right * wallrunForce * 1.3f);
			}
			if (isWallLeft)
			{
				rb.AddForce(-base.transform.right * wallrunForce * 1.3f);
			}
			if (isWallFront)
			{
				rb.AddForce(base.transform.forward * wallrunForce * 1.3f);
			}
			if (isWallBack)
			{
				rb.AddForce(-base.transform.forward * wallrunForce * 1.3f);
			}
		}
	}

	private void StopWallRun()
	{
		numOfJumps = 1f;
		rb.useGravity = true;
		isWallRunning = false;
		maxSpeed /= speedMultiplier;
	}

	private void CheckForWall()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position, base.transform.right, out hitInfo, 0.8f, whatIsGround))
		{
			isWallRight = Mathf.Abs(Vector3.Dot(hitInfo.normal, Vector3.up)) < 0.1f;
		}
		else
		{
			isWallRight = false;
		}
		if (Physics.Raycast(base.transform.position, -base.transform.right, out hitInfo, 0.8f, whatIsGround))
		{
			isWallLeft = Mathf.Abs(Vector3.Dot(hitInfo.normal, Vector3.up)) < 0.1f;
		}
		else
		{
			isWallLeft = false;
		}
		if (Physics.Raycast(base.transform.position, base.transform.forward, out hitInfo, 0.8f, whatIsGround))
		{
			isWallFront = Mathf.Abs(Vector3.Dot(hitInfo.normal, Vector3.up)) < 0.1f;
		}
		else
		{
			isWallFront = false;
		}
		if (Physics.Raycast(base.transform.position, -base.transform.forward, out hitInfo, 0.8f, whatIsGround))
		{
			isWallBack = Mathf.Abs(Vector3.Dot(hitInfo.normal, Vector3.up)) < 0.1f;
		}
		else
		{
			isWallBack = false;
		}
		if (!isWallFront && !isWallBack && !isWallLeft && !isWallRight)
		{
			touchingWall = false;
		}
		if (isWallRunning && (grounded || !touchingWall))
		{
			StopWallRun();
		}
	}

	private void TiltCamera(float direction)
	{
		Quaternion b = ((!(direction > 0f)) ? Quaternion.Euler(0f, 0f, 0f - tiltAngle) : Quaternion.Euler(0f, 0f, tiltAngle));
		cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, b, Time.deltaTime * tiltSmooth);
	}

	private void ResetTilt()
	{
		cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, initialRotation, Time.deltaTime * (tiltSmooth - 0.4f));
	}
}
