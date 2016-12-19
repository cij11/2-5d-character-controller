using UnityEngine;
using System.Collections;

public class ReticuleController : MonoBehaviour {

	float horizontal;
	float vertical;

	//Default aiming will align with the direction the sprite is facing.
	public SpriteStateController characterSprite;

	Transform target;
	// Use this for initialization
	void Start () {
	horizontal = 0f;
	vertical = 0f;

	target = this.transform.GetChild(0);
	
	}
	
	// Update is called once per frame
	void Update () {
		if( Mathf.Abs(Input.GetAxis("Horizontal")) > 0.001f ){
			horizontal = Mathf.Sign(Input.GetAxis("Horizontal"));
		}
		else{
			horizontal = 0f;
		}

		if( Mathf.Abs(Input.GetAxis("Vertical")) > 0.001f ){
			vertical = Mathf.Sign(Input.GetAxis("Vertical"));
		}
		else{
			vertical = 0f;
		}

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

		SetTargetPosition();
		SetTargetOpacity();
	}

	void SetTargetPosition(){
		target.transform.localPosition = new Vector3(horizontal, vertical, 0f);
	}
	void SetTargetOpacity(){
		//Only render reticule if fire held
		if(Input.GetAxis("Fire1") > 0){
			target.gameObject.active= true;
		}
		else{
			target.gameObject.active = false;
		}
	}
}
