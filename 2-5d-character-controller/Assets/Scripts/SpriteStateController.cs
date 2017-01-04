using UnityEngine;
using System.Collections;

public class SpriteStateController : MonoBehaviour
{
    CharacterMovementActuator characterMovement;
    CharacterContactSensor characterContacts;
    Animator animator;
    SpriteRenderer spriteRenderer;

    float changeDirectionCutoff = 1f;
    bool xFlip = false;

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

    }

    // Update is called once per frame
    void Update()
    {
        //Only care about absolute speed for distinguishing between idle and running
        animator.SetFloat("AbsoluteHorizontalSpeed", Mathf.Abs(characterMovement.GetHorizontalSpeed()));

        //Care about sign for vertical speed to distinguish rising and falling.
        animator.SetFloat("VerticalSpeed", characterMovement.GetVerticalSpeed());

        //Care about sign for horizontal speed for flipping sprite
        if (characterMovement.GetHorizontalSpeed() < -changeDirectionCutoff)
        {
            xFlip = true;
        }
        //Care about sign for horizontal speed for flipping sprite
        if (characterMovement.GetHorizontalSpeed() > changeDirectionCutoff)
        {
            xFlip = false;
        }

        spriteRenderer.flipX = xFlip;


        previousAnimationState = currentAnimationState;

        //States
        //0 ground/steep
        //1 airborn
        //2 wall grab
        //3 ledge grab
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

        if (currentAnimationState == 2)
        {
            //If grabbing a side, flip sprite appropriately
            if (characterContacts.GetSideGrabbed() == MovementDirection.LEFT)
            {
                spriteRenderer.flipX = true;
                xFlip = true;
            }
            else
            {
                spriteRenderer.flipX = false;
                xFlip = false;
            }
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

    public bool getXFlip()
    {
        return xFlip;
    }
}
