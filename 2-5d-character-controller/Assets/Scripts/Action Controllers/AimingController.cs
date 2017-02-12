using UnityEngine;
using System.Collections;

public class AimingController : MonoBehaviour
{

    CharacterContactSensor characterContact;
	CharacterMovementActuator movementActuator;

    int horizontalInput;
    int verticalInput;

    bool isWallAdjacent;
    MovementDirection wallSide;
    int facingDirection = 1;

    int horizontalAiming;
    int verticalAiming;
    Vector3 aimingVector;

    bool isAiming;

    float verticalGravityTimer = 0f;
    float verticalGravityPeriod = 0.15f;
    float horizontalGravityTimer = 0f;
    float horizontalGravityPeriod = 0.1f;

	FiringController firingController;

    void Start()
    {
        horizontalAiming = 0;
        verticalAiming = 0;
        horizontalInput = 0;
        verticalInput = 0;
        isAiming = false;
        isWallAdjacent = false;
        wallSide = MovementDirection.NEUTRAL;

		firingController = GetComponent<FiringController> () as FiringController;

        characterContact = this.transform.parent.GetComponent<CharacterContactSensor>() as CharacterContactSensor;
		movementActuator = GetComponentInParent<CharacterMovementActuator> () as CharacterMovementActuator;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGravityTimers();
        CheckCharacterWallAdjacent();

		if (!firingController.GetIsAimingLocked ()) {
			DetermineFacingDirection();
			UpdateAimingVector ();
		}
    }

    void UpdateGravityTimers()
    {
        if (verticalGravityTimer > 0) verticalGravityTimer -= Time.deltaTime;
        if(horizontalGravityTimer > 0) horizontalGravityTimer -= Time.deltaTime;
    }

    void CheckCharacterWallAdjacent()
    {
        if (characterContact != null)
        {
			if (movementActuator.GetIsHuggingWall())
            {
                isWallAdjacent = true;
            }
            else
            {
                isWallAdjacent = false;
            }
        }
        if (isWallAdjacent)
        {
            wallSide = characterContact.GetSideAdjacent();
        }
        else
        {
            wallSide = MovementDirection.NEUTRAL;
        }
    }

    void DetermineFacingDirection()
    {
        //Wall grabbing takes precedence. Must face away from wall if wall grabbing
        if (isWallAdjacent)
        {
            //Face right if grabbing left wall, otherwise face left.
            facingDirection = (wallSide == MovementDirection.LEFT) ? 1 : -1;
        }
        else
        {
            //Else update to change direction if a direciton is pressed
            if (horizontalInput < 0)
            {
                facingDirection = -1;
            }
            if (horizontalInput > 0)
            {
                facingDirection = 1;
            }
        }
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    void UpdateAimingVector()
    {
        //If no input is pressed, set the aiming direction to the current facing direction.
        if (horizontalInput == 0 && verticalInput == 0)
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
        aimingVector = new Vector3((float)horizontalAiming, (float)verticalAiming, 0f);
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

		//Character cannot aim at the wall they are grabbing
		if (isWallAdjacent) {
			if (verticalAiming != 0) {
				if (horizontalAiming == -facingDirection) {
					horizontalAiming = 0;
				}
			} else {
				if (horizontalAiming == -facingDirection) {
					horizontalAiming = facingDirection;
				}
			}
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

    public void SetHorizontalInput(int hor)
    {
        horizontalInput = hor;
        if(hor != 0) horizontalGravityTimer = horizontalGravityPeriod;
    }
    public void SetVerticalInput(int vert)
    {
        verticalInput = vert;
        if (vert != 0) verticalGravityTimer = verticalGravityPeriod; //Only reset gravity timer when axis is perturbed.
    }

	public int GetHorizontalInput(){
		return horizontalInput;
	}

    public int GetHorizontalAiming(){
        return horizontalAiming;
    }

    public int GetVerticalAiming(){
        return verticalAiming;
    }

	public bool GetIsAnyAimingInput(){
		if (!(horizontalInput == 0))
			return true;
		if (!(verticalInput == 0))
			return true;
		return false;
	}
}
