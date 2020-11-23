using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	private const float Gravity = 9.8f;
	private CharacterController _characterController;
	private Vector3 _characterVelocity;
	private bool _isSprinting;
	/*
	private bool _area1Active;
	private bool _area2Active;
	private bool _area3Active;
	private bool _area4Active;
	*/
	private float activityTimer = 0;
	private int area1Activity = 0;
	private int area2Activity = 0;
	private int area3Activity = 0;
	private int area4Activity = 0;
	
	[SerializeField, Tooltip("Description of this object")]
	private string description = "";

	[SerializeField][Tooltip("The force applied to the character when jumping")]
	private float JumpForce = 10f;

	[SerializeField][Tooltip("The camera for the first person controller")]
	public Camera _Camera;

	[SerializeField]
	[Tooltip("The fall speed modifier, increases gravity")]
	private float GravityMultiplier = 2f;
	
	[SerializeField][Tooltip("Walking speed of the character")]
	private Vector3 WalkingSpeed = new Vector3(2f,0f,4f); 	
	
	[SerializeField][Tooltip("Sprinting speed of the character")]
	private Vector3 SprintSpeed = new Vector3(2f,0f,9f); 
	
	[SerializeField][Tooltip("Walking speed of the character")]
	private Vector3 AirStrafeSpeed = new Vector3(0.01f,0f,0.01f); 	
	
	[SerializeField][Tooltip("Minimum angle the camera can look up/down")]
	private float MinCameraVerticalAngle = -70;
	[SerializeField][Tooltip("Maximum angle the camera can look up/down")]
	private float MaxCameraVerticalAngle = 55;
	
	[SerializeField][Tooltip("The speed at which the camera moves")]
	private Vector2 CameraSpeed = new Vector2(.5f,0.1f);

	[SerializeField][Tooltip("Does the sprint button need to be held? false = toggle")]
	private bool HoldToSprint;
	
	// Start is called before the first frame update
	private void Start()
	{
		// cache the character controller component
		_characterController = GetComponent<CharacterController>();
	}

	
	
	
	// Update is called once per frame
	private void Update()
	{

		HandleCameraMovement();
		
		// Was i grounded last frame?
		var wasGrounded = _characterController.isGrounded;

		//TODO: Crouch

		if (HoldToSprint)
		{
			//sprint hold
			_isSprinting = Input.GetButton("Sprint");
		}
		else
		{
			//sprint toggle
			if (Input.GetButtonDown("Sprint"))
			{
				_isSprinting = !_isSprinting;
			}
		}

		if (_characterController.isGrounded)
		{
			//ground movement controls
			HandleGroundMovement();
					
			if (Input.GetButtonDown("Jump"))
			{
				// remove vertical velocity
				_characterVelocity.y = 0;

				// add jump velocity
				//_characterVelocity += Vector3.up * JumpForce;
			}

		}		
		else
		{
			// add air acceleration this frame
			HandleAirMovement();
		}
		
		// add gravitational acceleration this frame
		_characterVelocity += Vector3.down * (Gravity * GravityMultiplier * Time.deltaTime);
		
		// Apply calculated movement
		_characterController.Move(_characterVelocity * Time.deltaTime);
		
		
	}

	/**/
	private void OnTriggerEnter(Collider other)
	{
		print("Player has entered the area of" + other.name);
		description = "Ghost Activity has temporarily subsided";
		DisplayDetailUI();
	}
	/**/

	void OnTriggerStay(Collider other)
	{
		activityTimer += Time.deltaTime;
		if (activityTimer >= 1f && other.name == "Area1")
		{
			area1Activity += 2;
			area2Activity -= 1;
			area3Activity -= 1;
			area4Activity -= 1;
			activityTimer = 0;
		}else if (activityTimer >= 1f && other.name == "Area2")
		{
			area2Activity += 2;
			area1Activity -= 1;
			area3Activity -= 1;
			area4Activity -= 1;
			activityTimer = 0;
		}else if (activityTimer >= 1f && other.name == "Area3")
		{
			area3Activity += 2;
			area1Activity -= 1;
			area2Activity -= 1;
			area4Activity -= 1;
			activityTimer = 0;
		}else if (activityTimer >= 1f && other.name == "Area4")
		{
			area4Activity += 2;
			area1Activity -= 1;
			area2Activity -= 1;
			area3Activity -= 1;
			activityTimer = 0;
		}
		
		if (area1Activity < 0)
		{
			area1Activity = 0;
		} else if (area2Activity < 0)
		{
			area2Activity = 0;
		} else if (area3Activity < 0)
		{
			area3Activity = 0;
		}else if (area4Activity < 0)
		{
			area4Activity = 0;
		}

		print("Area1: " + area1Activity);
		print("Area2: " + area2Activity);
		print("Area3: " + area3Activity);
		print("Area4: " + area4Activity);
		
		if (area1Activity >= 120 || area2Activity >= 120 || area3Activity >= 120 || area4Activity >= 120)
		{
			print("Ghost activity has reached or exceeded critical spookums, you are now dead.");
			description = "Ghost activity has reached or exceeded critical spookums, you are now dead.";
		}

		if ((area1Activity == 30 && other.name == "Area1") || (area2Activity == 30 && other.name == "Area2") || (area3Activity == 30 && other.name == "Area3") || (area4Activity == 30 && other.name == "Area4"))
		{
			description = "Ghost activity in the local area ('" + other.name + "') is currently low.";
			print(description);
		}
		if ((area1Activity == 60 && other.name == "Area1") || (area2Activity == 60 && other.name == "Area2") || (area3Activity == 60 && other.name == "Area3") || (area4Activity == 60 && other.name == "Area4"))
		{
			description = "Ghost activity in the local area ('" + other.name + "') is currently moderate but rising.";
			print(description);
		}
		if ((area1Activity == 90 && other.name == "Area1") || (area2Activity == 90 && other.name == "Area2") || (area3Activity == 90 && other.name == "Area3") || (area4Activity == 90 && other.name == "Area4"))
		{
			description = "Ghost activity in the local area ('" + other.name + "') is currently high.";
			print(description);
		}
		DisplayDetailUI();
	}
	
	public virtual void DisplayDetailUI()
	{
		Debug.Log($"Description:\n {description}");
		HUD.Instance.SetOutputText($"{description}");
	}
	
	
	private void HandleAirMovement() 
	{
		// cache the input axes
		Vector3 inputAxes = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
		
		// convert to a vector representing movement directions
		Vector3 inputAsMovement = new Vector3(inputAxes.x, 0f, inputAxes.y);
		
		// multiply movement direction by speed  
		inputAsMovement.Scale(AirStrafeSpeed);

		// transform the movement vector into world space relative to the player
		Vector3 worldSpaceMovement = transform.TransformVector(inputAsMovement);
		_characterVelocity += worldSpaceMovement;
	}

	private void HandleGroundMovement()
	{
		// cache the input axes
		Vector3 inputAxes = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
		
		// convert to a vector representing movement directions
		Vector3 inputAsMovement = new Vector3(inputAxes.x, 0f, inputAxes.y);

		if (!_isSprinting)
		{
			// multiply movement direction by speed  
			inputAsMovement.Scale(WalkingSpeed);
		}
		else
		{
			// multiply movement direction by speed  
			inputAsMovement.Scale(SprintSpeed);
		}

		// transform the movement vector into world space relative to the player
		Vector3 worldSpaceMovement = transform.TransformVector(inputAsMovement);
		_characterVelocity = worldSpaceMovement;
	}

	private float _cameraVerticalAngle;
	
	private void HandleCameraMovement()
	{
		// cache the camera input axes
		Vector2 inputAxes = new Vector2(Input.GetAxisRaw("Horizontal_Camera"),Input.GetAxisRaw("Vertical_Camera"));
		//Debug.Log($"Hor:{Input.GetAxisRaw("Horizontal_Camera")} Ver:{Input.GetAxisRaw("Vertical_Camera")}");

		// scale the input by the camera speed
		inputAxes.Scale(CameraSpeed);
		
		// horizontal rotation - rotate the character
		transform.Rotate(Vector3.up, inputAxes.x,Space.Self);
		
		// vertical rotation - rotate the camera
		// add new input to the accumulated angle
		_cameraVerticalAngle += inputAxes.y;
		
		// clamp the vertical rotation to be between min and max angles
		_cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, MinCameraVerticalAngle, MaxCameraVerticalAngle);
		
		// apply to the camera
		_Camera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle,0f,0f);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(_Camera.transform.position + _Camera.transform.forward,0.01f);
	}
}