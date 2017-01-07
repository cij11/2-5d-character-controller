using UnityEngine;
using System.Collections;

public class AIAbstractInput : AbstractInput {
	AIVirtualController virtualController;

	void Start(){
		virtualController = GetComponent<AIVirtualController>() as AIVirtualController;
		TakeControlOfCharacter();
	}
	public override void UpdateInput(){
		HorizontalInput();
		VerticalInput();
		Buttons();
	}

	void HorizontalInput(){
		horAxis = virtualController.GetHorAxis();
	}

	void VerticalInput(){
		vertAxis = virtualController.GetVertAxis();
	}

	void Buttons(){
		bool currentJump = virtualController.GetJump();
		bool currentFire = virtualController.GetFire();
		bool currentSwap = virtualController.GetSwap();

		DifferentiateButton(currentJump, jump, out jumpDown, out jump, out jumpUp);
		DifferentiateButton(currentFire, fire, out fireDown, out fire, out fireUp);
		DifferentiateButton(currentSwap, swap, out swapDown, out swap, out swapUp);
	}

	void DifferentiateButton(bool currentButton, bool oldButton, out bool buttonDown, out bool button, out bool buttonUp){
		if (currentButton == true && oldButton == false) buttonDown = true;
		else buttonDown = false;
		if (currentButton == false && oldButton == true) buttonUp = true;
		else buttonUp = false;
		button = currentButton;
	}
}
