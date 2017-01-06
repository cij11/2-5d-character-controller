using UnityEngine;
using System.Collections;

public class AimingController : MonoBehaviour
{

    float horizontalStored;
    float verticalStored;
    float horizontalInput;
    float verticalInput;
    float facingDirection = 1f;

    Vector3 aimingVector;
    bool isAiming;

    public CharacterContactSensor characterContact;
    bool isWallGrabbing;
    MovementDirection wallSide;

    //Default aiming will align with the direction the sprite is facing.
    public SpriteStateController characterSprite;

    // Use this for initialization
    void Start()
    {
        horizontalStored = 0f;
        verticalStored = 0f;
        horizontalInput = 0f;
        verticalInput = 0f;
        isAiming = false;
        isWallGrabbing = false;
        wallSide = MovementDirection.NEUTRAL;

        characterContact = this.transform.parent.GetComponent<CharacterContactSensor>() as CharacterContactSensor;

    }

    // Update is called once per frame
    void Update()
    {
        CheckCharacterWallGrabbing();
        DetermineFacingDirection();
        MatchAimingToControlsIfFireAndDirectionHeld();
        aimingVector = new Vector3(horizontalStored, verticalStored, 0f);
        aimingVector.Normalize();
    }

    void CheckCharacterWallGrabbing()
    {
        if (characterContact != null)
        {
            if (characterContact.GetContactState() == ContactState.WALLGRAB)
            {
                isWallGrabbing = true;
            }
            else
            {
                isWallGrabbing = false;
            }
        }
        if (isWallGrabbing)
        {
            wallSide = characterContact.GetSideGrabbed();
        }
        else
        {
            wallSide = MovementDirection.NEUTRAL;
        }
    }

    void DetermineFacingDirection()
    {
        //Wall grabbing takes precedence. Must face away from wall if wall grabbing
        if (isWallGrabbing)
        {
            //Face right if grabbing left wall, otherwise face left.
            facingDirection = (wallSide == MovementDirection.LEFT) ? 1f : -1f;
        }
        else
        {
            //Else update to change direction if a direciton is pressed
            if (horizontalInput < 0)
            {
                facingDirection = -1f;
            }
            if (horizontalInput > 0)
            {
                facingDirection = 1f;
            }
        }
    }

    public void StartTargetting()
    {
        isAiming = true;
        DefaultAiming();
    }

    void DefaultAiming()
    {
        //If no input is pressed, set the aiming direction to the current facing direction.
        if (Mathf.Abs(horizontalInput) < 0.001f && Mathf.Abs(verticalInput) < 0.001f)
        {
            horizontalStored = facingDirection;
			verticalStored = 0f;
        }
    }

    public void StopTargetting()
    {
        isAiming = false;
    }

    void MatchAimingToControlsIfFireAndDirectionHeld()
    {
        if (isAiming && ((Mathf.Abs(horizontalInput) > 0.001f) || (Mathf.Abs(verticalInput) > 0.001f)))
        {
            horizontalStored = horizontalInput;
            verticalStored = verticalInput;
        }
    }

    public Vector3 GetAimingVector()
    {
        return aimingVector;
    }

    public Vector3 GetAimingVectorWorldSpace(){
        return characterContact.gameObject.transform.rotation * aimingVector;
    }

    public bool GetIsAiming()
    {
        return isAiming;
    }

    public void SetHorizontalInput(float hor)
    {
        horizontalInput = hor;
    }
    public void SetVerticalInput(float vert)
    {
        verticalInput = vert;
    }

    public float GetFacingDirection(){
        return facingDirection;
    }
}
