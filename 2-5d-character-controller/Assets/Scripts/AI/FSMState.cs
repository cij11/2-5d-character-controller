using UnityEngine;
using System.Collections.Generic;

//Data object representing a state of the AI finite state machine.
public class FSMState{
    
    string name;
	string startingSubstate; //If this is a superstate, load startingSubstate as the first state in the child FSM.
    Action action;
    List<FSMTransition> transitions;

    public FSMState(string newName, Action maction){
        name = newName;
		startingSubstate = "";
        action = maction;
        transitions = new List<FSMTransition>();
    }

    public void AddTransition(FSMTransition transition){
        transitions.Add(transition);
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
    public Action GetAction(){
        return action;
    }

	public void SetStartingSubstate(string start){
		startingSubstate = start;
	}

	public string GetStartingSubstate(){
		return startingSubstate;
	}
}