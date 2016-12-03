using UnityEngine;
using System.Collections;

public class RigidBodyController : MonoBehaviour {
	public Vector3 gravityFocus = new Vector3(0, 0, 0);
	Rigidbody body;
	float width = 1f;
	float height = 1f;

	float walkPower = 2;

	//Speed the character walks at when left or right held
	float maxWalkSpeed = 12;
	//Time to attain walk velocity from stationary.
	float speedUpMagnifier = 20f; 
	float breakPower = 60f;
	float gravityPower = 14f;

	float jumpVelocity = 10f;

	CollisionInfo collisions;
	RaycastOrigins raycastOrigins;
	float skinWidth = 0.01f;
	int verticalRayCount = 3;
	float verticalRaySpacing;
	public LayerMask collisionMask;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody>();
		UpdateRaycastOrigins();
		CalculateRaySpacing();
	}
	
	// Update is called once per frame
	void Update () {
		OrientToGravityFocus();
		Move();
		ApplyGravity();

		collisions.Reset();
		UpdateRaycastOrigins();
		DetectGround();
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
			}
		}
		if(collisions.below) print("Collision below");
		else print("No collision below");
	}

	//Horizontal rays detect slopes and walls
	void DetectSlopeAndWalls(){

	}

	void Move(){
		if(Input.GetKey("a")){
			Walk(-1);
		}
		if(Input.GetKey("d")){
			Walk(1);
		}

		//Jump
		if(Input.GetKeyDown("space")){
			//Only allow jumping if standing on something
			if (collisions.below){
				Vector3 jumpBoost = transform.up * jumpVelocity;
				Vector3 oldVel = body.velocity;
				body.velocity = oldVel + jumpBoost;
			}
		}
	}

	void Walk(float direction){
		//Get the current velocity in the desired direction
		float currentWalkSpeed = Vector3.Dot(body.velocity, direction*body.transform.right);

		//dot product of desired and current velocity will be negative if walking the wrong way
		if (Mathf.Sign(currentWalkSpeed) < 0){
			Break(direction);
		}
		else if (currentWalkSpeed < maxWalkSpeed){
			body.AddForce(this.transform.right * direction * speedUpMagnifier);
		}
	}

	void Break(float direction){
		body.AddForce(this.transform.right *direction * breakPower);
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
        public bool left, right;
		public float slopeAngle;
		public float oldSlopeAngle;

        public void Reset(){
            above = below = false;
            left = right = false;
			oldSlopeAngle = slopeAngle;

        }
	 }

	 //Vectors in worldspace where rays are cast from, coresponding to corners of the square.
	struct RaycastOrigins{
		public Vector3 topLeft, topRight;
		public Vector3 bottomLeft, bottomRight;
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
