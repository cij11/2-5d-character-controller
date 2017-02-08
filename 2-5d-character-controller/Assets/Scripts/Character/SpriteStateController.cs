using UnityEngine;
using System.Collections;

public class SpriteStateController : MonoBehaviour
{
    CharacterMovementActuator characterMovement;
    CharacterContactSensor characterContacts;
    AimingController aimingController;
	CharacterCorpus corpus;

    Animator animator;
    SpriteRenderer spriteRenderer;

    float changeDirectionCutoff = 1f;
    bool xFlip = false;

	Color defaultColor;

    //Set unity StateChange attribute to true if stqte changes, otherwise false.
    //This will allow gating/triggering of state change from the 'any state' bucket
    //in the animator.
    int currentAnimationState = 0;
    int previousAnimationState = 0;
    bool hasAnimationStateChanged = false;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterMovement = this.transform.parent.GetComponent<CharacterMovementActuator>() as CharacterMovementActuator;
        characterContacts = this.transform.parent.GetComponent<CharacterContactSensor>() as CharacterContactSensor;
        aimingController = this.transform.parent.FindChild("ActionControllers").GetComponent<AimingController>() as AimingController;
		corpus = GetComponentInParent<CharacterCorpus> () as CharacterCorpus;

		defaultColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimatorSpeeds();
        SetSpriteFlip();
        SetAnimatorState();
		SetSpriteDamageColor ();
    }

    void SetAnimatorSpeeds()
    {
        //Only care about absolute speed for distinguishing between idle and running
        animator.SetFloat("AbsoluteHorizontalSpeed", Mathf.Abs(characterMovement.GetHorizontalSpeed()));
        //Care about sign for vertical speed to distinguish rising and falling.
        animator.SetFloat("VerticalSpeed", characterMovement.GetVerticalSpeed());
    }

    void SetSpriteFlip()
    {
        xFlip = aimingController.GetFacingDirection() < 0 ? true : false;
        spriteRenderer.flipX = xFlip;
    }

    void SetAnimatorState()
    {
        previousAnimationState = currentAnimationState;

        //States
        //0 ground/steep  - Idle or moving animation then depends on horizontal speed.
        //1 airborn
        //2 wall grab
		//3 dead
        if (characterContacts.GetContactState() == ContactState.FLATGROUND)
        {
            currentAnimationState = 0;
        }
        if (characterContacts.GetContactState() == ContactState.STEEPSLOPE)
        {
            currentAnimationState = 0;
        }
        if (characterContacts.GetContactState() == ContactState.AIRBORNE)
        {
            currentAnimationState = 1;
        }
        if (characterContacts.GetContactState() == ContactState.WALLGRAB)
        {
            currentAnimationState = 2;
        }
		if (corpus.GetIsReeling ()) {
			currentAnimationState = 4;
		}
		if (!corpus.GetIsAlive ()) {
			currentAnimationState = 3;
		}

        //Use this if rigging so all transitions come from any state
        //to prevent repeated triggering.
        if (previousAnimationState != currentAnimationState)
        {
            animator.SetInteger("State", currentAnimationState);
            hasAnimationStateChanged = true;
        }
        else
        {
            hasAnimationStateChanged = false;
        }
    }

	private void SetSpriteDamageColor(){
		if (currentAnimationState == 4) {
			spriteRenderer.color = new Color (1f, 1f, 1f, 1f);
		} else {
			spriteRenderer.color = defaultColor;
		}
	}
}
