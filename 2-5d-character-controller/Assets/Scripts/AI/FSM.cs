using UnityEngine;
using System.Collections.Generic;

//Single responsibility: Change internal state in response to transitions evaluating to true.
public class FSM : MonoBehaviour
{
    private FSMState activeState;

    //Decision making information.
	private FSMCounter fsmCounter;

    private float maxTime = 315360000; //Limit states to ten years duration.
    
	private FSMLoader loader;
    private AIMotorActions motorActions;
	private AIConditionChecker conditionChecker;
    private Transform parentTransform;

	public GameObject FSMPrefab;

	private FSM childFSM;

	public void InitialiseFSM(string startingState, FSMLoader load, AIMotorActions aiM, AIConditionChecker aiC, Transform parentT){
		loader = load;
		activeState = loader.GetState(startingState);

		motorActions = aiM;
		conditionChecker = aiC;
		parentTransform = parentT;

		fsmCounter = new FSMCounter ();
		fsmCounter.timer = 0f;
		fsmCounter.frames = 0;

		if (activeState.GetAction () == Action.RUN_SUB_FSM) {
			SpawnFSMStartingWithState (activeState.GetStartingSubstate());
		}
    }

    public void FSMUpdate()
    {
        UpdateCounter();
        EvaluateTransitions();
		PerformStateAction ();
    }

	public void UpdateCounter(){
		UpdateTimer ();
		UpdateFrames ();
	}

    private void UpdateTimer()
    {
        if (fsmCounter.timer < maxTime)
			fsmCounter.timer += Time.deltaTime;
    }
	private void UpdateFrames(){
		fsmCounter.frames++;
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
			bool conditionTruth = conditionChecker.TestCondition(expression.condition, expression.param, fsmCounter);
            if(conditionTruth != expression.trueIfConditionTrue) return false;
        }
        return true;
    }

    void ChangeToState(string nextState)
    {
		ZeroCounter ();
        activeState = loader.GetState(nextState);
		if (activeState.GetAction () == Action.RUN_SUB_FSM) { //If this state is parent to a sub fsm
			SpawnFSMStartingWithState (activeState.GetStartingSubstate());
		}
    }

	void ZeroCounter(){
		fsmCounter.timer = 0f;
		fsmCounter.frames = 0;
	}

	private void SpawnFSMStartingWithState (string startingSubstate){
		if(childFSM != null)Destroy (childFSM.gameObject);

		GameObject FSMObject = Instantiate (FSMPrefab, parentTransform.position, parentTransform.rotation);
		FSMObject.transform.SetParent (this.transform.parent);
		childFSM = FSMObject.GetComponent<FSM> () as FSM;
		childFSM.InitialiseFSM(startingSubstate, loader, motorActions, conditionChecker, parentTransform);
	}

	void PerformStateAction(){
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