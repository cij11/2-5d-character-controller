using UnityEngine;
using System.Collections;

public class AimingController : MonoBehaviour
{

    float horizontalStored;
    float verticalStored;
    float horizontalInput;
    float verticalInput;

    Vector3 aimingVector;
    bool isAiming;

    public RigidBodyController rigidBodyController;
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

        rigidBodyController = this.transform.parent.GetComponent<RigidBodyController>() as RigidBodyController;

    }

    // Update is called once per frame
    void Update()
    {
        CheckCharacterWallGrabbing();
        MatchAimingToControlsIfFireAndDirectionHeld();
        aimingVector = new Vector3(horizontalStored, verticalStored, 0f);
    }

    void CheckCharacterWallGrabbing()
    {
        if (rigidBodyController != null)
        {
            if (rigidBodyController.GetContactState() == ContactState.WALLGRAB)
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
            wallSide = rigidBodyController.GetSideGrabbed();
        }
        else
        {
            wallSide = MovementDirection.NEUTRAL;
        }
    }

    public void StartTargetting()
    {
        isAiming = true;
        DefaultAiming();
    }

    void DefaultAiming()
    {
        if (Mathf.Abs(horizontalInput) < 0.001f && Mathf.Abs(verticalInput) < 0.001f)
        {
            if (isWallGrabbing)
            {
                SetHorizontalAwayFromWall();
            }
            else
            {
                SetHorizontalToSpriteDirection();
            }
			verticalStored = 0f;
        }
    }

    public void StopTargetting()
    {
        isAiming = false;
    }
    void SetHorizontalToSpriteDirection()
    {
        if (characterSprite.getXFlip())
        {
            horizontalStored = -1.0f;
        }
        else
        {
            horizontalStored = 1.0f;
        }
    }

	void SetHorizontalAwayFromWall(){
		if(wallSide == MovementDirection.LEFT){
			horizontalStored = 1f;
		}
		else{
			horizontalStored = -1f;
		}
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
