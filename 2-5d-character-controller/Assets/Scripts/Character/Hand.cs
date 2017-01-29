using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

	AimingController aimingController;
	CharacterContactSensor contactSensor;
	SpriteRenderer handSprite;

	float idleX = 0.05f;
	float idleY = 0.05f;
	float idleRotation = 0f;

	float shoulderAttachmentXWallGrab = -0.1f;

	float aimingDisplacement = 0.2f;
	// Use this for initialization
	void Start () {
		aimingController = this.transform.parent.GetComponentInChildren<AimingController> () as AimingController;
		contactSensor = this.transform.GetComponentInParent<CharacterContactSensor> () as CharacterContactSensor;
		handSprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

	//	ResetIdle ();
		PositionHand ();
		RotateHand ();
		FlipHand ();
		OrientHeldItem ();
		DontRenderIfSwingingMelee ();
	}

	//Return hand to idle position and angle if character moves while not aiming.
	void ResetIdle(){
		if (!aimingController.GetIsAiming ()) {
			if (aimingController.GetIsAnyAimingInput ()) {
				this.transform.localPosition = new Vector3(idleX * (float)aimingController.GetFacingDirection(), idleY, 0f);
			}
		}
	}

	void PositionHand(){
	//	if (aimingController.GetIsAiming ()) {
		float shoulderOffset = idleX * aimingController.GetFacingDirection();
		if (contactSensor.GetContactState () == ContactState.WALLGRAB) {
			shoulderOffset = shoulderAttachmentXWallGrab;
		}
		this.transform.localPosition = aimingController.GetAimingVector () * aimingDisplacement + new Vector3(shoulderOffset, idleY, 0f);
			
		//}
	}

	void RotateHand(){
	//	if(aimingController.GetIsAiming()){
			float handAngle = HandAngleFromAimingController ();
			this.transform.localEulerAngles = new Vector3(0f, 0f, handAngle);
		//}
	}

	void FlipHand(){
		if (aimingController.GetFacingDirection () == 1) {
			handSprite.flipX = false;
		} else {
			handSprite.flipX = true;
		}
	}

	private float HandAngleFromAimingController(){
		float handAngle = 0f;

		//If aiming straight up or down, angle depends on facing direction.
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

		//If aiming flat horizontally, angle is always level at 0 degrees
		} else if (aimingController.GetVerticalAiming () == 0) {
			handAngle = 0f;

		//If aiming diagonally, top left and bot right have the same angle. Top right and bot left have the same angle.
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
		return handAngle;
	}

	void OrientHeldItem(){
		Transform childTransform = this.transform.GetChild (0); 
		if (childTransform != null) {
			Weapon childWeapon = childTransform.GetComponent<Weapon> () as Weapon;
			SpriteRenderer childSprite = childWeapon.GetSpriteRenderer ();

			if (childSprite != null) {
				if (aimingController.GetFacingDirection () == 1) {
					childSprite.flipX = false;
				} else {
					childSprite.flipX = true;
				}
			}

			if (aimingController.GetFacingDirection () == 1) {
				childTransform.localPosition = childWeapon.gripOffset;
			} else {
				childTransform.localPosition = new Vector3 (-childWeapon.gripOffset.x, childWeapon.gripOffset.y, 0f);
			}
		}
	}

	void DontRenderIfSwingingMelee(){
		Transform childTransform = this.transform.GetChild (0); 
		if (childTransform != null) {
			Weapon childWeapon = childTransform.GetComponent<Weapon> () as Weapon;
			if (childWeapon.GetIsSwinging ()) {
				childWeapon.DisableSprite ();
				DisableSprite ();
			} else {
				childWeapon.EnableSprite ();
				EnableSprite ();
			}
		}
	}

	void DisableSprite(){
		handSprite.enabled = false;
	}

	void EnableSprite(){
		handSprite.enabled = true;
	}

}
