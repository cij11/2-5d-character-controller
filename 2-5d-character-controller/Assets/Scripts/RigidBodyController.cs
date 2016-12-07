using UnityEngine;
using System.Collections;

public class RigidBodyController : MonoBehaviour {
	public Vector3 gravityFocus = new Vector3(0, 0, 0);
	Rigidbody body;
	Collider physCollider;
	float width = 1f;
	float height = 1f;

	float walkPower = 2;

	//Speed the character walks at when left or right held
	float maxWalkSpeed = 12;
	//Time to attain walk velocity from stationary.
	float landSpeedUpForce = 20f; 
	float landBreakForce = 60f;

	float maxAirSpeed = 12;
	float airSpeedUpForce = 8f;
	float airBreakForce = 20f;

	float gravityPower = 14f;

	float jumpVelocity = 10f;

	CollisionInfo collisions;
	RaycastOrigins raycastOrigins;
	float skinWidth = 0.01f;
	int verticalRayCount = 8;
	float verticalRaySpacing;
	public LayerMask collisionMask;
	float maxSlopeIdle = 60;
	float surfaceFrictionForce = 5;
	float staticFrictionCutoff = 0.5f;

	//Store state variables and timers - eg, double jump used, time since touched wall
	StateInfo stateInfo;

	//Force that will be applied to keep character sticking to a wall if grabbing or sliding down it
	float wallHugForce = 5f;
	//Time in seconds that a wall jump can still occur after releasing the wall
	float wallJumpTimeWindow = 0.1f;
	int maxDoubleJumps = 2;

	float jetpackPower = 18f;
	public PhysicMaterial[] physicMaterials;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody>();
		physCollider = GetComponent<Collider>();
		UpdateRaycastOrigins();
		CalculateRaySpacing();
	}
	
	// Update is called once per frame
	void Update () {
		OrientToGravityFocus();
		Move();
		Jump();
		Jetpack();

		ApplyGravity();
		ApplyWallHugForce();
		AssignPhysicMaterial();

		collisions.Reset();
		UpdateRaycastOrigins();
		DetectGround();
		DetectSlope();
		DetectWall();

		stateInfo.Update();
	}

	void OrientToGravityFocus(){
		//Get a vector to point feet at. This might be the center of a hull for a round world, or down for a ship
		//with artificial gravity. Give this body position as arguement to calculate down or radial orientation position
		Vector3 pointToOrientTo = gravityFocus;
/*
		Vector3 vectorToMountCenter = pointToOrientTo - body.position;
		vectorToMountCenter.Normalize ();
		Vector3 relativeDown = vectorToMountCenter;
		Vector3 lookAtVector = this.body.position + new Vector3 (0f, 0f, 1f);

		//Look at a spot immediately left of the unit, then look intot the scene.
		body.transform.rotation = Quaternion.LookRotation (Vector3.forward, -relativeDown);
*/
		//Align the up vector of the body with the vector from the focus towards the body.
		Vector3 vectorFromFocusToBody = body.position - gravityFocus;
		vectorFromFocusToBody.Normalize();
		body.transform.rotation = Quaternion.FromToRotation(body.transform.up, vectorFromFocusToBody) * transform.rotation;
	}

