using UnityEngine;
using System.Collections;

public class AIGoals : MonoBehaviour {
	GameObject targetObject;
	AIMotorActions motorActions;
	private string defaultTarget;
	// Use this for initialization
	void Start () {
		defaultTarget = "Player";
		targetObject = GameObject.FindGameObjectWithTag(defaultTarget);
		motorActions = GetComponent<AIMotorActions>() as AIMotorActions;
		SetTargetObjectForAI(targetObject);
	}

	void SetTargetObjectForAI(GameObject targetObject){
		motorActions.SetTarget(targetObject);
	}
}
