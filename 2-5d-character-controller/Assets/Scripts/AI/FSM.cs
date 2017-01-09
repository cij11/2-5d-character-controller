using UnityEngine;
using System.Collections.Generic;

public class FSM
{
    private FSMState activeState;
    private Dictionary<string, FSMState> states;

    private float timer;
    private float maxTime = 315360000; //Limit states to ten years duration.

    public FSM()
    {
        timer = 0f;
        states = new Dictionary<string, FSMState>();
        LoadStates();
        activeState = GetState("idle");
    }

    //Data driven approach, so that hard coding can be replaced with
    //loading from XML.
    private void LoadStates()
    {
        FSMState state1 = new FSMState("idle");
        state1.AddAction(MotorAction.IDLE);
        state1.AddTransition(Condition.TIMER, 2, "run_left");
        states.Add(state1.GetName(), state1);

        FSMState state2 = new FSMState("run_left");
        state2.AddAction(MotorAction.MOVELEFT);
        state2.AddTransition(Condition.TIMER, 5, "target_player");
        states.Add(state2.GetName(), state2);

        FSMState state3 = new FSMState("target_player");
        state3.AddAction(MotorAction.AIMTARGET);
        state3.AddTransition(Condition.TIMER, 5, "shoot_player");
        states.Add(state3.GetName(), state3);

        FSMState state4 = new FSMState("shoot_player");
        state4.AddAction(MotorAction.RELEASEFIRETARGET);
        state4.AddTransition(Condition.TIMER, 0.1f, "idle");
        states.Add(state4.GetName(), state4);
    }

    public void Update()
    {
        UpdateTimer();
        EvaluateTransitions();
    }

    private void UpdateTimer()
    {
        if (timer < maxTime)
            timer += Time.deltaTime;
    }

    private void EvaluateTransitions()
    {
        List<FSMTransition> transitions = activeState.GetTransitions();
        bool conditionTruth = false;
        string nextState = null;
        foreach (FSMTransition transition in transitions)
        {
            conditionTruth = TestCondition(transition.GetCondition(), transition.GetParam());
            if (conditionTruth)
            {
                nextState = transition.GetState();
                break;
            }
        }

        if (conditionTruth)
        {
            ChangeToState(nextState);
        }
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
        }
        return false;
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

    public MotorAction GetAction(){
        return activeState.GetAction();
    }
}