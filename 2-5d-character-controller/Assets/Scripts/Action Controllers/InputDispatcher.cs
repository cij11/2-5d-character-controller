using UnityEngine;
using System.Collections;

public class InputDispatcher : MonoBehaviour {
	
	MovementController movementController;
	AimingController aimingController;
	FiringController firingController;
	WeaponManager weaponManager;

	AbstractInput abstractInput;

	// Use this for initialization
	void Start () {
		RegisterControllers();
	}

	void RegisterControllers(){
		GameObject actionControllers = this.transform.parent.FindChild("ActionControllers").gameObject;
		movementController = actionControllers.GetComponent<MovementController>();
		aimingController = actionControllers.GetComponent<AimingController>();
		firingController = actionControllers.GetComponent<FiringController>();
		weaponManager = actionControllers.GetComponent<WeaponManager>();
	}

	public void SetControllingAbstractInput(AbstractInput controllingInput){
		abstractInput = controllingInput;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateAbstractInput();
		Movement();
		Aiming();
		Firing();
		WeaponChanging();
	}

	void UpdateAbstractInput(){
		abstractInput.UpdateInput();
	}

	void Movement(){
		if(abstractInput.GetHorAxis() < 0){
			movementController.MoveHorizontal(-1);
		}
		if(abstractInput.GetHorAxis() > 0){
			movementController.MoveHorizontal(1);
		}
		if(abstractInput.GetJumpDown()){
			movementController.Jump(abstractInput.GetHorAxis(), abstractInput.GetVertAxis());
		}
		if(abstractInput.GetFireDown()){
			movementController.StartTargetting();
		}
		if(abstractInput.GetFireUp()){
			movementController.StopTargetting();
		}
	}

	void Aiming(){
		aimingController.SetHorizontalInput(abstractInput.GetHorAxis());
		aimingController.SetVerticalInput(abstractInput.GetVertAxis());

		if(abstractInput.GetFireDown()){
			aimingController.StartTargetting();
		}
		if(abstractInput.GetFireUp()){
			aimingController.StopTargetting();
		}
	}

	void Firing(){
		if(abstractInput.GetFireDown()){
			firingController.InitiateFire();
		}

		if(abstractInput.GetFireUp()){
			firingController.ReleaseFire();
		}
	}

	void WeaponChanging(){
		if(abstractInput.GetSwapDown()){
			if (abstractInput.GetFire ()) {
				weaponManager.ThrowWeapon ();
			} else {
				weaponManager.CycleWieldable ();
			}
		}
	}
}