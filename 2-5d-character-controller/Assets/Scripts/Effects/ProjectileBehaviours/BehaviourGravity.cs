using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourGravity : ProjectileBehaviour {
	public float gravityForce = 500f;
	public override void PerformBehaviour (CharacterComponentData compData)
	{
		base.PerformBehaviour (compData);
		Rigidbody body = projectile.GetComponent<Rigidbody> () as Rigidbody;
		body.AddForce (GravityManager.instance.GetDownVector (body.transform.position) * gravityForce * Time.deltaTime);
	}
}