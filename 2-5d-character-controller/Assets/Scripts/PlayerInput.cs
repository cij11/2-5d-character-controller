using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	
	MovementController movementController;
	AimingController aimingController;
	FiringController firingController;

	float epsilon = 0.001f;


	// Use this for initialization
	void Start () {
		RegisterControllers();
	}

	//Look for the 
	void RegisterControllers(){
		GameObject actionControllers = this.transform.parent.FindChild("ActionControllers").gameObject;
		movementController = actionControllers.GetComponent<MovementController>();
		aimingController = actionControllers.GetComponent<AimingController>();
		firingController = actionControllers.GetComponent<FiringController>();
	}
	
	// Update is called once per frame
	void Update () {
		Movement();
		Aiming();
		Firing();
	}

	void Movement(){
		if(Input.GetAxis("Horizontal") < 0){
			movementController.MoveHorizontal(-1);
		}
		if(Input.GetAxis("Horizontal") > 0){
			movementController.MoveHorizontal(1);
		}
		if(Input.GetAxis("Vertical") < 0){
			movementController.MoveVertical(-1);
		}
		if(Input.GetAxis("Vertical") > 0){
			movementController.MoveVertical(1);
		}
		if(Input.GetButtonDown("Jump")){
			movementController.Jump();
		}
		if(Input.GetButtonDown("Fire1")){
			movementController.StartTargetting();
		}
		if(Input.GetButtonUp("Fire1")){
			movementController.StopTargetting();
		}
	}

	void Aiming(){
		if(Mathf.Abs(Input.GetAxis("Horizontal")) > epsilon){
			aimingController.SetHorizontalInput(Mathf.Sign(Input.GetAxis("Horizontal")));
		}
		else {
			aimingController.SetHorizontalInput(0f);
		}

		if(Mathf.Abs(Input.GetAxis("Vertical")) > epsilon){
			aimingController.SetVerticalInput(Mathf.Sign(Input.GetAxis("Vertical")));
		}
		else {
			aimingController.SetVerticalInput(0f);
		}

		
	}

	void Firing(){
		if(Input.GetButtonDown("Fire1")){
			firingController.InitiateFire();
		}
		if(Input.GetButton("Fire1")){
			firingController.SustainFire();
		}
		if(Input.GetButtonUp("Fire1")){
			firingController.ReleaseFire();
		}
	}
}