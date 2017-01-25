using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

	private FSMLoader loader;
	private FSM rootFSM;

	private AIMotorActions motorActions;
	private AIConditionChecker conditionChecker;
	private Transform parentTransform;

	public GameObject FSMPrefab;

	void Start () {
		//Load the states and transitions into the FSMLoader
		loader = GetComponent<FSMLoader>() as FSMLoader;
		loader.InitialiseLoader ();

		//Store references to commonly used components
		motorActions = GetComponent<AIMotorActions>() as AIMotorActions;
		conditionChecker = GetComponent<AIConditionChecker> () as AIConditionChecker;
		parentTransform = this.transform.parent;

		//Instantiate the root FSM
		GameObject FSMObject = Instantiate (FSMPrefab, parentTransform.position, parentTransform.rotation);
		FSMObject.transform.SetParent (this.transform.parent);
		rootFSM = FSMObject.GetComponent<FSM> () as FSM;
		rootFSM.InitialiseFSM(loader.GetStartingStateName(), loader, motorActions, conditionChecker, parentTransform);
	}

	void Update () {
		rootFSM.FSMUpdate ();
	}
}
