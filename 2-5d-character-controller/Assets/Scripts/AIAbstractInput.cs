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
		if(virtualController.ExtractJumpTap()){
			TapButton(jump, out jumpDown, out jump, out jumpUp);
		}
		else{
			DifferentiateButton(virtualController.GetJump(), jump, out jumpDown, out jump, out jumpUp);
		}

		if(virtualController.ExtractFireTap()){
			TapButton(fire, out fireDown, out fire, out fireUp);
		}
		else{
			DifferentiateButton(virtualController.GetFire(), fire, out fireDown, out fire, out fireUp);
		}

		if(virtualController.ExtractSwapTap()){
			TapButton(swap, out swapDown, out swap, out swapUp);
		}
		else{
			DifferentiateButton(virtualController.GetSwap(), swap, out swapDown, out swap, out swapUp);
		}
	}

	void TapButton(bool oldButton, out bool buttonDown, out bool button, out bool buttonUp){
		buttonDown = !oldButton ? true : false;
		button = false;
		buttonUp = true;
	}
	void DifferentiateButton(bool currentButton, bool oldButton, out bool buttonDown, out bool button, out bool buttonUp){
		if (currentButton == true && oldButton == false) buttonDown = true;
		else buttonDown = false;
		if (currentButton == false && oldButton == true) buttonUp = true;
		else buttonUp = false;
		button = currentButton;
	}
}
