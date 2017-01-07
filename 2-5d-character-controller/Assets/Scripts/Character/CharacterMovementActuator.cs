using UnityEngine;
using System.Collections;

public class CharacterMovementActuator : MonoBehaviour
{

    public Vector3 gravityFocus = new Vector3(0, 0, 0);
    Rigidbody body;
    Collider physCollider;
    CharacterContactSensor contactSensor;

    //Speed the character walks at when left or right held
    float maxWalkSpeed = 12;
    //Time to attain walk velocity from stationary.
    float landSpeedUpForce = 1500f;
    float landBreakForce = 5000f;

    float maxAirSpeed = 12;
    float airSpeedUpForce = 600f;
    float airBreakForce = 1500f;

    float gravityForce = 900f;

    float jumpSpeed = 10f;
    float wallJumpClearanceSpeed = 3f;
    bool isSlideCommandGiven = false;
    float releaseSpeed = 2f;

    float wallHugForce = 200f;

    float jetpackForce = 1200f;
    float parachuteFallSpeed = 0.2f;
    float paracuteDeceleration = 1.5f;
    float terminalArialSpeed = -50f;
    float terminalWallSlideSpeed = -5f;
    public PhysicMaterial[] physicMaterials;

    bool isMoveHorizontalCommandGiven = false;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        contactSensor = GetComponent<CharacterContactSensor>();
        physCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OrientToGravityFocus();

        ApplyGravity();
        LimitWallSlideSpeed();

