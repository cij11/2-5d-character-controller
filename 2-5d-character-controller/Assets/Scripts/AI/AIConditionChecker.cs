using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIConditionChecker : MonoBehaviour {

	private AIRaycastSensors raycastSensors;
	private AIMotorActions motorActions;
	private CharacterContactSensor contactSensor;
	private AIGoals goals;
	private Transform parentTransform;

	//Horizontal and vertical directions pointing to the closest octant to the target
	int horOctant;
	int vertOctant;
	float horDistance;
	float vertDistance;

	//How close the target needs to be to the line of the horizontal to be seen
	float verticalSpottingHeight = 1f;

	// Use this for initialization
	void Start () {
		motorActions = GetComponent<AIMotorActions>() as AIMotorActions;
		raycastSensors = GetComponent<AIRaycastSensors>() as AIRaycastSensors;
		contactSensor = GetComponentInParent<CharacterContactSensor> () as CharacterContactSensor;
		goals = GetComponent<AIGoals> () as AIGoals;
		parentTransform = this.transform.parent;
	}
	
	public bool TestCondition(Condition condition, float param, FSMCounter counter)
	{
		switch (condition)
		{
		case Condition.TIMER:
			{
				if (counter.timer > param) return true;
				break;
			}
		case Condition.FRAMES:
			{
				if(counter.frames >= (Mathf.CeilToInt(param))) return true;
				break;
			}
		case Condition.TARGET_IN_RADIUS:
			{
				if(Vector3.Magnitude(goals.GetTargetPosition() - parentTransform.position) < param) return true;
				break;
			}
		case Condition.TARGET_OUTSIDE_RADIUS:
			{
				if(Vector3.Magnitude(goals.GetTargetPosition() - parentTransform.position) > param) return true;
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
		case Condition.TARGET_IN_FRONT_OCTANT:
			{
				FindTargetOctant();
				if(horOctant == goals.GetForwardDirection() && vertOctant == 0) return true;
				break;
			}
		case Condition.TARGET_IN_FRONT_HORIZONTAL:
			{
				FindTargetDistances ();
				if (Mathf.Sign (horDistance) == Mathf.Sign (goals.GetForwardDirection ())) {
					if (Mathf.Abs (vertDistance) < verticalSpottingHeight)
						return true;
				}
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
		case Condition.CLIFF_FORWARD:
			{
				if (contactSensor.GetContactState () != ContactState.AIRBORNE) { //Don't look for cliffs if already airborne...
					if (goals.GetForwardDirection () == 1) {
						if (raycastSensors.GetRightCliff ())
							return true;
					} else {
						if (raycastSensors.GetLeftCliff ())
							return true;
					}
				}
				break;
			}
		case Condition.ALWAYS:
			{
				return true;
				break;
			}
		}
		return false;
	}

	void FindTargetOctant(){
		Octant.PointsToOctant(parentTransform.position, goals.GetTargetPosition(),
			parentTransform.right, parentTransform.up, out horOctant, out vertOctant);
	}

	void FindTargetDistances(){
		Octant.PointsToDistances(parentTransform.position, goals.GetTargetPosition(),
			parentTransform.right, parentTransform.up, out horDistance, out vertDistance);
	}
}
