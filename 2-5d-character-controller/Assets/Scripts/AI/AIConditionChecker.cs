using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIConditionChecker : MonoBehaviour {

	private AIRaycastSensors raycastSensors;
	private AIMotorActions motorActions;
	private AIGoals goals;
	private Transform parentTransform;

	//Horizontal and vertical directions pointing to the closest octant to the target
	int horOctant;
	int vertOctant;

	// Use this for initialization
	void Start () {
		motorActions = GetComponent<AIMotorActions>() as AIMotorActions;
		raycastSensors = GetComponent<AIRaycastSensors>() as AIRaycastSensors;
		goals = GetComponent<AIGoals> () as AIGoals;
		parentTransform = this.transform.parent;
	}
	
	public bool TestCondition(Condition condition, float param, float timer)
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
}
