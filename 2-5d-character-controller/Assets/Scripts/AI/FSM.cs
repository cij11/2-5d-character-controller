using UnityEngine;
using System.Collections.Generic;

//Single responsibility: Change internal state in response to transitions evaluating to true.
public class FSM : MonoBehaviour
{
    private FSMState activeState;

    //Decision making information.
    private float timer;
    private float maxTime = 315360000; //Limit states to ten years duration.
    
	private FSMLoader loader;
    private AIMotorActions motorActions;
	private AIConditionChecker conditionChecker;
    private Transform parentTransform;

	public GameObject FSMPrefab;

	private FSM childFSM;

	public void InitialiseFSM(string startingState, FSMLoader load, AIMotorActions aiM, AIConditionChecker aiC, Transform parentT){
        timer = 0f;
		loader = load;
		activeState = loader.GetState(startingState);

		motorActions = aiM;
		conditionChecker = aiC;
		parentTransform = parentT;

		if (activeState.GetAction () == Action.RUN_SUB_FSM) {
			SpawnFSMStartingWithState (activeState.GetStartingSubstate());
		}
    }

    public void FSMUpdate()
    {
        UpdateTimer();
        EvaluateTransitions();
		PerformStateAction ();
    }

    private void UpdateTimer()
    {
        if (timer < maxTime)
            timer += Time.deltaTime;
    }

	//Check each transistion to see if state should change
	//Each transition has a list of Expressions. Every expression in the transition must be true to take the transition.
    private void EvaluateTransitions()
    {
        List<FSMTransition> transitions = activeState.GetTransitions();
        bool transitionTruth = false;
        string nextState = null;
        foreach (FSMTransition transition in transitions)
        {
            transitionTruth = TestTransition(transition);
            if(transitionTruth){
                nextState = transition.GetState();
                break;
            }
        }

        if (transitionTruth)
        {
            ChangeToState(nextState);
        }
    }

    //A transition is true if all of its expressions evaluate to true.
    //eg, if all of the expression have their conditions evaluate to the same bool as their truth value.
    private bool TestTransition(FSMTransition transition){
        foreach(FSMExpression expression in transition.GetExpressions())
        {
            bool conditionTruth = conditionChecker.TestCondition(expression.condition, expression.param, timer);
            if(conditionTruth != expression.trueIfConditionTrue) return false;
        }
        return true;
    }

    void ChangeToState(string nextState)
    {
        timer = 0f;
        activeState = loader.GetState(nextState);
		if (activeState.GetAction () == Action.RUN_SUB_FSM) { //If this state is parent to a sub fsm
			SpawnFSMStartingWithState (activeState.GetStartingSubstate());
		}
    }

	private void SpawnFSMStartingWithState (string startingSubstate){
		if(childFSM != null)Destroy (childFSM.gameObject);

		GameObject FSMObject = Instantiate (FSMPrefab, parentTransform.position, parentTransform.rotation);
		FSMObject.transform.SetParent (this.transform.parent);
		childFSM = FSMObject.GetComponent<FSM> () as FSM;
		childFSM.InitialiseFSM(startingSubstate, loader, motorActions, conditionChecker, parentTransform);
	}

	void PerformStateAction(){
		print ("Active state = " + activeState.GetName ());
		if (childFSM != null) {
			childFSM.FSMUpdate ();
		} else {
			motorActions.PerformAction (activeState.GetAction ());
		}
	}

    public Action GetAction(){
        return activeState.GetAction();
    }
}