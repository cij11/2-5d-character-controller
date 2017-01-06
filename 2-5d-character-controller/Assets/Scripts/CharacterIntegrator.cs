using UnityEngine;
using System.Collections;

public class CharacterIntegrator : MonoBehaviour
{

    CharacterMovementActuator movementActuator;
    CharacterContactSensor contactSensor;
    bool isTargetting;

    float jumpAfterFallingGracePeriod = 0.1f;
    float fallingTimer = 0f;
    int maxDoubleJumps = 1;
    int remainingDoubleJumps = 0;

    // Use this for initialization
    void Start()
    {
        movementActuator = GetComponent<CharacterMovementActuator>() as CharacterMovementActuator;
        contactSensor = GetComponent<CharacterContactSensor>() as CharacterContactSensor;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateArialJumpingTrackers();
        if (isTargetting)
        {
            movementActuator.ParachuteCommand();
        }
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

    public void MoveHorizontal(float direction)
    {
        if (!isTargetting)
            movementActuator.MoveHorizontalCommand(direction);
        else
            movementActuator.FaceDirectionCommand(direction);
    }
    public void MoveVertical(float direction)
    {

    }

    public void Grab()
    {

    }

    //Determine what type of jump is appropriate when jump pressed, and apply
    public void Jump(float hor, float vert)
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

    private void ZeroFallingGraceTimer(){
        fallingTimer = -1f;
    }

    public void StartTargetting()
    {
        isTargetting = true;
    }
    public void StopTargetting()
    {
        isTargetting = false;
    }
}
