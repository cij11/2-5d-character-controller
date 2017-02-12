using UnityEngine;
using System.Collections;

public class CharacterMovementActuator : MonoBehaviour
{
	public bool localGravity = true;
	public Vector3 radialGravityFocus = new Vector3 (0, 0, 0);
	public Vector3 directionalGravityUp = new Vector3 (0f, -1f, 0f);
	public bool radialGravity = false;
	Rigidbody body;
	Collider physCollider;
	CharacterContactSensor contactSensor;

	public float maxAirSpeed = 12;
	public float maxWalkSpeed = 12;
	public float jumpSpeed = 10f;

	//Speed the character walks at when left or right held

	//Time to attain walk velocity from stationary.
	float landSpeedUpForce = 1500f;
	float landBreakForce = 5000f;

	float airSpeedUpForce = 600f;
	float airBreakForce = 1500f;

	float gravityForce = 900f;

	float wallJumpClearanceSpeed = 3f;
	bool isSlideCommandGiven = false;
	float releaseSpeed = 2f;

	float wallHugForce = 200f;

	float jetpackForce = 1200f;
	float parachuteFallSpeed = 0.2f;
	float paracuteDeceleration = 1.5f;
	float terminalArialSpeed = -50f;
	float terminalWallSlideSpeed = -5f;
	float terminalWallClimbSpeed = 1f;

	float phaseTimer = 0f;
	bool isPhasing = false;
	bool isRolling = false;
	//Rolling is a horizontal phase that sticks to the ground.

	float phaseDirection = 1f;
	float phaseVerticalDireciton = 0f;
	float exitPhaseSpeed = 2f;
	float phasePeriod;

	public PhysicMaterial[] physicMaterials;

	bool isMoveHorizontalCommandGiven = false;

	bool isDead = false;

	bool huggingWall = false;

	// Use this for initialization
	void Start ()
	{
		body = GetComponent<Rigidbody> ();
		contactSensor = GetComponent<CharacterContactSensor> ();
		physCollider = GetComponent<BoxCollider> ();
		directionalGravityUp.Normalize ();
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		OrientToGravity ();
		ApplyGravity ();
		ApplyWallHug ();

		if (!isDead) {
			LimitWallSlideSpeed ();

			AssignPhysicMaterial ();
			isMoveHorizontalCommandGiven = false;
			isSlideCommandGiven = false;

			ProcessPhasing ();
		}
	}

	void OrientToGravity ()
	{
		if (!localGravity) {
			OrientToDownVector (GravityManager.instance.GetDownVector (body.transform.position));
		} else {
			if (radialGravity) {
				OrientToGravityFocus ();
			} else {
				OrientToGravityDirection ();
			}
		}
	}

	void OrientToDownVector(Vector3 downVec){
		body.transform.rotation = Quaternion.FromToRotation (-body.transform.up, downVec) * transform.rotation;
		body.transform.rotation = Quaternion.FromToRotation (body.transform.forward, Vector3.forward) * transform.rotation; //Ensure character always facing towards the 'screen'
	}

	void OrientToGravityDirection ()
	{
		body.transform.rotation = Quaternion.FromToRotation (body.transform.up, directionalGravityUp) * transform.rotation;
	}

	void OrientToGravityFocus ()
	{
		//Get a vector to point feet at. This might be the center of a hull for a round world, or down for a ship
		//with artificial gravity. Give this body position as arguement to calculate down or radial orientation position
		Vector3 pointToOrientTo = radialGravityFocus;

		//Align the up vector of the body with the vector from the focus towards the body.
		Vector3 vectorFromFocusToBody = body.position - radialGravityFocus;
		vectorFromFocusToBody.Normalize ();
		body.transform.rotation = Quaternion.FromToRotation (body.transform.up, vectorFromFocusToBody) * transform.rotation;
		body.transform.rotation = Quaternion.FromToRotation (body.transform.forward, Vector3.forward) * transform.rotation; //Ensure character always facing towards the 'screen'
	}



	float VerticalSpeed ()
	{
		return Vector3.Dot (body.velocity, body.transform.up);
	}

	float HorizontalSpeed ()
	{
		return Vector3.Dot (body.velocity, body.transform.right);
	}

	void LimitWallSlideSpeed ()
	{
		//Negate gravity
	}

	void LimitWallClimbSpeed ()
	{
		float verticalSpeed = VerticalSpeed ();
		if (verticalSpeed > terminalWallClimbSpeed) {
			body.velocity = body.transform.up;
		}
	}

