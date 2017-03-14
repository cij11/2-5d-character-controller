using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFloating : MonoBehaviour {
	CharacterMovementActuator movementActuator;
	AimingController aimingController;
	// Use this for initialization
	void Start () {
		movementActuator = GetComponentInParent<CharacterMovementActuator> () as CharacterMovementActuator;
		aimingController = this.transform.parent.GetComponentInChildren<AimingController> () as AimingController;
	}
	
	// Update is called once per frame
	void Update () {
		movementActuator.FloatCommand (aimingController.GetHorizontalInput (), aimingController.GetVerticalAiming (), 10f);
	//	movementActuator.JetpackCommand ();
	}
}
