using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {
	UpdateTimer updateTimer;
	int action = 0;
	AIVirtualController virtualController;
	Vector3 movementGoal;
	Vector3 aimingGoal;
	GameObject objectGoal;
	Transform parentTransform;

	FSM brainFSM;

	// Use this for initialization
	void Start () {
		virtualController = GetComponent<AIVirtualController>() as AIVirtualController;
		parentTransform = this.transform.parent;
		updateTimer = new UpdateTimer(60);
		objectGoal = GameObject.FindGameObjectWithTag("Player");
		brainFSM = new FSM();
	}
	
	// Update is called once per frame
	void Update () {
		brainFSM.Update();
		MotorAction action = brainFSM.GetAction();
		print(action.ToString());
		PerformAction(action);
	}

	void PerformAction(MotorAction action){
		switch (action){
			case MotorAction.IDLE:
			{
				ResetController();
				break;
			}
			case MotorAction.MOVELEFT:{
				MoveTowardsDirection(-1f);
				break;
			}
			case MotorAction.MOVERIGHT:{
				MoveTowardsDirection(1f);
				break;
			}
			case MotorAction.AIMTARGET:{
				AimAtObject(objectGoal);
				break;
			}
			case MotorAction.RELEASEFIRETARGET:{
				ReleaseFireAtObject(objectGoal);
				break;
			}
			default:
				ResetController();
				break;
		}
	}

	void MoveTowardsPoint(Vector3 goalPoint){
		Vector3 vectorToGoal = (goalPoint - this.transform.parent.position);

		float horizontalDirectionToGoal = Mathf.Sign(Vector3.Dot(vectorToGoal, this.transform.parent.right));
		virtualController.PushHorAxis(horizontalDirectionToGoal);
	}

	void MoveTowardsObject(GameObject target){
		MoveTowardsPoint(target.transform.position);
	}

	void MoveTowardsDirection(float dir){
		if (dir < 0){
			virtualController.PushHorAxis(-1f);
		}
		else
			virtualController.PushHorAxis(1f);
	}

	void AimAtObject(GameObject target){
		AimAtPoint(target.transform.position);
	}

	void AimAtPoint(Vector3 point){
		virtualController.PushFire();
		PushBothAxisToOctant(point);
	}

	void PushBothAxisToOctant(Vector3 point){
		Vector3 vectorToPoint = point - parentTransform.position;
		float horDist = Vector3.Dot(vectorToPoint, parentTransform.right);
		float vertDist = Vector3.Dot(vectorToPoint, parentTransform.up);

		float radAngle = Mathf.Atan(Mathf.Abs(vertDist)/Mathf.Abs(horDist));
		float angle = Mathf.Rad2Deg * radAngle;
		print(angle);
		if (angle < 22.5f){ //Horizontal octant
			virtualController.PushHorAxis(Mathf.Sign(horDist));
			virtualController.PushVertAxis(0f);
		}
		else if (angle >= 22.5 && angle < 67.5){ //Diagonal octants
			virtualController.PushHorAxis(Mathf.Sign(horDist));
			virtualController.PushVertAxis(Mathf.Sign(vertDist));
		}
		else{ //Vertical octants
			virtualController.PushHorAxis(0f);
			virtualController.PushVertAxis(Mathf.Sign(vertDist));
		}
	}

	void ReleaseFireAtObject(GameObject target){
		ReleaseFireAtPoint(target.transform.position);
	}
	
	void ReleaseFireAtPoint(Vector3 point){
		PushBothAxisToOctant(point);
		virtualController.ReleaseFire();
	}

	void ResetController(){
		virtualController.PushHorAxis(0f);
		virtualController.PushVertAxis(0f);
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
}
