using UnityEngine;
using System.Collections;

public class AimingController : MonoBehaviour
{
    
    CharacterContactSensor characterContact;

    float horizontalInput;
    float verticalInput;

    bool isWallGrabbing;
    MovementDirection wallSide;
    float facingDirection = 1f;

    float horizontalAiming;
    float verticalAiming;
    Vector3 aimingVector;

    bool isAiming;

    void Start()
    {
        horizontalAiming = 0f;
        verticalAiming = 0f;
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
        UpdateAimingVector();
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
    
    public float GetFacingDirection()
    {
        return facingDirection;
    }

    void UpdateAimingVector()
    {
        //If no input is pressed, set the aiming direction to the current facing direction.
        if (Mathf.Abs(horizontalInput) < 0.001f && Mathf.Abs(verticalInput) < 0.001f)
        {
            AimInFacingDirection();
        }
        else
        {
            AimInInputDirection();
        }
        aimingVector = new Vector3(horizontalAiming, verticalAiming, 0f);
        aimingVector.Normalize();
    }

    void AimInFacingDirection()
    {
        horizontalAiming = facingDirection;
        verticalAiming = 0f;
    }
    
    void AimInInputDirection()
    {
        horizontalAiming = horizontalInput;
        verticalAiming = verticalInput;
    }
    
    public Vector3 GetAimingVector()
    {
        return aimingVector;
    }

    public void StartTargetting()
    {
        isAiming = true;
    }

    public void StopTargetting()
    {
        isAiming = false;
    }


    public Vector3 GetAimingVectorWorldSpace()
    {
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
}
