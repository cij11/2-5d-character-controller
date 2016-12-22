using UnityEngine;
using System.Collections;

public class AimingController : MonoBehaviour {

	float horizontal;
	float vertical;

	Vector3 aimingVector;
	bool isHoldingFire;

	//Default aiming will align with the direction the sprite is facing.
	public SpriteStateController characterSprite;

	// Use this for initialization
	void Start () {
	horizontal = 0f;
	vertical = 0f;
	
	}
	
	// Update is called once per frame
	void Update () {
		//if no directional button is pressed, default the target to aim in the direction the
		//sprite is facing.
		if (Mathf.Abs(vertical) < 0.001f && Mathf.Abs(horizontal) < 0.001f){
			if(characterSprite.getXFlip()){
				horizontal = -1.0f;
			}
			else{
				horizontal = 1.0f;
			}
		}

		aimingVector = new Vector3(horizontal, vertical, 0f);
		isHoldingFire = Input.GetButton("Fire1");
	}

	public void SetHorizontal(float h){
		horizontal = h;
	}

	public void SetVertical(float v){
		vertical = v;
	}

	public Vector3 GetAimingVector(){
		return aimingVector;
	}
	public bool GetIsHoldingFire(){
		return isHoldingFire;
	}
}
