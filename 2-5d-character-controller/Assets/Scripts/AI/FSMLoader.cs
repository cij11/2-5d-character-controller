using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMLoader : MonoBehaviour {

	private Dictionary<string, FSMState> states;

	private FSMState latestState; //Add transitions to the most recently added state;
	private FSMTransition latestTransition; //Add expressions to the most recently created transition

	private string startingStateName;

	public void InitialiseLoader(){
		states = new Dictionary<string, FSMState> ();
		SeriallyLoadStates();
		startingStateName = "patrol";
	}

	protected virtual void SeriallyLoadStates(){
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
	protected void AddState(string name, Action action, string substate = ""){
		FSMState fsmState = new FSMState (name, action);
		fsmState.SetStartingSubstate (substate);
		states.Add (fsmState.GetName (), fsmState);
		latestState = fsmState;
	}

	//Add a transition to the most recently created state.
	//Transistion is created with at least one expression.
	protected void AddTransistion(string nextState, Condition condition, float param, bool truth){
		FSMTransition transition = new FSMTransition (nextState);
		latestTransition = transition;
		latestTransition.AddExpression (condition, param, truth);
		latestState.AddTransition (transition);
	}

	//Add an expression to the most recently created transition
	protected void AddExpression(Condition condition, float param, bool truth){
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

	public string GetStartingStateName(){
		return startingStateName;
	}
}
