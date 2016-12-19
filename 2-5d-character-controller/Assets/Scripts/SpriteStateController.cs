using UnityEngine;
using System.Collections;

public class SpriteStateController : MonoBehaviour {
	public RigidBodyController bodyController;
	Animator animator;
	SpriteRenderer spriteRenderer;

	float changeDirectionCutoff = 0.5f;

	//Set unity StateChange attribute to true if stqte changes, otherwise false.
	//This will allow gating/triggering of state change from the 'any state' bucket
	//in the animator.
	int currentAnimationState = 0;
	int previousAnimationState = 0;
	bool hasAnimationStateChanged = false;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		//Only care about absolute speed for distinguishing between idle and running
		animator.SetFloat("AbsoluteHorizontalSpeed", Mathf.Abs(bodyController.GetHorizontalSpeed()));
		
		//Care about sign for vertical speed to distinguish rising and falling.
		animator.SetFloat("VerticalSpeed", bodyController.GetVerticalSpeed());

		//Care about sign for horizontal speed for flipping sprite
		if (bodyController.GetHorizontalSpeed() < -changeDirectionCutoff){
			spriteRenderer.flipX = true;
		}
		//Care about sign for horizontal speed for flipping sprite
		if (bodyController.GetHorizontalSpeed() > changeDirectionCutoff){
			spriteRenderer.flipX = false;
		}
		


		previousAnimationState = currentAnimationState;

		//States
		//0 ground/steep
		//1 airborn
		//2 wall grab
		//3 ledge grab
		if (bodyController.GetContactState() == ContactState.FLATGROUND){
			currentAnimationState = 0;
		}
		if (bodyController.GetContactState() == ContactState.STEEPSLOPE){
			currentAnimationState = 0;
		}
		if (bodyController.GetContactState() == ContactState.AIRBORNE){
			currentAnimationState = 1;
		}
		if (bodyController.GetContactState() == ContactState.WALLGRAB){
			currentAnimationState = 2;
		}
		if (bodyController.GetContactState() == ContactState.LEDGEGRAB){
			currentAnimationState = 3;
			
		}

		if(currentAnimationState == 2 || currentAnimationState == 3){
					//If grabbing a side, flip sprite appropriately
		if(bodyController.GetSideGrabbed() == MovementDirection.LEFT){
			spriteRenderer.flipX = true;
		}
		else{
spriteRenderer.flipX = false;
		}
		}
	
		//Use this if rigging so all transitions come from any state
		//to prevent repeated triggering.
		if (previousAnimationState != currentAnimationState){
			animator.SetInteger("State", currentAnimationState);
			hasAnimationStateChanged = true;
		}
		else{
			hasAnimationStateChanged = false;
		}
	}
}
