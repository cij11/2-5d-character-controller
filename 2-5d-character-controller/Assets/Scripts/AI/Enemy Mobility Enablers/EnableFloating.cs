using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFloating : MonoBehaviour {
	CharacterMovementActuator movementActuator;
	AimingController aimingController;

	public bool floatsTowardsAiming = true;
	public bool hoversInPlace = false;

	Vector3 homePosition;

	// Use this for initialization
	void Start () {
		movementActuator = GetComponentInParent<CharacterMovementActuator> () as CharacterMovementActuator;
		aimingController = this.transform.parent.GetComponentInChildren<AimingController> () as AimingController;
		homePosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(floatsTowardsAiming)
			movementActuator.FloatCommand (aimingController.GetHorizontalInput (), aimingController.GetVerticalAiming (), 100f);
		if (hoversInPlace) {
			float hor = Mathf.Sign (homePosition.x - this.transform.position.x);
			float vert = Mathf.Sign (homePosition.y - this.transform.position.y);
			movementActuator.FloatCommand (hor, vert, 3f);
		}
	//	movementActuator.JetpackCommand ();
	}
}