        //Replace. Make idle material by default, then change to other materials as action dictates.
        AssignPhysicMaterial();
        isMoveHorizontalCommandGiven = false;
        isSlideCommandGiven = false;
    }

    void OrientToGravityFocus()
    {
        //Get a vector to point feet at. This might be the center of a hull for a round world, or down for a ship
        //with artificial gravity. Give this body position as arguement to calculate down or radial orientation position
        Vector3 pointToOrientTo = gravityFocus;

        //Align the up vector of the body with the vector from the focus towards the body.
        Vector3 vectorFromFocusToBody = body.position - gravityFocus;
        vectorFromFocusToBody.Normalize();
        body.transform.rotation = Quaternion.FromToRotation(body.transform.up, vectorFromFocusToBody) * transform.rotation;
    }



    float VerticalSpeed()
    {
        return Vector3.Dot(body.velocity, body.transform.up);
    }

    float HorizontalSpeed()
    {
        return Vector3.Dot(body.velocity, body.transform.right);
    }

    //If in a position to grab or slide down a wall, apply a small force towards the wall
    public void ApplyWallHugForce()
    {
        if (contactSensor.GetContactState() == ContactState.WALLGRAB)
        {
            body.AddForce(-contactSensor.GetGrabbedWallNormal() * wallHugForce * Time.deltaTime);
        }
    }

    void LimitWallSlideSpeed()
    {
        //Negate gravity
    }

    //Assign physic material to idle, unless on a steep slope, a wall, or input given
    //to change it to frictionless or low friction.
    void AssignPhysicMaterial()
    {
        //Reset to idle rest material by default
        physCollider.material = physicMaterials[(int)PhysicMatTypes.IDLE_STANDING];

        //If grabbing adjacent to a wall set high static friction, unless holding down
        if (contactSensor.GetContactState() == ContactState.WALLGRAB)
        {
            if (isSlideCommandGiven)
            {
                //If down is held, set the material to be smoother
                physCollider.material = physicMaterials[(int)PhysicMatTypes.LOW_KINETIC_NO_STATIC];
            }
            else
            {
                //else make very sticky
                physCollider.material = physicMaterials[2];
            }
        }
        //If FLATGROUND and a directional command is given, or a slide command is given, set to low friction
        if (contactSensor.GetContactState() == ContactState.FLATGROUND)
        {
            //And player is FLATGROUND
            if (isMoveHorizontalCommandGiven || isSlideCommandGiven)
            {
                //Change to slippery material
                physCollider.material = physicMaterials[(int)PhysicMatTypes.FRICTIONLESS];
            }
        }
        //If on a steep slope, set the material slippery
        if (contactSensor.GetContactState() == ContactState.STEEPSLOPE)
        {
            physCollider.material = physicMaterials[(int)PhysicMatTypes.FRICTIONLESS];
        }

        //If the collider is airborne, make frictionless. Prevents juddering
        //when head touching the underside of a surface, and allows movement
        //if ground has not been detected (eg, if standing on peak)
        if (contactSensor.GetContactState() == ContactState.AIRBORNE)
        {
            physCollider.material = physicMaterials[(int)PhysicMatTypes.FRICTIONLESS];
        }
    }

    public void MoveHorizontalCommand(float direction)
    {
        //Horizontal move command was given, so set this true to prevent
        //physicMaterial resetting to idle in AssignPhysicMaterial method.
        isMoveHorizontalCommandGiven = true;

        if (contactSensor.GetContactState() == ContactState.FLATGROUND)
        {
            MoveFlatHorizontal(Mathf.Sign(direction), landSpeedUpForce, landBreakForce, maxWalkSpeed);
        }

        if (contactSensor.GetContactState() == ContactState.STEEPSLOPE)
        {
            MoveSteepHorizontal(Mathf.Sign(direction), landSpeedUpForce, landBreakForce, maxWalkSpeed);
        }

        if (contactSensor.GetContactState() == ContactState.AIRBORNE)
        {
            MoveArialHorizontal(Mathf.Sign(direction), airSpeedUpForce, airBreakForce, maxAirSpeed);
        }
    }

    //If the current vertical speed is < the jump velocity, cancel any existing
    //vertical speed and set to jump velocity
    public void GroundJump()
    {
        //Projection of our velocity directed up (if +ve,) or down (if -ve)
        float verticalSpeed = Vector3.Dot(body.velocity, body.transform.up);
        //If moving less than jump Velocity vertically
        if (verticalSpeed < jumpSpeed)
        {
            //Cancel the current downwards velocity, and set it to the jump velocity
            //	body.velocity = body.velocity + body.transform.up * (-verticalSpeed + jumpSpeed);
            CancelVelocityAlongVector(body.transform.up);
            body.velocity = body.velocity + body.transform.up * jumpSpeed;
        }
    }

    public void ReleaseWall(float direction)
    {
        body.velocity = body.transform.right * direction * releaseSpeed;
    }
    public void WallJump(float direction)
    {
        //Cancel all existing velocity, and set equal to a 45 degree jump in the given direction
        body.velocity = body.transform.up * jumpSpeed + body.transform.right * direction * jumpSpeed;
    }

    //If on a ledge, jump up, and slightly away from the wall.
    public void WallJumpUp(float direction)
    {
        body.velocity = body.transform.up * jumpSpeed + body.transform.right * direction * wallJumpClearanceSpeed;
    }

    public void DoubleJump()
    {
        //Same as ground jump for now. Later, give option to change direction on double jump
        GroundJump();
    }
    //Horizontal movement. Takes parameters for the rate of acceleration in the desired direction,
    //breaking movement in the current direction, and max speed to accelerate too.
    void MoveFlatHorizontal(float direction, float speedUpForce, float breakForce, float maxSpeed)
    {
        //Get the current speed in the desired direction
        float currentSpeedInDesiredDirection = Vector3.Dot(body.velocity, direction * body.transform.right);
        //The vector we will apply walk forces to
        Vector3 horizontalVector = CalculatePerpendicular(contactSensor.GetGroundNormal());

        //dot product of desired and current velocity will be negative if walking the wrong way
        if (Mathf.Sign(currentSpeedInDesiredDirection) < 0)
        {
            body.AddForce(horizontalVector * direction * breakForce * Time.deltaTime);
        }
        else if (currentSpeedInDesiredDirection < maxSpeed)
        {
            body.AddForce(horizontalVector * direction * speedUpForce * Time.deltaTime);
        }
    }

    void MoveSteepHorizontal(float direction, float speedUpForce, float breakForce, float maxSpeed)
    {
        float uphill = contactSensor.GetUphillDirection();

        //If trying to break on a steep slope, direct a force downwards.
        if (Mathf.Sign(direction) != Mathf.Sign(uphill))
        {
            //Only break if currently moving upwards
            if (VerticalSpeed() > 0)
            {
                body.AddForce(-body.transform.up * breakForce * Time.deltaTime);
            }
            //Allow to accelerate downhill if not at max speed
            else
            {
                if (Mathf.Abs(VerticalSpeed()) < maxWalkSpeed)
                {
                    body.AddForce(-body.transform.up * speedUpForce * Time.deltaTime);
                }
            }
        }
        //Otherwise, direct a force horizontally
        else
        {
            if (Mathf.Abs(HorizontalSpeed()) < maxWalkSpeed)
            {
                body.AddForce(body.transform.right * direction * speedUpForce * Time.deltaTime);
            }
        }
    }

    void MoveArialHorizontal(float direction, float speedUpForce, float breakForce, float maxSpeed)
    {
        //Get the current speed in the desired direction
        float currentSpeedInDesiredDirection = Vector3.Dot(body.velocity, direction * body.transform.right);
        Vector3 horizontalVector = body.transform.right;
        //dot product of desired and current velocity will be negative if walking the wrong way
        if (Mathf.Sign(currentSpeedInDesiredDirection) < 0)
        {
            body.AddForce(horizontalVector * direction * breakForce * Time.deltaTime);
        }
        else if (currentSpeedInDesiredDirection < maxSpeed)
        {
            body.AddForce(horizontalVector * direction * speedUpForce * Time.deltaTime);
        }
    }

    public void JetpackCommand()
    {
        body.AddForce(body.transform.up * jetpackForce * Time.deltaTime);
    }

    public void ParachuteCommand()
    {
        float verticalSpeed = Vector3.Dot(body.velocity, body.transform.up);
        //If the character is moving down
        if (verticalSpeed < 0)
        {
            //And the downwards speed is greater than the parachuteSpeed
            if (Mathf.Abs(verticalSpeed) > parachuteFallSpeed)
            {
                //Apply a force counter to gravity
                body.AddForce(body.transform.up * gravityForce * paracuteDeceleration * Time.deltaTime);
            }
        }
    }

    void ApplyGravity()
    {
        float verticalSpeed = GetVerticalSpeed();
        if (contactSensor.GetContactState() != ContactState.WALLGRAB)
        {
            if (verticalSpeed > terminalArialSpeed)
            {
                body.AddForce(-this.transform.up * gravityForce * Time.deltaTime);
            }
        }
        else
        {
            if (verticalSpeed > terminalWallSlideSpeed)
            {
                body.AddForce(-this.transform.up * gravityForce * Time.deltaTime);
            }
        }
    }

    //Use the dot product to find the scalar projection of the current velocity
    //in the undesired direction.
    //Subtract this scalar * the undesired direction from the body velocity
    //to zero all movement in the diretion of the given vector
    void CancelVelocityAlongVector(Vector3 vectorToKillVelocity)
    {
        vectorToKillVelocity.Normalize();
        float speedInUndesiredDiretion = Vector3.Dot(body.velocity, vectorToKillVelocity);
        body.velocity = body.velocity - vectorToKillVelocity * speedInUndesiredDiretion;
    }

    //Return the vector at 90 degrees to the arguement vector.
    Vector3 CalculatePerpendicular(Vector3 vec)
    {
        return new Vector3(vec.y, -vec.x, 0);
    }

    public float GetVerticalSpeed()
    {
        return VerticalSpeed();
    }

    public float GetHorizontalSpeed()
    {
        return HorizontalSpeed();
    }

    public void SetWalkingSpeed(float newSpeed){
        maxWalkSpeed = newSpeed;
    }
}