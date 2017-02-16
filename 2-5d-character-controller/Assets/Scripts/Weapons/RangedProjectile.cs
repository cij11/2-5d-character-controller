using UnityEngine;
using System.Collections;

public class RangedProjectile : Projectile {

	public float muzzleSpeed = 20f;
	Rigidbody body;
		
	public override void Launch(){
		LaunchRanged();
	}

	void LaunchRanged(){
		AddHorizontalSpeedInDirectionOfFiring ();
		Physics.IgnoreCollision(GetComponent<Collider>(), componentData.GetCharacter().GetComponent<Collider>());
	//	this.GetComponent<Rigidbody>().velocity = velocityVector  * muzzleSpeed;
	}

	void AddHorizontalSpeedInDirectionOfFiring(){
		body = GetComponent<Rigidbody> () as Rigidbody;

		float horizontalSpeed = componentData.GetMovementActuator ().GetHorizontalSpeed ();
		Vector3 movingTowardsFireBoost = new Vector3 (0f, 0f, 0f);
		if (Mathf.Sign (horizontalSpeed) == Mathf.Sign (componentData.GetAimingController ().GetAimingVector ().x)) {
			//If the character is moving in the direction it's firing, boost the speed of the shot.
			movingTowardsFireBoost = body.transform.right * horizontalSpeed;
		}
		body.velocity = worldLaunchVector * muzzleSpeed + movingTowardsFireBoost;
	}

	void OnCollisionEnter(Collision other) {
		ResolveCollision (other.gameObject);
    }
}
