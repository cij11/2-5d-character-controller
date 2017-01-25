using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMLoader : MonoBehaviour {

	private Dictionary<string, FSMState> states;
	private FSM rootFSM;

	private AIRaycastSensors raycastSensors;
	private AIMotorActions motorActions;
	private AIGoals goals;
	private Transform parentTransform;

	public GameObject FSMPrefab;

	private FSMState latestState; //Add transitions to the most recently added state;
	private FSMTransition latestTransition; //Add expressions to the most recently created transition

	void Start () {
		states = new Dictionary<string, FSMState> ();
	//	LoadStates ();
		SeriallyLoadStates();
		motorActions = GetComponent<AIMotorActions>() as AIMotorActions;
		raycastSensors = GetComponent<AIRaycastSensors>() as AIRaycastSensors;
		goals = GetComponent<AIGoals> () as AIGoals;
		parentTransform = this.transform.parent;

		GameObject FSMObject = Instantiate (FSMPrefab, parentTransform.position, parentTransform.rotation);
		FSMObject.transform.SetParent (this.transform.parent);
		rootFSM = FSMObject.GetComponent<FSM> () as FSM;
		rootFSM.InitialiseFSM("patrol", this, motorActions, raycastSensors, goals, parentTransform);
	}

	//Data driven approach, so that hard coding can be replaced with
	//loading from json.
	private void LoadStates()
	{
		FSMState statea = new FSMState ("patrol", Action.RUN_SUB_FSM);
		statea.SetStartingSubstate ("start_patrol");
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

		FSMState state0 = new FSMState("start_patrol", Action.IDLE);
		FSMTransition trans0 = new FSMTransition("run_right");
		trans0.AddExpression(Condition.TIMER, 0.1f, true);
		state0.AddTransition(trans0);
		states.Add(state0.GetName(), state0);

		FSMState state1 = new FSMState("run_right", Action.MOVE_RIGHT);
		FSMTransition trans2 = new FSMTransition("run_left");
		trans2.AddExpression(Condition.TIMER, 1, true);
		state1.AddTransition(trans2);
		states.Add(state1.GetName(), state1);

		FSMState state2 = new FSMState("run_left", Action.MOVE_LEFT);
		FSMTransition trans4 = new FSMTransition("run_right");
		trans4.AddExpression(Condition.TIMER, 1, true);
		state2.AddTransition(trans4);
		states.Add(state2.GetName(), state2);

		FSMState state3 = new FSMState("aim_target", Action.AIM_TARGET);
		FSMTransition trans5 = new FSMTransition("shoot_target");
		trans5.AddExpression(Condition.TIMER, 2, true);
		state3.AddTransition(trans5);
		states.Add(state3.GetName(), state3);

		FSMState state4 = new FSMState("shoot_target", Action.RELEASE_FIRE_TARGET);
		FSMTransition trans6 = new FSMTransition("aim_target");
		trans6.AddExpression(Condition.TIMER, 0.1f, true);
		state4.AddTransition(trans6);
		states.Add(state4.GetName(), state4);
	}

	private void SeriallyLoadStates(){
		AddState ("patrol", Action.RUN_SUB_FSM, "start_patrol");
		AddTransistion ("attack", Condition.TARGET_IN_RADIUS, 3, true);

		AddState ("attack", Action.RUN_SUB_FSM, "aim_target");
		AddTransistion ("patrol", Condition.TARGET_IN_RADIUS, 3, false);

		AddState ("start_patrol", Action.IDLE);
		AddTransistion ("run_right", Condition.TIMER, 1, true);

		AddState ("run_right", Action.MOVE_RIGHT);
		AddTransistion ("run_left", Condition.TIMER, 1, true);

		AddState ("run_left", Action.MOVE_LEFT);
		AddTransistion ("run_right", Condition.TIMER, 1, true);

		AddState ("aim_target", Action.AIM_TARGET);
		AddTransistion ("shoot_target", Condition.TIMER, 2, true);

		AddState ("shoot_target", Action.RELEASE_FIRE_TARGET);
		AddTransistion ("aim_target", Condition.TIMER, 0.1f, true);
	}

	//Create a new state with the given name, action, and substate.
	//Save as the latest state, for sequentially adding transitions.
	private void AddState(string name, Action action, string substate = ""){
		FSMState fsmState = new FSMState (name, action);
		fsmState.SetStartingSubstate (substate);
		states.Add (fsmState.GetName (), fsmState);
		latestState = fsmState;
	}

	//Add a transition to the most recently created state.
	//Transistion is created with at least one expression.
	private void AddTransistion(string nextState, Condition condition, float param, bool truth){
		FSMTransition transition = new FSMTransition (nextState);
		latestTransition = transition;
		latestTransition.AddExpression (condition, param, truth);
		latestState.AddTransition (transition);
	}

	//Add an expression to the most recently created transition
	private void AddExpression(Condition condition, float param, bool truth){
		latestTransition.AddExpression (condition, param, truth);
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
