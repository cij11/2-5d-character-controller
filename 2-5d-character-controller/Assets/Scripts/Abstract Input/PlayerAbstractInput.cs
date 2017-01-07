using UnityEngine;
using System.Collections;

public class PlayerAbstractInput : AbstractInput {
	public override void UpdateInput(){
		HorizontalInput();
		VerticalInput();
		Buttons();
	}

	void HorizontalInput(){
		if (Input.GetAxis("Horizontal") < 0){
			horAxis = -1f;
		}
		else if (Input.GetAxis("Horizontal") > 0){
			horAxis = 1f;
		}
		else{
			horAxis = 0f;
		}
	}

	void VerticalInput(){
		if (Input.GetAxis("Vertical") < 0){
			vertAxis = -1f;
		}
		else if (Input.GetAxis("Vertical") > 0){
			vertAxis = 1f;
		}
		else{
			vertAxis = 0f;
		}
	}

	void Buttons(){
		jumpDown = Input.GetButtonDown("Jump");
		jump = Input.GetButton("Jump");
		jumpUp = Input.GetButtonUp("Jump");

		fireDown = Input.GetButtonDown("Fire1");
		fire = Input.GetButton("Fire1");
		fireUp = Input.GetButtonUp("Fire1");

		swapDown = Input.GetButtonDown("Fire2");
	}
}