	//Assign physic material to idle, unless on a steep slope, a wall, or input given
	//to change it to frictionless or low friction.
	void AssignPhysicMaterial ()
	{
		//Reset to idle rest material by default
		physCollider.material = physicMaterials [(int)PhysicMatTypes.IDLE_STANDING];

		//If grabbing adjacent to a wall set high static friction, unless holding down
		if (huggingWall) {
			if (isSlideCommandGiven) {
				//If down is held, set the material to be smoother
				physCollider.material = physicMaterials [(int)PhysicMatTypes.LOW_KINETIC_NO_STATIC];
			} else {
				//else make very sticky
				physCollider.material = physicMaterials [2];
			}
		}
		//If FLATGROUND and a directional command is given, or a slide command is given, set to low friction
		if (contactSensor.GetContactState () == ContactState.FLATGROUND) {
			//And player is FLATGROUND
			if (isMoveHorizontalCommandGiven || isSlideCommandGiven) {
				//Change to slippery material
				physCollider.material = physicMaterials [(int)PhysicMatTypes.FRICTIONLESS];
			}
		}
		//If on a steep slope, set the material slippery
		if (contactSensor.GetContactState () == ContactState.STEEPSLOPE) {
			physCollider.material = physicMaterials [(int)PhysicMatTypes.FRICTIONLESS];
		}

		//If the collider is airborne, make frictionless. Prevents juddering
		//when head touching the underside of a surface, and allows movement
		//if ground has not been detected (eg, if standing on peak)
		if (contactSensor.GetContactState () == ContactState.AIRBORNE) {
			physCollider.material = physicMaterials [(int)PhysicMatTypes.FRICTIONLESS];
		}
		if (contactSensor.GetContactState () == ContactState.WALLADJACENT && !huggingWall) {
			physCollider.material = physicMaterials [(int)PhysicMatTypes.FRICTIONLESS];
		}

		if (isPhasing) {
			physCollider.material = physicMaterials [(int)PhysicMatTypes.FRICTIONLESS];
		}
	}

	public void MoveHorizontalCommand (float direction)
	{
		//Horizontal move command was given, so set this true to prevent
		//physicMaterial resetting to idle in AssignPhysicMaterial method.
		isMoveHorizontalCommandGiven = true;

		if (contactSensor.GetContactState () == ContactState.FLATGROUND) {
			MoveFlatHorizontal (Mathf.Sign (direction), landSpeedUpForce, landBreakForce, maxWalkSpeed);
		}

		if (contactSensor.GetContactState () == ContactState.STEEPSLOPE) {
			MoveSteepHorizontal (Mathf.Sign (direction), landSpeedUpForce, landBreakForce, maxWalkSpeed);
		}

		if (contactSensor.GetContactState () == ContactState.AIRBORNE) {
			MoveArialHorizontal (Mathf.Sign (direction), airSpeedUpForce, airBreakForce, maxAirSpeed);
		}
	}

	//If the current vertical speed is < the jump velocity, cancel any existing
	//vertical speed and set to jump velocity
	public void GroundJump ()
	{
		//Projection of our velocity directed up (if +ve,) or down (if -ve)
		float verticalSpeed = Vector3.Dot (body.velocity, body.transform.up);
		//If moving less than jump Velocity vertically
		if (verticalSpeed < jumpSpeed) {
			//Cancel the current downwards velocity, and set it to the jump velocity
			//	body.velocity = body.velocity + body.transform.up * (-verticalSpeed + jumpSpeed);
			CancelVelocityAlongVector (body.transform.up);
			body.velocity = body.velocity + body.transform.up * jumpSpeed;
		}
	}

	public void ReleaseWall (float direction)
	{
		body.velocity = body.transform.right * direction * releaseSpeed;
	}

	public void WallJump (float direction)
	{
		//Cancel all existing velocity, and set equal to a 45 degree jump in the given direction
		body.velocity = body.transform.up * jumpSpeed + body.transform.right * direction * jumpSpeed;
	}

	//If on a ledge, jump up, and slightly away from the wall.
	public void WallJumpUp (float direction)
	{
		body.velocity = body.transform.up * jumpSpeed + body.transform.right * direction * wallJumpClearanceSpeed;
	}

