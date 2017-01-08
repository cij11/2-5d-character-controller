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

	// Use this for initialization
	void Start () {
		virtualController = GetComponent<AIVirtualController>() as AIVirtualController;
		parentTransform = this.transform.parent;
		updateTimer = new UpdateTimer(60);
		objectGoal = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if(updateTimer.TryUpdateThisTick()){
			action++;
			if (action > 1) action = 0;
		}
		if (action == 0){
			AimAtObject(objectGoal);
		}
		else{
			ReleaseFireAtObject(objectGoal);
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
		FireAtPoint(target.transform.position);
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