//Cast rays down to see if near ground for purposes of jumping, air vs land control, and friction.
	void DetectGround(){
		float rayLength =  0.05f;

		for (int i = 0; i < verticalRayCount; i++){
			Vector3 rayOrigin = raycastOrigins.bottomLeft;
			rayOrigin += body.transform.right * (verticalRaySpacing * i);
			RaycastHit hit;
			bool isHit = Physics.Raycast(rayOrigin, -body.transform.up, out hit, rayLength, collisionMask);


			Debug.DrawRay(rayOrigin, -body.transform.up * rayLength, Color.red);
			
			//if there is a hit, move the character only enough to come
			//into contact with the 
			if(isHit){
				collisions.below = true;
				collisions.surfaceNormal = hit.normal;
				collisions.groundSlopeAngle = Vector3.Angle(hit.normal,body.transform.up);
			}
		}
		if(collisions.below) print("Collision below");
		else print("No collision below");
		print("Slope angle = " + collisions.groundSlopeAngle.ToString());
		print("Slope normal = " + collisions.surfaceNormal.ToString());
	}

	//Fire a ray backwards from the bottom corner behind the player.
	//If this hits terrain, then the character is descending a slope.
	//Set descending flag to true and store angle.
	void DetectSlope(){
		//Project velocity onto players horzontal vector. +ve for right, -ve for left
//		float currentSpeed = Vector3.Dot(body.velocity, body.transform.right);
//		float direction = Mathf.Sign(currentSpeed);

		float rayLength = 0.05f;
		RaycastHit hitLeft;
		RaycastHit hitRight;

		collisions.leftBot = Physics.Raycast(raycastOrigins.bottomLeft, -body.transform.right, out hitLeft, rayLength, collisionMask);
		collisions.rightBot = Physics.Raycast(raycastOrigins.bottomRight, body.transform.right, out hitRight, rayLength, collisionMask);

		Debug.DrawRay(raycastOrigins.bottomLeft, -body.transform.right * rayLength, Color.red);
		Debug.DrawRay(raycastOrigins.bottomRight, body.transform.right * rayLength, Color.red);

		//Store the angles of any detected walls too 
		if(collisions.leftBot){
			collisions.leftWallAngle = Vector3.Angle(body.transform.up, hitLeft.normal);
		}
		if(collisions.rightBot){
			collisions.rightWallAngle = Vector3.Angle(body.transform.up, hitRight.normal);
		}
		
		print("Left ray wall angle = " + collisions.leftWallAngle.ToString());
		print("Right ray wall angle = " + collisions.rightWallAngle.ToString());


	}

	//Not currently used for anything 
	void DetectWall(){
		float rayLength = 0.05f;
		RaycastHit hitLeft;
		RaycastHit hitRight;

		collisions.leftTop = Physics.Raycast(raycastOrigins.topLeft, -body.transform.right, out hitLeft, rayLength, collisionMask);
		collisions.rightTop = Physics.Raycast(raycastOrigins.topRight, body.transform.right, out hitRight, rayLength, collisionMask);

		Debug.DrawRay(raycastOrigins.topLeft, -body.transform.right * rayLength, Color.red);
		Debug.DrawRay(raycastOrigins.topRight, body.transform.right * rayLength, Color.red);

		//Allow top rays to store wall 
		if(collisions.leftTop){
			collisions.leftTopWallAngle = Vector3.Angle(body.transform.up, hitLeft.normal);
		}
		if(collisions.rightTop){
			collisions.rightTopWallAngle = Vector3.Angle(body.transform.up, hitRight.normal);
		}

	}

    //If in a position to grab or slide down a wall, apply a small force towards the wall
    void ApplyWallHugForce()
    {
        if (!collisions.below)
        {
            if (collisions.leftTop || (collisions.leftBot && collisions.leftWallAngle > 85))
            {
				//adjacent to left wall, so apply wall hug force, and reset timer for that side
				body.AddForce(-body.transform.right * wallHugForce);
				stateInfo.leftWallHugTimer = 0f;
            }
            if (collisions.rightTop || (collisions.rightBot && collisions.rightWallAngle > 85))
            {
				//Likewise for the right wall
				body.AddForce(body.transform.right * wallHugForce);
				stateInfo.rightWallHugTimer = 0f;
            }
        }
    }

	void AssignPhysicMaterial(){
		//Reset to slippery material by default
		physCollider.material = physicMaterials[0];

		//If grabbing adjacent to a wall and in the air, set high static friction, unless holding down
		if(!collisions.below){
			if(collisions.leftTop || collisions.rightTop || (collisions.leftBot && collisions.leftWallAngle > 85) || (collisions.rightBot && collisions.rightWallAngle > 85)){
				if(Input.GetKey("s")){
					//If down is held, set the material to be smoother
					physCollider.material = physicMaterials[3];
				}
				else{
					//else make very sticky
					physCollider.material = physicMaterials[2];
				}
			}
		}
		//If no directional input is presssed. Press 's' to enable skiing.
		if (!Input.GetKey("a") && !Input.GetKey("d") &&!Input.GetKey("s")){
			//And player is grounded
			if(collisions.below){
				//And this is a slope the player can stand/idle/rest on
				if (collisions.groundSlopeAngle < maxSlopeIdle){
					//Change to sticky material of conditions for resting on a slope are met
					physCollider.material = physicMaterials[1];
				}
			}
		}
	}

	void Move(){
		//Left and right move. more responsive controls on the ground compared to the air
		if(Input.GetKey("a")){
			if (collisions.below){
				MoveHorizontal(-1, landSpeedUpForce, landBreakForce, maxWalkSpeed);
			}
			else{
				MoveHorizontal(-1, airSpeedUpForce, airBreakForce, maxAirSpeed);
			}
		}
		if(Input.GetKey("d")){
			if (collisions.below){
				MoveHorizontal(1, landSpeedUpForce, landBreakForce, maxWalkSpeed);
			}
			else{
				MoveHorizontal(1, airSpeedUpForce, airBreakForce, maxAirSpeed);
			}
		}
	}

	//Determine what type of jump is appropriate when jump pressed, and apply
	void Jump(){
		//Reset doublejump counter if grounded
		if (collisions.below){
			stateInfo.remainingDoubleJumps = maxDoubleJumps;
		}
		if(Input.GetKeyDown("space")){

			//Detect ground and add upwards component to velocity if standing.
			//If terrain is too steep, walljump instead
			//Only allow jumping if standing on or adjacent to something
			if (collisions.below && collisions.groundSlopeAngle < maxSlopeIdle){
				GroundJump();
			}
			else{
				//Potentially just need the wall hug timers...
				if (collisions.leftBot || collisions.leftTop || stateInfo.leftWallHugTimer < wallJumpTimeWindow){
					WallJump(1f);
				}
				else if (collisions.rightBot || collisions.rightTop  || stateInfo.rightWallHugTimer < wallJumpTimeWindow){
					WallJump(-1f);
				}
				//Even if we can't ground jump or wall jump, we might be able to double jump
				else if (stateInfo.remainingDoubleJumps > 0){
					//decrement counter
					stateInfo.remainingDoubleJumps--;
					DoubleJump();
				}
			}
		}
	}

	//If the current vertical speed is < the jump velocity, cancel any existing
	//vertical speed and set to jump velocity
	void GroundJump(){
					//Projection of our velocity directed up (if +ve,) or down (if -ve)
			float verticalSpeed = Vector3.Dot(body.velocity, body.transform.up);
						//If moving less than jump Velocity vertically
				if (verticalSpeed < jumpVelocity){
					//Cancel the current downwards velocity, and set it to the jump velocity
					body.velocity = body.velocity + body.transform.up * (-verticalSpeed + jumpVelocity);
				}
	}

	void WallJump(float direction){
		//Cancel all existing velocity, and set equal to a 45 degree jump in the given direction
		body.velocity = body.transform.up * jumpVelocity + body.transform.right * direction * jumpVelocity;
	}

	//If on a ledge, jump straight up
	void LedgeJump(){

	}

	void DoubleJump(){
		//Same as ground jump for now. Later, give option to change direction on double jump
		GroundJump();
	}
	//Horizontal movement. Takes parameters for the rate of acceleration in the desired direction,
	//breaking movement in the current direction, and max speed to accelerate too.
	void MoveHorizontal(float direction, float speedUpForce, float breakForce, float maxSpeed){
		//Get the current speed in the desired direction
		float currentSpeedInDesiredDirection = Vector3.Dot(body.velocity, direction*body.transform.right);

		//get vector parallel to surface normal.
		Vector3 surfaceNormalParallel = new Vector3(collisions.surfaceNormal.y, -collisions.surfaceNormal.x, 0);

		//The vector we will apply walk forces to
		Vector3 horizontalVector = body.transform.right;

		//if moving down hill, align the force vector perpendicular ot the surface normal
			//If grounded
			//And moving left right with a collision right
			//then moving downhill
			//So
			//Movement to the left should be aligned with the slope

			//or

			//If grounded
			//and moving right with a collision left
			//then moving downhill
			//So
			//movement to the right should be aligned with the slope
		if (collisions.below){
			//If player wants to move left
			if (direction < 0){
				//If there is a collision behind the player
				if(collisions.rightBot){
					//Then set the movement direction to align with the slope
					horizontalVector = surfaceNormalParallel;
				}
			}
			//If player wants to move right
			else{
				//If there is a collision behind the player
				if(collisions.leftBot){
					//Then set the movement direction to align with the slope
					horizontalVector = surfaceNormalParallel;
				}
			}
		}

		//dot product of desired and current velocity will be negative if walking the wrong way
		if (Mathf.Sign(currentSpeedInDesiredDirection) < 0){
			Break(direction, breakForce);
		}
		else if (currentSpeedInDesiredDirection < maxSpeed){
			body.AddForce(horizontalVector * direction * speedUpForce);
		}
	}

	void Break(float direction, float breakForce){
		Vector3 horizontalVector = body.transform.right;
		//get vector parallel to surface normal.
		Vector3 surfaceNormalParallel = new Vector3(collisions.surfaceNormal.y, -collisions.surfaceNormal.x, 0);

		if (collisions.below){
			//If player wants to move left
			if (direction < 0){
				//If there is a collision behind the player
				if(collisions.rightBot){
					//Then set the movement direction to align with the slope
					horizontalVector = surfaceNormalParallel;
				}
			}
			//If player wants to move right
			else{
				//If there is a collision behind the player
				if(collisions.leftBot){
					//Then set the movement direction to align with the slope
					horizontalVector = surfaceNormalParallel;
				}
			}
		}
		body.AddForce(horizontalVector *direction * breakForce);
	}

	void Jetpack(){
		if(Input.GetKey("w")){
			body.AddForce(body.transform.up * jetpackPower);
		}
	}

	void ApplyGravity(){
		body.AddForce(-this.transform.up* gravityPower);
	}

	//Find the corners of the square
	void UpdateRaycastOrigins(){
 
		float xoffset = (width/2f) - skinWidth;
		float yoffset = (height/2f) - skinWidth;
		float zpos = transform.position.z;
		raycastOrigins.bottomLeft = body.transform.position + body.transform.rotation * new Vector3(-xoffset, -yoffset, zpos);
		raycastOrigins.bottomRight = body.transform.position + body.transform.rotation * new Vector3(xoffset, -yoffset, zpos);
		raycastOrigins.topLeft = body.transform.position + body.transform.rotation * new Vector3(-xoffset, yoffset, zpos);
		raycastOrigins.topRight = body.transform.position + body.transform.rotation * new Vector3(xoffset, yoffset, zpos);
	}

	 public struct CollisionInfo{
        public bool above, below;
        public bool leftBot, rightBot;
		public bool leftTop, rightTop;
		public Vector3 surfaceNormal;
		public float groundSlopeAngle;
		public float leftWallAngle;
		public float rightWallAngle;
		public float leftTopWallAngle;
		public float rightTopWallAngle;

        public void Reset(){
            above = below = false;
            leftBot = rightBot = false;
			leftTop = rightTop = false;
			surfaceNormal = new Vector3(0f, 0f, 0f);
			groundSlopeAngle = 0f;
			leftWallAngle = 0f;
			rightWallAngle = 0f;
			leftTopWallAngle = 0f;
			rightTopWallAngle = 0f;
        }
	 }

	 //Vectors in worldspace where rays are cast from, coresponding to corners of the square.
	struct RaycastOrigins{
		public Vector3 topLeft, topRight;
		public Vector3 bottomLeft, bottomRight;
	}

	public struct StateInfo{
		public float leftWallHugTimer;
		public float rightWallHugTimer;
		public int remainingDoubleJumps;
		public void Update(){
			leftWallHugTimer+=Time.deltaTime;
			if (leftWallHugTimer > 10f){
				leftWallHugTimer = 10f;
			}
			rightWallHugTimer+=Time.deltaTime;
			if (rightWallHugTimer > 10f){
				rightWallHugTimer = 10f;
			}
		}
	}

	void CalculateRaySpacing(){
		verticalRaySpacing = (width-skinWidth*2f) / (verticalRayCount - 1);
	}

	//Display raycast origins
	void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(raycastOrigins.bottomLeft, 0.01f);
		Gizmos.DrawSphere(raycastOrigins.bottomRight, 0.01f);
		Gizmos.DrawSphere(raycastOrigins.topLeft, 0.01f);
		Gizmos.DrawSphere(raycastOrigins.topRight, 0.01f);
    }
}