	public void DoubleJump ()
	{
		//Same as ground jump for now. Later, give option to change direction on double jump
		GroundJump ();
	}
	//Horizontal movement. Takes parameters for the rate of acceleration in the desired direction,
	//breaking movement in the current direction, and max speed to accelerate too.
	void MoveFlatHorizontal (float direction, float speedUpForce, float breakForce, float maxSpeed)
	{
		//Get the current speed in the desired direction
		float currentSpeedInDesiredDirection = Vector3.Dot (body.velocity, direction * body.transform.right);
		//The vector we will apply walk forces to
		Vector3 horizontalVector = CalculatePerpendicular (contactSensor.GetGroundNormal ());

		//dot product of desired and current velocity will be negative if walking the wrong way
		if (Mathf.Sign (currentSpeedInDesiredDirection) < 0) {
			body.AddForce (horizontalVector * direction * breakForce * Time.deltaTime);
		} else if (currentSpeedInDesiredDirection < maxSpeed) {
			body.AddForce (horizontalVector * direction * speedUpForce * Time.deltaTime);
		}
	}

	void MoveSteepHorizontal (float direction, float speedUpForce, float breakForce, float maxSpeed)
	{
		float uphill = contactSensor.GetUphillDirection ();

		//If trying to break on a steep slope, direct a force downwards.
		if (Mathf.Sign (direction) != Mathf.Sign (uphill)) {
			//Only break if currently moving upwards
			if (VerticalSpeed () > 0) {
				body.AddForce (-body.transform.up * breakForce * Time.deltaTime);
			}
            //Allow to accelerate downhill if not at max speed
            else {
				if (Mathf.Abs (VerticalSpeed ()) < maxWalkSpeed) {
					body.AddForce (-body.transform.up * speedUpForce * Time.deltaTime);
				}
			}
		}
        //Otherwise, direct a force horizontally
        else {
			if (Mathf.Abs (HorizontalSpeed ()) < maxWalkSpeed) {
				body.AddForce (body.transform.right * direction * speedUpForce * Time.deltaTime);
			}
		}
	}

	void MoveArialHorizontal (float direction, float speedUpForce, float breakForce, float maxSpeed)
	{
		//Get the current speed in the desired direction
		float currentSpeedInDesiredDirection = Vector3.Dot (body.velocity, direction * body.transform.right);
		Vector3 horizontalVector = body.transform.right;
		//dot product of desired and current velocity will be negative if walking the wrong way
		if (Mathf.Sign (currentSpeedInDesiredDirection) < 0) {
			body.AddForce (horizontalVector * direction * breakForce * Time.deltaTime);
		} else if (currentSpeedInDesiredDirection < maxSpeed) {
			body.AddForce (horizontalVector * direction * speedUpForce * Time.deltaTime);
		}
	}

	public void JetpackCommand ()
	{
		body.AddForce (body.transform.up * jetpackForce * Time.deltaTime);
	}

	public void ParachuteCommand ()
	{
		float verticalSpeed = Vector3.Dot (body.velocity, body.transform.up);
		//If the character is moving down
		if (verticalSpeed < 0) {
			//And the downwards speed is greater than the parachuteSpeed
			if (Mathf.Abs (verticalSpeed) > parachuteFallSpeed) {
				//Apply a force counter to gravity
				body.AddForce (body.transform.up * gravityForce * paracuteDeceleration * Time.deltaTime);
			}
		}
	}

	public void TeleportCommand (Vector3 dashVector, float dashMaxDistance)
	{
		body.velocity = new Vector3 (0f, 0f, 0f);
		//cast ray
		//teleport to the float distance, or the length until the ray hit something.
		Vector3 rayOrigin = this.transform.position;
		RaycastHit hit;
		bool isHit = Physics.Raycast (rayOrigin, this.transform.rotation * dashVector, out hit, dashMaxDistance);
		Vector3 dashPoint = this.transform.position + body.rotation * dashVector * (dashMaxDistance - 1f);
		if (isHit) {
			dashPoint = hit.point;
		}
		//Teleport at least 1 unit to avoid teleporting through terrain
		if (Vector3.Magnitude (body.transform.position - dashPoint) > 1f) {
			body.transform.position = dashPoint - body.rotation * (dashVector * 1f); //Reduce teleport distance so doesn't teleport into a wall.
		}
		OrientToGravityFocus ();
	}

	public void RollCommand (float facing, float speed, float period)
	{
		isRolling = true;
		PhaseCommand (facing, 0f, speed, period);
	}

	public void PhaseCommand (float hor, float vert, float speed, float period)
	{
		InitialisePhasing (hor, vert, speed, period);
	}

