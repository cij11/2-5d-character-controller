using UnityEngine;
using System.Collections.Generic;

//Single responsibility: Change internal state in response to transitions evaluating to true.
public class FSM : MonoBehaviour
{
    private FSMState activeState;
    private Dictionary<string, FSMState> states;
    private FSM parentFSM = null;

    //Decision making information.
    private float timer;
    private float maxTime = 315360000; //Limit states to ten years duration.
    private GameObject targetObject;
    
    //Horizontal and vertical directions pointing to the closest octant to the target
    float horOctant;
    float vertOctant;
    private AIRaycastSensors raycastSensors;
    private AIMotorActions motorActions;
    private Transform parentTransform;

    void Start(){
        timer = 0f;
        states = new Dictionary<string, FSMState>();
        LoadStates();
        activeState = GetState("run_right");

        motorActions = GetComponent<AIMotorActions>() as AIMotorActions;
        raycastSensors = GetComponent<AIRaycastSensors>() as AIRaycastSensors;
        parentTransform = this.transform.parent;
    }

    //Data driven approach, so that hard coding can be replaced with
    //loading from XML.
    private void LoadStates()
    {
        FSMState state1 = new FSMState("run_right", Action.MOVERIGHT);
        FSMTransition trans1 = new FSMTransition("aim_target");
        trans1.AddExpression(Condition.TARGET_IN_OCTANT_BELOW, 2, true);
        state1.AddTransition(trans1);
        FSMTransition trans2 = new FSMTransition("run_left");
        trans2.AddExpression(Condition.CLIFF_RIGHT, 0, true);
        state1.AddTransition(trans2);
        states.Add(state1.GetName(), state1);

        FSMState state2 = new FSMState("run_left", Action.MOVELEFT);
        FSMTransition trans3 = new FSMTransition("aim_target");
        trans3.AddExpression(Condition.TARGET_IN_OCTANT_BELOW, 2, true);
        state2.AddTransition(trans3);
        FSMTransition trans4 = new FSMTransition("run_right");
        trans4.AddExpression(Condition.CLIFF_LEFT, 0, true);
        state2.AddTransition(trans4);
        states.Add(state2.GetName(), state2);

        FSMState state3 = new FSMState("aim_target", Action.AIMTARGET);
        FSMTransition trans5 = new FSMTransition("shoot_target");
        trans5.AddExpression(Condition.TIMER, 2, true);
        state3.AddTransition(trans5);
        states.Add(state3.GetName(), state3);

        FSMState state4 = new FSMState("shoot_target", Action.RELEASEFIRETARGET);
        FSMTransition trans6 = new FSMTransition("run_right");
        trans6.AddExpression(Condition.TIMER, 0.1f, true);
        state4.AddTransition(trans6);
        states.Add(state4.GetName(), state4);
    }

    void Update()
    {
        UpdateTimer();
        EvaluateTransitions();
		motorActions.PerformAction (GetAction ());
    }

    private void UpdateTimer()
    {
        if (timer < maxTime)
            timer += Time.deltaTime;
    }

    private void EvaluateTransitions()
    {
        EvaluateParentTransitions(); //Check if a higher level FSM is changing state

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

    private void EvaluateParentTransitions(){
        if (parentFSM != null){
            parentFSM.EvaluateTransitions();
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
                if(Vector3.Magnitude(targetObject.transform.position - parentTransform.position) < param) return true;
                break;
            }
            case Condition.TARGET_OUTSIDE_RADIUS:
            {
                if(Vector3.Magnitude(targetObject.transform.position - parentTransform.position) > param) return true;
                break;
            }
            case Condition.TARGET_IN_LOS:
            {
                if (raycastSensors.IsGameobjectInLOS(targetObject)) return true;
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
                Octant.PointsToOctant(parentTransform.position, targetObject.transform.position,
                  parentTransform.right, parentTransform.up, out horOctant, out vertOctant);
    }

    void ChangeToState(string nextState)
    {
        timer = 0f;
        activeState = GetState(nextState);
    }

    private FSMState GetState(string stateName)
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

    public Action GetAction(){
        return activeState.GetAction();
    }

    public void SetTarget(GameObject target){
        targetObject = target;
    }
}