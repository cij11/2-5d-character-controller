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

    float verticalGravityTimer = 0f;
    float verticalGravityPeriod = 0.1f;
    float horizontalGravityTimer = 0f;
    float horizontalGravityPeriod = 0.1f;
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
        UpdateGravityTimers();
        CheckCharacterWallGrabbing();
        DetermineFacingDirection();
        UpdateAimingVector();
    }

    void UpdateGravityTimers()
    {
        if (verticalGravityTimer > 0) verticalGravityTimer -= Time.deltaTime;
        if(horizontalGravityTimer > 0) horizontalGravityTimer -= Time.deltaTime;
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
            //Only pull to default after vertical gravity has expired.
            if(verticalGravityTimer <= 0){
              AimInFacingDirection();
            }
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
        //Only update vertical to 0 after <gravity> time has passed
        if (verticalGravityTimer < 0)
        {
            verticalAiming = 0;
        }
    }

    void AimInInputDirection()
    {
        if(horizontalInput == 0){
            if(horizontalGravityTimer <= 0){
                horizontalAiming = 0;
            }
        }
        else{
          horizontalAiming = horizontalInput;
        }

        //If vertical input is 0, only update it after <gravity> time has passed.
        if (verticalInput == 0)
        {
            if (verticalGravityTimer <= 0)
            {
                verticalAiming = 0;
            }
        }
        else
        {
            verticalAiming = verticalInput;
        }
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
        if(hor != 0) horizontalGravityTimer = horizontalGravityPeriod;
    }
    public void SetVerticalInput(int vert)
    {
        verticalInput = vert;
        if (vert != 0) verticalGravityTimer = verticalGravityPeriod; //Only reset gravity timer when axis is perturbed.
    }
}
