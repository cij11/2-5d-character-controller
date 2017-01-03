using UnityEngine;
using System.Collections;

public class AIInput : MonoBehaviour {
	
	CharacterIntegrator characterIntegrator;
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
		characterIntegrator = this.transform.parent.GetComponent<CharacterIntegrator>();
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

			characterIntegrator.MoveHorizontal(-1);

	}

	void Aiming(){

	}

	void Firing(){

	}
}