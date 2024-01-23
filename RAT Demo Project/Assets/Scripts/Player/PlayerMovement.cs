using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private float speed = 6f;

	private Vector3 movement;
	private Animator animator;
	private Rigidbody playerRigidbody;
	private int floorMask;
	private float camRayLength = 100f;
	private Camera cam;

	private Vector3 lastMousePos = new Vector3(100000f, 100000f);

	private void Awake()
	{
		cam = Camera.main;
		floorMask = LayerMask.GetMask("Floor");
		animator = GetComponent<Animator>();
		playerRigidbody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		Move(h, v);
		Turning();
		Animating(h, v);
	}

	private void Move(float h, float v)
	{
		movement.Set (h, 0, v);
		movement = movement.normalized * speed * Time.deltaTime;

		playerRigidbody.MovePosition (transform.position + movement);
	}

	private void Turning()
	{
		Vector3 mousePos = Input.mousePosition;
		if (lastMousePos == mousePos)
			return;

		Ray camRay = cam.ScreenPointToRay(mousePos);
		RaycastHit floorHit;
		if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) 
		{
			Vector3 playerToMouse = floorHit.point - transform.position;
			playerToMouse.y = 0;

			var rotation = Quaternion.LookRotation(playerToMouse);
			playerRigidbody.MoveRotation(rotation);
		}
		lastMousePos = Input.mousePosition;
	}

	private void Animating(float h, float v)
	{
		bool walking = h != 0f || v != 0f;
		animator.SetBool("IsWalking", walking);
	}

}