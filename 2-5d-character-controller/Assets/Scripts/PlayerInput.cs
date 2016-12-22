using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	
	MovementController movementController;
	AimingController aimingController;
	FiringController firingController;


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
		if(Input.GetKeyDown("space")){
			movementController.Jump();
		}
	}

	void Aiming(){
		if(Input.GetAxis("Horizontal") < 0){
			aimingController.SetHorizontal(-1);
		}
		else if(Input.GetAxis("Horizontal") > 0){
			aimingController.SetHorizontal(1);
		}
		else{
			aimingController.SetHorizontal(0);
		}

		if(Input.GetAxis("Vertical") < 0){
			aimingController.SetVertical(-1);
		}
		else if(Input.GetAxis("Vertical") > 0){
			aimingController.SetVertical(1);
		}
		else{
			aimingController.SetVertical(0);
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