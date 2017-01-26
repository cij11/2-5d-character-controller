using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

	AimingController aimingController;
	SpriteRenderer handSprite;

	float idleX = -0.2f;
	float idleY = -0.1f;
	float idleRotation = 10f;

	float aimingDisplacement = 0.2f;
	// Use this for initialization
	void Start () {
		aimingController = this.transform.parent.GetComponentInChildren<AimingController> () as AimingController;
		handSprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

		PositionHand ();
		RotateHand ();
		FlipHand ();
	}

	void PositionHand(){
		if (aimingController.GetIsAiming ()) {
			this.transform.localPosition = aimingController.GetAimingVector () * aimingDisplacement;
		} else {
			this.transform.localPosition = new Vector3(idleX * (float)aimingController.GetFacingDirection(), idleY, 0f);
		}
	}

	void RotateHand(){
		if(aimingController.GetIsAiming()){
			float handAngle = 0f;

			if (aimingController.GetHorizontalAiming () == 0) {
				if (aimingController.GetVerticalAiming () == 1) {
					if (aimingController.GetFacingDirection () == 1) {
						handAngle = 90f;
					} else {
						handAngle = -90f;
					}
				} else {
					if (aimingController.GetFacingDirection () == 1) {
						handAngle = -90f;
					} else {
						handAngle = 90f;
					}
				}
			} else if (aimingController.GetVerticalAiming () == 0) {
				handAngle = 0f;
			} else if ((aimingController.GetVerticalAiming () == 1)) {
				if (aimingController.GetHorizontalAiming () == 1) {
					handAngle = 45f;
				} else {
					handAngle = -45f;
				}
			} else { //All that's left is down left and down right
				if(aimingController.GetHorizontalAiming() == 1){
					handAngle = -45f;
				}
					else{
						handAngle = 45f;
					}
			}
			//90 straight up

			//45 up right

			//0 straight right

			//-45 //dow right

			//-90 straight down

			//180 straight left
			this.transform.localEulerAngles = new Vector3(0f, 0f, handAngle);
		}
		else{
			this.transform.localEulerAngles = new Vector3(0f, 0f, idleRotation * aimingController.GetFacingDirection());
		}
	}

	void FlipHand(){
		if (aimingController.GetFacingDirection () == 1) {
			handSprite.flipX = false;
		} else {
			handSprite.flipX = true;
		}
	}
}
