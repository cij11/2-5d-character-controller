using UnityEngine;
using System.Collections;

public class InputDispatcher : MonoBehaviour {
	
	MovementController movementController;
	AimingController aimingController;
	FiringController firingController;
	ItemManager itemManager;

	AbstractInput abstractInput;

	CharacterCorpus corpus;

	// Use this for initialization
	void Start () {
		RegisterControllers();
	}

	void RegisterControllers(){
		GameObject actionControllers = this.transform.parent.FindChild("ActionControllers").gameObject;
		movementController = actionControllers.GetComponent<MovementController>();
		aimingController = actionControllers.GetComponent<AimingController>();
		firingController = actionControllers.GetComponent<FiringController>();
		itemManager = actionControllers.GetComponent<ItemManager>();
		corpus = GetComponentInParent<CharacterCorpus> () as CharacterCorpus;
	}

	public void SetControllingAbstractInput(AbstractInput controllingInput){
		abstractInput = controllingInput;
	}
	
	// Update is called once per frame
	void Update () {
		if (corpus.GetIsAlive ()) {
			UpdateAbstractInput ();
			Movement ();
			Aiming ();
			Firing ();
			Items ();
		}
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

		firingController.SetFireHeld (abstractInput.GetFire ());
	}

	void Items(){
		if(abstractInput.GetSwapDown()){
			itemManager.StartSwap ();
		}

		if(abstractInput.GetFireDown()){
			itemManager.ThrowItemIfSwapping();
		}

		if (abstractInput.GetSwapUp ()) {
			itemManager.DischargeSwap ();
		}
	}
}