using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourSteer : ProjectileBehaviour {

	public override void PerformBehaviour (CharacterComponentData compData)
	{
		base.PerformBehaviour (compData);
		Rigidbody body = projectile.GetComponent<Rigidbody> () as Rigidbody;
		Vector3 velocityVec = body.velocity;
		Vector3 perpToVelocity = new Vector3 (-velocityVec.y, velocityVec.x, 0f);
		perpToVelocity.Normalize();
		float steeringForce = 2000f;

		if (compData.GetAimingController ().GetIsAiming ()) {
			if (compData.GetAimingController ().GetHorizontalInput () == 1) {
				print ("Steering clockwise");
				body.AddForce (perpToVelocity * -steeringForce * Time.deltaTime);
			}
			if (compData.GetAimingController ().GetHorizontalInput () == -1) {
				print ("Steering counterclockwise");
				body.AddForce (perpToVelocity * steeringForce * Time.deltaTime);
			}

		}
	}
}
