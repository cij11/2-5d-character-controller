using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Stores information about AI objectives
public class AIGoals : MonoBehaviour {
	GameObject targetObject;
	private string defaultTarget;
	int forwardDirection;
	private MovementDirection roseDirection;                

	// Use this for initialization
	void Start () {
		defaultTarget = "Player";
		targetObject = GameObject.FindGameObjectWithTag(defaultTarget);
		forwardDirection = 1;
	}

	public GameObject GetTargetObject(){
		return targetObject;
	}

	public Vector3 GetTargetPosition(){
		if (targetObject == null || targetObject.Equals (null)) {
			return new Vector3 (Mathf.Infinity, Mathf.Infinity, this.transform.position.z);
		} else {
			return targetObject.transform.position;
		}
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

	public void PickRandomRoseDirection(){
		int randomInt = Random.Range (0, 8);
		roseDirection = (MovementDirection)randomInt;
		if (roseDirection == MovementDirection.NEUTRAL)
			PickRandomRoseDirection ();
			
	}

	public MovementDirection GetRoseDirection(){
		return roseDirection;
	}
}
