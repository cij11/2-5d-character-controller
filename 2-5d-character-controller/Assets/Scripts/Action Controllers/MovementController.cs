using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour
{

    CharacterMovementActuator movementActuator;
    CharacterContactSensor contactSensor;
    AimingController aimingController;
    FiringController firingController;
    bool isTargetting;
    float hangtimePeriod = 1f;
    float hangtimeTimer = 0f;
    bool hugsWalls = true;

    float jumpAfterFallingGracePeriod = 0.1f;
    float fallingTimer = 0f;
    int maxDoubleJumps = 1;
    int remainingDoubleJumps = 0;

    float phaseSpeed = 40f;
    float phaseDuration = 0.1f;

    // Use this for initialization
    void Start()
    {
        movementActuator = this.GetComponentInParent<CharacterMovementActuator>() as CharacterMovementActuator;
        contactSensor = this.GetComponentInParent<CharacterContactSensor>() as CharacterContactSensor;
        aimingController = this.GetComponent<AimingController>() as AimingController;
        firingController = this.GetComponent<FiringController>() as FiringController;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateArialJumpingTrackers();
        HugWallIfAdjacentAndGripsWalls();
        AimingHang();
    }

    void UpdateArialJumpingTrackers()
    {
        if (contactSensor.GetIsInContactWithTerrain())
        {
            fallingTimer = jumpAfterFallingGracePeriod;
            remainingDoubleJumps = maxDoubleJumps;
        }
        if (fallingTimer > 0) fallingTimer -= Time.deltaTime;
    }

    void HugWallIfAdjacentAndGripsWalls()
    {
        if (hugsWalls && contactSensor.GetContactState() == ContactState.WALLGRAB)
        {
            movementActuator.ApplyWallHugForce();
        }
    }

    void AimingHang()
    {
        if (isTargetting)
        {
            hangtimeTimer -= Time.deltaTime;
            if (hangtimeTimer < 0)
            {
                isTargetting = false;
            }
            movementActuator.ParachuteCommand();
        }
    }

    //Paramaters - Direction. -1f = left. 1f = right. 0f = nil.
    public void MoveHorizontal(float direction)
    {
        if (!firingController.GetIsEncumbered())
        {
            movementActuator.MoveHorizontalCommand(direction);
        }
    }
    public void SetWalkingSpeed(float newSpeed)
    {
        movementActuator.SetWalkingSpeed(newSpeed);
    }

    public void Grab()
    {

    }

    //Determine what type of jump is appropriate when jump pressed, and apply
    public void Jump(float hor, float vert)
    {
        if (firingController.GetIsEncumbered())
        {
            EncumberedJump(hor);
        }
        else if (vert < 0)
        {
            RollJump(hor);
        }
        else
        {
            StandardJump(hor, vert);
        }
    }

    private void EncumberedJump(float hor)
    {
        movementActuator.RollCommand(aimingController.GetFacingDirection(), phaseSpeed, phaseDuration);
    }

    private void RollJump(float hor)
    {
        movementActuator.RollCommand(aimingController.GetFacingDirection(), phaseSpeed, phaseDuration);
    }

    private void StandardJump(float hor, float vert)
    {
        //Detect ground and add upwards component to velocity if standing.
        //If terrain is too steep, walljump instead
        //Only allow jumping if standing on or adjacent to something
        if (contactSensor.GetContactState() == ContactState.FLATGROUND)
        {
            movementActuator.GroundJump();
        }
        else if (contactSensor.GetContactState() == ContactState.STEEPSLOPE)
        {
            JumpAwayFromSlope();
        }
        else if (contactSensor.GetContactState() == ContactState.WALLGRAB)
        {
            WallJumpUpDownOrAway(hor, vert);
        }
        else if (contactSensor.GetContactState() == ContactState.AIRBORNE)
        {
            AttemptAirborneJump();
        }
        //If jump has been pressed, count as leaving the ground to prevent
        //being able to immediately take a second jump in the air is if grounded.
        ZeroFallingGraceTimer();
    }

    private void JumpAwayFromSlope()
    {
        //if uphill is top right
        //walljump left
        if (contactSensor.GetUphillDirection() > 0)
        {
            movementActuator.WallJump(-1);
        }
        else //walljump right
        {
            movementActuator.WallJump(1);
        }
    }

    private void WallJumpUpDownOrAway(float hor, float vert)
    {
        float propelDirection = (contactSensor.GetSideGrabbed() == MovementDirection.LEFT) ? 1f : -1f;
        if (vert < 0)
        {
            movementActuator.ReleaseWall(propelDirection);
            //If pressing up and NOT pressing horizontally away from the wall
        }
        else if (vert > 0 && (hor == 0 || Mathf.Sign(hor) != Mathf.Sign(propelDirection)))
        {
            movementActuator.WallJumpUp(propelDirection);
        }
        else
        {
            movementActuator.WallJump(propelDirection);
        }
    }

    private void AttemptAirborneJump()
    {
        //Have some leeway on attempting a jump after having already fallen
        if (fallingTimer > 0)
        {
            movementActuator.GroundJump();
        }
        else if (remainingDoubleJumps > 0)
        {
            remainingDoubleJumps--;
            movementActuator.DoubleJump();
        }
    }

    private void ZeroFallingGraceTimer()
    {
        fallingTimer = -1f;
    }

    public void StartTargetting()
    {
        isTargetting = true;
        hangtimeTimer = hangtimePeriod;
    }
    public void StopTargetting()
    {
        isTargetting = false;
    }
}
