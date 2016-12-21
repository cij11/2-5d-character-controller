using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour {

	AimingController aimingController;
	SpriteRenderer targetSprite;
	Vector3 displacementFromBody;
	Vector3 defaultDisplacement = new Vector3(0f, 0f, 0f);
	bool isVisible = false;
	// Use this for initialization
	void Start () {
		//Try this pattern: Check for component.
		//If not present, set null.
		//Implement default behaviour for null commponent, and log.

		aimingController = this.transform.parent.Find("Aiming").GetComponent<AimingController>();
		targetSprite = GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
		QueryAimingController();
		PositionTargettingReticule();
		ShowOrHideTargettingReticule();
	}

	void QueryAimingController(){
		if (aimingController != null){
			displacementFromBody = aimingController.GetAimingVector();
			isVisible = aimingController.GetIsHoldingFire();
		}
		else{
			displacementFromBody = defaultDisplacement;
		}

		isVisible = aimingController.GetIsHoldingFire();
	}

	void PositionTargettingReticule(){
		this.transform.localPosition = displacementFromBody;
	}

	void ShowOrHideTargettingReticule(){
				//Only render reticule if fire held
		if(isVisible){
			targetSprite.enabled = true;
		}
		else{
			targetSprite.enabled = false;
		}
	}
}
