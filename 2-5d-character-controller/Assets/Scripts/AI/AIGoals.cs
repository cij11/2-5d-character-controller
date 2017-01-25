using UnityEngine;
using System.Collections;

//Stores information about AI objectives
public class AIGoals : MonoBehaviour {
	GameObject targetObject;
	private string defaultTarget;
	int forwardDirection;

	// Use this for initialization
	void Start () {
		defaultTarget = "Player";
		targetObject = GameObject.FindGameObjectWithTag(defaultTarget);
		forwardDirection = 1;
	}

	public GameObject GetTargetObject(){
		return targetObject;
	}

	public int GetForwardDirection(){
		return forwardDirection;
	}
	public void SetForwardDirection(int newDirection){
		forwardDirection = newDirection;
	}
	public void ToggleForwardDirection(){
		forwardDirection = -forwardDirection;
	}
}
