using UnityEngine;
using System.Collections.Generic;

//Single responsibility: Change internal state in response to transitions evaluating to true.
public class FSM : MonoBehaviour
{
    private FSMState activeState;

    //Decision making information.
    private float timer;
    private float maxTime = 315360000; //Limit states to ten years duration.
    
    //Horizontal and vertical directions pointing to the closest octant to the target
    int horOctant;
    int vertOctant;
	private FSMLoader loader;
    private AIRaycastSensors raycastSensors;
    private AIMotorActions motorActions;
	private AIGoals goals;
    private Transform parentTransform;

	public GameObject FSMPrefab;

	private FSM childFSM;

	public void InitialiseFSM(string startingState, FSMLoader load, AIMotorActions aiM, AIRaycastSensors aiR, AIGoals aiG, Transform parentT){
        timer = 0f;
		loader = load;
		activeState = loader.GetState(startingState);

		motorActions = aiM;
		raycastSensors = aiR;
		goals = aiG;
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
            bool conditionTruth = TestCondition(expression.condition, expression.param);
            if(conditionTruth != expression.trueIfConditionTrue) return false;
        }
        return true;
    }

    private bool TestCondition(Condition condition, float param)
    {
        switch (condition)
        {
            case Condition.TIMER:
                {
                    if (timer > param) return true;
                    break;
                }
            case Condition.TARGET_IN_RADIUS:
            {
				if(Vector3.Magnitude(goals.GetTargetObject().transform.position - parentTransform.position) < param) return true;
                break;
            }
            case Condition.TARGET_OUTSIDE_RADIUS:
            {
				if(Vector3.Magnitude(goals.GetTargetObject().transform.position - parentTransform.position) > param) return true;
                break;
            }
            case Condition.TARGET_IN_LOS:
            {
				if (raycastSensors.IsGameobjectInLOS(goals.GetTargetObject())) return true;
                break;
            }
            case Condition.TARGET_IN_OCTANT_BELOW:
            {
                FindTargetOctant();
                if(horOctant == 0f && vertOctant <0) return true;
                break;
            }
            case Condition.TARGET_IN_OCTANT_ABOVE:
            {
                FindTargetOctant();
                if(horOctant == 0f && vertOctant >0) return true;
                break;
            }
            case Condition.TARGET_IN_HORIZONTAL_OCTANTS:
            {
                FindTargetOctant();
                if(horOctant != 0f && vertOctant == 0) return true;
                break;
            }
            case Condition.CLIFF_LEFT:
            {
                if(raycastSensors.GetLeftCliff()) return true;
                break;
            }
            case Condition.CLIFF_RIGHT:
            {
                if(raycastSensors.GetRightCliff()) return true;
                break;
            }
        }
        return false;
    }

    void FindTargetOctant(){
		Octant.PointsToOctant(parentTransform.position, goals.GetTargetObject().transform.position,
                  parentTransform.right, parentTransform.up, out horOctant, out vertOctant);
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
		childFSM.InitialiseFSM(startingSubstate, loader, motorActions, raycastSensors, goals, parentTransform);
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