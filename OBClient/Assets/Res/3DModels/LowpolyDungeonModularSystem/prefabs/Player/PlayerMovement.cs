using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {


	public float speed = 3f;
	public float sprintSpeed = 6f;
	public float crouchSpeed = 1f;
	public float jumpSpeed = 5f;
	float verticalVelocity = 0f;
	float currentSpeed;
	bool crouching = false;
	bool sprinting = false;

	Vector3 direction = Vector3.zero; //WSAD dir
	CharacterController cc;
	// Animator anim;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
		//anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		//WSAD movement in "direction"
		direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if(direction.magnitude > 1f){
			direction = direction.normalized;
		}
		//anim.SetFloat("Speed", direction.magnitude);

		//Jump

		if(cc.isGrounded && Input.GetButtonDown("Jump")){
			verticalVelocity = jumpSpeed;
		}
		if(Input.GetButtonDown("Crouch")){
			//anim.SetBool("Crouch", true);
			crouching = true;
		}
		else if(Input.GetButtonUp("Crouch")){
			//anim.SetBool("Crouch", false);
			crouching = false;
		}

		if(Input.GetButtonDown("Sprint")){
			//anim.SetBool("Sprint", true);
			sprinting = true;
		}
		else if(Input.GetButtonUp("Sprint")){
			//anim.SetBool("Sprint", false);
			sprinting = false;
		}
		//Debug.Log ("spr: "+sprinting+", crc: "+crouching);
	}

	void FixedUpdate () {
		
		// "direction" is the desired movement direction, based on our player's input

		Vector3 dist = direction * currentSpeed * Time.deltaTime;

		if(sprinting == true){
			currentSpeed = sprintSpeed;
		}
		else if(crouching == true){
			currentSpeed = crouchSpeed;
		}
		else{
			currentSpeed = speed;
		}

		//Debug.Log(currentSpeed);

		if(cc.isGrounded && verticalVelocity < 0 && crouching == false) {
			
			//anim.SetBool("Jump", false);
			
			verticalVelocity = (Physics.gravity.y * 4) * Time.deltaTime;
		}
		else {
			// We are either not grounded, or we have a positive verticalVelocity (i.e. we ARE starting a jump)
			
			// To make sure we don't go into the jump animation while walking down a slope, make sure that
			// verticalVelocity is above some arbitrary threshold before triggering the animation.
			// 75% of "jumpSpeed" seems like a good safe number, but could be a standalone public variable too.
			//
			// Another option would be to do a raycast down and start the jump/fall animation whenever we were
			// more than ___ distance above the ground.
			if(Mathf.Abs(verticalVelocity) > jumpSpeed*0.75f) {
				//anim.SetBool("Jump", true);
			}
			
			// Apply gravity.
			verticalVelocity += (Physics.gravity.y * 4) * Time.deltaTime;
		}
		
		// Add our verticalVelocity to our actual movement for this frame
		dist.y = verticalVelocity * Time.deltaTime;
		
		// Apply the movement to our character controller (which handles collisions for us)
		cc.Move( dist );
	}
}
