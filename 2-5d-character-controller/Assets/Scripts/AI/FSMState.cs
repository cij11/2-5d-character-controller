using UnityEngine;
using System.Collections.Generic;

//Data object representing a state of the AI finite state machine.
public class FSMState{
    
    string name;
    MotorAction action;
    List<FSMTransition> transitions;

    public FSMState(string newName){
        name = newName;
        transitions = new List<FSMTransition>();
    }

    public void AddAction(MotorAction maction){
        action = maction;
    }
    public void AddTransition(Condition condition, float param, string nextState){
        transitions.Add(new FSMTransition(condition, param, nextState));
    }

    public void ExectuteState(){
        Debug.Log(name);
    }

    public string GetName(){
        return name;
    }

    public List<FSMTransition> GetTransitions(){
        return transitions;
    }
    public MotorAction GetAction(){
        return action;
    }
}