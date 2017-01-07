using UnityEngine;
using System.Collections;

public class InputDispatcher : MonoBehaviour {
	
	CharacterIntegrator characterIntegrator;
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
		characterIntegrator = this.transform.parent.GetComponent<CharacterIntegrator>();
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
			characterIntegrator.MoveHorizontal(-1);
		}
		if(abstractInput.GetHorAxis() > 0){
			characterIntegrator.MoveHorizontal(1);
		}
		if(abstractInput.GetVertAxis() < 0){
			characterIntegrator.MoveVertical(-1);
		}
		if(abstractInput.GetVertAxis() > 0){
			characterIntegrator.MoveVertical(1);
		}
		if(abstractInput.GetJumpDown()){
			characterIntegrator.Jump(abstractInput.GetHorAxis(), abstractInput.GetVertAxis());
		}
		if(abstractInput.GetFireDown()){
			characterIntegrator.StartTargetting();
		}
		if(abstractInput.GetFireUp()){
			characterIntegrator.StopTargetting();
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
		if(abstractInput.GetFire()){
			firingController.SustainFire();
		}
		if(abstractInput.GetFireUp()){
			firingController.ReleaseFire();
		}
	}

	void WeaponChanging(){
		if(abstractInput.GetSwapDown()){
			weaponManager.CycleWieldable();
		}
	}
}