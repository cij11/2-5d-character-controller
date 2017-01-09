using UnityEngine;
using System.Collections;

public class AIGoals : MonoBehaviour {
	GameObject targetObject;
	FSM fsm;
	AIMotorActions motorActions;
	// Use this for initialization
	void Start () {
		targetObject = GameObject.FindGameObjectWithTag("Player");
		fsm = GetComponent<FSM>() as FSM;
		motorActions = GetComponent<AIMotorActions>() as AIMotorActions;
		SetTargetObjectForAI(targetObject);
	}

	void SetTargetObjectForAI(GameObject targetObject){
		fsm.SetTarget(targetObject);
		motorActions.SetTarget(targetObject);
	}
	
}
