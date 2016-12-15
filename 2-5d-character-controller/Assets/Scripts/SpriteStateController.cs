using UnityEngine;
using System.Collections;

public class SpriteStateController : MonoBehaviour {
	public RigidBodyController bodyController;
	Animator animator;
	SpriteRenderer spriteRenderer;

	float changeDirectionCutoff = 0.5f;

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

		//States
		//0 ground/steep
		//1 airborn
		//2 wall grab
		//3 ledge grab
		if (bodyController.GetContactState() == ContactState.FLATGROUND){
			animator.SetInteger("State", 0);
		}
		if (bodyController.GetContactState() == ContactState.STEEPSLOPE){
			animator.SetInteger("State", 0);
		}
		if (bodyController.GetContactState() == ContactState.AIRBORNE){
			animator.SetInteger("State", 1);
		}
		if (bodyController.GetContactState() == ContactState.WALLGRAB){
			animator.SetInteger("State", 2);
		}
		if (bodyController.GetContactState() == ContactState.LEDGEGRAB){
			animator.SetInteger("State", 3);
		}
	}
}