	private void InitialisePhasing (float hor, float vert, float speed, float period)
	{
		phaseDirection = hor;
		phaseVerticalDireciton = vert;
		body.velocity = body.rotation * new Vector3 (hor, vert, 0f).normalized * speed;
		this.phasePeriod = period;
		isPhasing = true;
		phaseTimer = this.phasePeriod;
		this.gameObject.layer = 9; //Layer 9 is phasing layer.
	}

	void ProcessPhasing ()
	{
		if (isPhasing) {
			UpdatePhasingTimer ();
			NegateGravity ();
			if (isRolling) {
				HugGroundWhilePhasing ();
			}
		}
	}

	void UpdatePhasingTimer ()
	{
		phaseTimer -= Time.deltaTime;
		if (phaseTimer <= 0f) {
			StopPhasing ();
		}
	}

	void NegateGravity ()
	{
		body.AddForce (-body.transform.up * gravityForce * Time.deltaTime);
	}

	void HugGroundWhilePhasing ()
	{
		if (contactSensor.GetContactState () == ContactState.FLATGROUND || contactSensor.GetContactState () == ContactState.STEEPSLOPE) {
			float speed = Vector3.Magnitude (body.velocity);
			Vector3 groundVector = CalculatePerpendicular (contactSensor.GetGroundNormal ());
			groundVector.Normalize ();

			body.velocity = phaseDirection * groundVector * speed;

		}

	}

	void StopPhasing ()
	{
		isPhasing = false;
		isRolling = false;

		EaseOutOfPhasing ();
		JumpOutOfVerticalPhasing ();
		this.gameObject.layer = 0;
	}

	void EaseOutOfPhasing ()
	{
		body.velocity = body.velocity.normalized * exitPhaseSpeed;
	}

	void JumpOutOfVerticalPhasing ()
	{
		if (phaseVerticalDireciton > 0f) {
			body.velocity = body.velocity + new Vector3 (0f, jumpSpeed, 0f);
		}
	}

	void ApplyGravity ()
	{
		float verticalSpeed = GetVerticalSpeed ();
		if (!huggingWall) {
			if (verticalSpeed > terminalArialSpeed) {
				body.AddForce (-this.transform.up * gravityForce * Time.deltaTime);
			}
		}
	}

	void ApplyWallHug(){
		if(huggingWall){
			ApplyAntiGravity ();
			ApplyWallHugForce ();
		}
	}

	void ApplyAntiGravity(){
		float verticalSpeed = GetVerticalSpeed ();
		if (verticalSpeed > terminalWallSlideSpeed) {
			body.AddForce (-this.transform.up * gravityForce * Time.deltaTime);
		}
	}

	//If in a position to grab or slide down a wall, apply a small force towards the wall
	void ApplyWallHugForce ()
	{
		body.AddForce (-contactSensor.GetGrabbedWallNormal () * wallHugForce * Time.deltaTime);
	}

	//Use the dot product to find the scalar projection of the current velocity
	//in the undesired direction.
	//Subtract this scalar * the undesired direction from the body velocity
	//to zero all movement in the diretion of the given vector
	void CancelVelocityAlongVector (Vector3 vectorToKillVelocity)
	{
		vectorToKillVelocity.Normalize ();
		float speedInUndesiredDiretion = Vector3.Dot (body.velocity, vectorToKillVelocity);
		body.velocity = body.velocity - vectorToKillVelocity * speedInUndesiredDiretion;
	}

	//Return the vector at 90 degrees to the arguement vector.
	Vector3 CalculatePerpendicular (Vector3 vec)
	{
		return new Vector3 (vec.y, -vec.x, 0);
	}

	public float GetVerticalSpeed ()
	{
		return VerticalSpeed ();
	}

	public float GetHorizontalSpeed ()
	{
		return HorizontalSpeed ();
	}

	public void SetWalkingSpeed (float newSpeed)
	{
		maxWalkSpeed = newSpeed;
	}

	public void LungeCommand (Vector3 lungeVector, float speed)
	{
		body.velocity = body.velocity + body.rotation * lungeVector * speed;
	}

	public Vector3 GetVelocity(){
		return this.body.velocity;
	}

	public void KillCommand(){
		isDead = true;
		physCollider.material = physicMaterials [(int)PhysicMatTypes.IDLE_STANDING];
		this.gameObject.layer = 9; // Go to the phasing layer, where only interact with obstacles.
	}

	public void SetWallHug(bool hug){
		huggingWall = hug;

		if (huggingWall) {
			if (contactSensor.GetHasContactStateChanged ()) {
				LimitWallClimbSpeed ();
			}
		}
	}

	public bool GetIsHuggingWall(){
		return huggingWall;
	}
}