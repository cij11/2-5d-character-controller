using UnityEngine;
using System.Collections;

public class AIMotorActions : MonoBehaviour {
	AIVirtualController virtualController;
	Transform parentTransform;
	GameObject targetObject;

	// Use this for initialization
	void Start () {
		virtualController = GetComponent<AIVirtualController>() as AIVirtualController;
		parentTransform = this.transform.parent;
	}

	public void PerformAction(Action action){
		switch (action){
			case Action.IDLE:
			{
				ResetController();
				break;
			}
			case Action.MOVE_LEFT:{
				MoveTowardsDirection(-1);
				break;
			}
			case Action.MOVE_RIGHT:{
				MoveTowardsDirection(1);
				break;
			}
			case Action.AIM_TARGET:{
				AimAtObject(targetObject);
				break;
			}
			case Action.RELEASE_FIRE_TARGET:{
				ReleaseFireAtObject(targetObject);
				break;
			}
			default:
				ResetController();
				break;
		}
	}

	void MoveTowardsPoint(Vector3 goalPoint){
		Vector3 vectorToGoal = (goalPoint - this.transform.parent.position);

		int horizontalDirectionToGoal = (int)Mathf.Sign(Vector3.Dot(vectorToGoal, this.transform.parent.right));
		virtualController.PushHorAxis(horizontalDirectionToGoal);
	}

	void MoveTowardsObject(GameObject target){
		MoveTowardsPoint(target.transform.position);
	}

	void MoveTowardsDirection(float dir){
		if (dir < 0){
			virtualController.PushHorAxis(-1);
		}
		else
			virtualController.PushHorAxis(1);
	}

	void AimAtObject(GameObject target){
		AimAtPoint(target.transform.position);
	}

	void AimAtPoint(Vector3 point){
		virtualController.PushFire();
		PushBothAxisToOctant(point);
	}

    void PushBothAxisToOctant(Vector3 point)
    {
        int horOctant;
        int vertOctant;

        Octant.PointsToOctant(parentTransform.position, point,
            parentTransform.right, parentTransform.up, out horOctant, out vertOctant);

        virtualController.PushHorAxis(horOctant);
        virtualController.PushVertAxis(vertOctant);
    }

    void ReleaseFireAtObject(GameObject target){
		ReleaseFireAtPoint(target.transform.position);
	}
	
	void ReleaseFireAtPoint(Vector3 point){
		PushBothAxisToOctant(point);
		virtualController.ReleaseFire();
	}

	void ResetController(){
		virtualController.PushHorAxis(0);
		virtualController.PushVertAxis(0);
		virtualController.ReleaseFire();
		virtualController.ReleaseJump();
		virtualController.ReleaseSwap();
	}

	float HorDirectionToPoint(Vector3 point){
		Vector3 vectorToPoint = point - this.transform.parent.position;
		return Mathf.Sign(Vector3.Dot(vectorToPoint, this.transform.parent.right));
	}
	float VertDirectionToPoint(Vector3 point){
		Vector3 vectorToPoint = point - this.transform.parent.position;
		return Mathf.Sign(Vector3.Dot(vectorToPoint, this.transform.parent.up));
	}

	
    public void SetTarget(GameObject target){
        targetObject = target;
    }
}
