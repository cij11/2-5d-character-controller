using UnityEngine;
using System.Collections;

public class RigidBodyController : MonoBehaviour {
	public Vector3 gravityFocus = new Vector3(0, 0, 0);
	Rigidbody body;

	float walkPower = 2;

	//Speed the character walks at when left or right held
	float maxWalkSpeed = 12;
	//Time to attain walk velocity from stationary.
	float speedUpMagnifier = 20f; 
	float breakPower = 60f;
	float gravityPower = 14f;

	float jumpVelocity = 10f;

	CollisionInfo collisions;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		OrientToGravityFocus();
		Move();
		ApplyGravity();

		collisions.Reset();
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

	void Move(){
		if(Input.GetKey("a")){
			Walk(-1);
		}
		if(Input.GetKey("d")){
			Walk(1);
		}

		//Jump
		if(Input.GetKeyDown("space")){
			Vector3 jumpBoost = transform.up * jumpVelocity;
			Vector3 oldVel = body.velocity;
			body.velocity = oldVel + jumpBoost;
		}
	}

	void Walk(float direction){
		//Get the current velocity in the desired direction
		float currentWalkSpeed = Vector3.Dot(body.velocity, direction*body.transform.right);

		print(currentWalkSpeed);
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

	 public struct CollisionInfo{
        public bool above, below;
        public bool left, right;

        public void Reset(){
            above = below = false;
            left = right = false;
        }
	 }
}
