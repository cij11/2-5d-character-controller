using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMLoader : MonoBehaviour {

	private Dictionary<string, FSMState> states;
	private FSM rootFSM;

	private AIRaycastSensors raycastSensors;
	private AIMotorActions motorActions;
	private Transform parentTransform;

	public GameObject FSMPrefab;

	void Start () {
		states = new Dictionary<string, FSMState> ();
		LoadStates ();
		motorActions = GetComponent<AIMotorActions>() as AIMotorActions;
		raycastSensors = GetComponent<AIRaycastSensors>() as AIRaycastSensors;
		parentTransform = this.transform.parent;

		GameObject FSMObject = Instantiate (FSMPrefab, parentTransform.position, parentTransform.rotation);
		FSMObject.transform.SetParent (this.transform.parent);
		rootFSM = FSMObject.GetComponent<FSM> () as FSM;
		rootFSM.InitialiseFSM("patrol", this, motorActions, raycastSensors, parentTransform);
	}

	//Data driven approach, so that hard coding can be replaced with
	//loading from json.
	private void LoadStates()
	{
		FSMState statea = new FSMState ("patrol", Action.RUN_SUB_FSM);
		statea.SetStartingSubstate ("run_right");
		FSMTransition transa1 = new FSMTransition ("attack");
		transa1.AddExpression(Condition.TARGET_IN_RADIUS, 3, true);
		statea.AddTransition (transa1);
		states.Add (statea.GetName(), statea);

		FSMState stateb = new FSMState ("attack", Action.RUN_SUB_FSM);
		stateb.SetStartingSubstate ("aim_target");
		FSMTransition transb1 = new FSMTransition ("patrol");
		transb1.AddExpression(Condition.TARGET_IN_RADIUS, 3, false);
		stateb.AddTransition (transb1);
		states.Add (stateb.GetName(), stateb);

		FSMState state1 = new FSMState("run_right", Action.MOVERIGHT);
		FSMTransition trans2 = new FSMTransition("run_left");
		trans2.AddExpression(Condition.TIMER, 1, true);
		state1.AddTransition(trans2);
		states.Add(state1.GetName(), state1);

		FSMState state2 = new FSMState("run_left", Action.MOVELEFT);
		FSMTransition trans4 = new FSMTransition("run_right");
		trans4.AddExpression(Condition.TIMER, 1, true);
		state2.AddTransition(trans4);
		states.Add(state2.GetName(), state2);

		FSMState state3 = new FSMState("aim_target", Action.AIMTARGET);
		FSMTransition trans5 = new FSMTransition("shoot_target");
		trans5.AddExpression(Condition.TIMER, 2, true);
		state3.AddTransition(trans5);
		states.Add(state3.GetName(), state3);

		FSMState state4 = new FSMState("shoot_target", Action.RELEASEFIRETARGET);
		FSMTransition trans6 = new FSMTransition("aim_target");
		trans6.AddExpression(Condition.TIMER, 0.1f, true);
		state4.AddTransition(trans6);
		states.Add(state4.GetName(), state4);
	}

	public FSMState GetState(string stateName)
	{
		if (states.ContainsKey(stateName))
		{
			return states[stateName];
		}
		else
		{
			Debug.Log("A state with name " + stateName + " is not in FSM states dictionary.");
			return null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		rootFSM.FSMUpdate();
	}
}
