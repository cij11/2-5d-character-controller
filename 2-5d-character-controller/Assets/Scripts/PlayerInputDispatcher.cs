using UnityEngine;
using System.Collections;

public class PlayerInputDispatcher : MonoBehaviour {
	
	CharacterIntegrator characterIntegrator;
	AimingController aimingController;
	FiringController firingController;
	WeaponManager weaponManager;

	VirtualInput virtualInput;

	// Use this for initialization
	void Start () {
		RegisterControllers();
		virtualInput = new PlayerVirtualInput();
	}

	//Look for the 
	void RegisterControllers(){
		GameObject actionControllers = this.transform.parent.FindChild("ActionControllers").gameObject;
		characterIntegrator = this.transform.parent.GetComponent<CharacterIntegrator>();
		aimingController = actionControllers.GetComponent<AimingController>();
		firingController = actionControllers.GetComponent<FiringController>();
		weaponManager = actionControllers.GetComponent<WeaponManager>();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateVirtualInput();
		Movement();
		Aiming();
		Firing();
		WeaponChanging();
	}

	void UpdateVirtualInput(){
		virtualInput.UpdateInput();
	}

	void Movement(){
		if(virtualInput.GetHorAxis() < 0){
			characterIntegrator.MoveHorizontal(-1);
		}
		if(virtualInput.GetHorAxis() > 0){
			characterIntegrator.MoveHorizontal(1);
		}
		if(virtualInput.GetVertAxis() < 0){
			characterIntegrator.MoveVertical(-1);
		}
		if(virtualInput.GetVertAxis() > 0){
			characterIntegrator.MoveVertical(1);
		}
		if(virtualInput.GetJumpDown()){
			characterIntegrator.Jump(virtualInput.GetHorAxis(), virtualInput.GetVertAxis());
		}
		if(virtualInput.GetFireDown()){
			characterIntegrator.StartTargetting();
		}
		if(virtualInput.GetFireUp()){
			characterIntegrator.StopTargetting();
		}
	}

	void Aiming(){
		aimingController.SetHorizontalInput(virtualInput.GetHorAxis());
		aimingController.SetVerticalInput(virtualInput.GetVertAxis());

		if(virtualInput.GetFireDown()){
			aimingController.StartTargetting();
		}
		if(virtualInput.GetFireUp()){
			aimingController.StopTargetting();
		}
	}

	void Firing(){
		if(virtualInput.GetFireDown()){
			firingController.InitiateFire();
		}
		if(virtualInput.GetFire()){
			firingController.SustainFire();
		}
		if(virtualInput.GetFireUp()){
			firingController.ReleaseFire();
		}
	}

	void WeaponChanging(){
		if(virtualInput.GetSwapDown()){
			weaponManager.CycleWieldable();
		}
	}
}