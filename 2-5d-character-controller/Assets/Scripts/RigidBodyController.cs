﻿using UnityEngine;
using System.Collections;

public class RigidBodyController : MonoBehaviour
{
    ContactState contactState = ContactState.GROUNDED;
    MovementDirection verticalDirection = MovementDirection.NEUTRAL;
    MovementDirection horizontalDirection = MovementDirection.NEUTRAL;
    MovementDirection sideGrabbed = MovementDirection.NEUTRAL;

    public Vector3 gravityFocus = new Vector3(0, 0, 0);
    Rigidbody body;
    Collider physCollider;
    float width = 1f;
    float height = 1f;

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

    CollisionInfo collisions;
    RaycastOrigins raycastOrigins;
    float skinWidth = 0.005f;
    int verticalRayCount = 8;
    int horizontalRayCount = 3;
    float verticalRaySpacing;
    float horizontalRaySpacing;
    public LayerMask collisionMask;
    //Standing on a slope above this is steep - character will not rest on the slope, and will jump away
    float steepSlopeAngle = 60f;
    float minWallGrabAngle = 85f;

    //Store state variables and timers - eg, double jump used, time since touched wall
    StateInfo stateInfo;

    //Force that will be applied to keep character sticking to a wall if grabbing or sliding down it
    float wallHugForce = 200f;
    //Time in seconds that a wall jump can still occur after releasing the wall
    float wallJumpTimeWindow = 0.1f;
    int maxDoubleJumps = 2;

    float jetpackForce = 1200f;
    float parachuteFallSpeed = 1f;
    float paracuteDeceleration = 1.5f;
    public PhysicMaterial[] physicMaterials;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        physCollider = GetComponent<Collider>();
        UpdateRaycastOrigins();
        CalculateRaySpacing();
    }

    // Update is called once per frame
    void Update()
    {
        OrientToGravityFocus();
        DetermineContactState();

        ApplyGravity();
        ApplyWallHugForce();

        //Replace. Make idle material by default, then change to other materials as action dictates.
        AssignPhysicMaterial();

        print("Left wall angle: " + collisions.leftSurfaceAngle);
        print("Right wall angle: " + collisions.rightSurfaceAngle);

        collisions.Reset();
        Raycasts();

        stateInfo.Update();
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

    void DetermineContactState()
    {
        contactState = ContactState.GROUNDED;
        sideGrabbed = MovementDirection.NEUTRAL;

        //If there is a collision below, the character is either grounded or on a steep slope
        if (collisions.below)
        {
            stateInfo.remainingDoubleJumps = maxDoubleJumps;
            if (collisions.groundSlopeAngle < steepSlopeAngle)
            {
                contactState = ContactState.GROUNDED;
            }
            else
            {
                contactState = ContactState.STEEPSLOPE;
            }

        }
        //If there is a collision on the side but the slope is not vertical enough, the character is also on a steep slope.
        else if ((collisions.left && collisions.leftSurfaceAngle < minWallGrabAngle) || (collisions.right && collisions.rightSurfaceAngle < minWallGrabAngle))
        {
            contactState = ContactState.STEEPSLOPE;
        }
        else if (false)
        { //ledge grabs not currently implemented
            contactState = ContactState.LEDGEGRAB;
        }
        else if (false)
        {
            contactState = ContactState.LEDGEGRAB;
        }
        //If the character was in contact with a steep surface beside it within a few milliseconds, it counts as
        //grabbing the wall
        else if (stateInfo.leftWallContactTimer < wallJumpTimeWindow)
        {
            contactState = ContactState.WALLGRAB;
            sideGrabbed = MovementDirection.LEFT;
        }
        else if (stateInfo.rightWallContactTimer < wallJumpTimeWindow)
        {
            contactState = ContactState.WALLGRAB;
            sideGrabbed = MovementDirection.RIGHT;
        }
        else
        {
            contactState = ContactState.AIRBORNE;
        }
        print(contactState.ToString());
    }

    float VerticalSpeed()
    {
        return Vector3.Dot(body.velocity, body.transform.up);
    }

    float HorizontalSpeed()
    {
        return Vector3.Dot(body.velocity, body.transform.right);
    }

    void Raycasts()
    {
        UpdateRaycastOrigins();
        RaycastDown();
        RaycastLeft();
        RaycastRight();
    }

    //Cast rays down to see if near ground for purposes of jumping, air vs land control, and friction.
    void RaycastDown()
    {
        float rayLength = 0.05f;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector3 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += body.transform.right * (verticalRaySpacing * i);
            RaycastHit hit;
            bool isHit = Physics.Raycast(rayOrigin, -body.transform.up, out hit, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, -body.transform.up * rayLength, Color.red);

            //If any ray hits, register a collision below, and store the surface angle and normal
            if (isHit)
            {
                collisions.below = true;
                collisions.surfaceNormal = hit.normal;
                collisions.groundSlopeAngle = Vector3.Angle(hit.normal, body.transform.up);
            }
        }
    }

    void RaycastLeft()
    {
        float rayLength = 0.05f;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector3 rayOrigin = raycastOrigins.topLeft;
            rayOrigin -= body.transform.up * (horizontalRaySpacing * i);
            RaycastHit hit;
            bool isHit = Physics.Raycast(rayOrigin, -body.transform.right, out hit, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, -body.transform.right * rayLength, Color.red);

            //If any ray hits, register a collision below, and store the surface angle and normal
            if (isHit)
            {
                collisions.left = true;
                collisions.leftSurfaceAngle = Vector3.Angle(hit.normal, body.transform.up);
                if (collisions.leftSurfaceAngle > minWallGrabAngle)
                    stateInfo.leftWallContactTimer = 0f;
            }
        }
    }

    void RaycastRight()
    {
        float rayLength = 0.05f;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector3 rayOrigin = raycastOrigins.topRight;
            rayOrigin -= body.transform.up * (horizontalRaySpacing * i);
            RaycastHit hit;
            bool isHit = Physics.Raycast(rayOrigin, body.transform.right, out hit, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, body.transform.right * rayLength, Color.red);

            //If any ray hits, register a collision below, and store the surface angle and normal
            if (isHit)
            {
                collisions.right = true;
                collisions.rightSurfaceAngle = Vector3.Angle(hit.normal, body.transform.up);
                if (collisions.rightSurfaceAngle > minWallGrabAngle)
                    stateInfo.rightWallContactTimer = 0;
            }
        }
    }


    //If in a position to grab or slide down a wall, apply a small force towards the wall
    void ApplyWallHugForce()
    {
        if (contactState == ContactState.WALLGRAB)
        {
            if (collisions.left)
            {
                //adjacent to left wall, so apply wall hug force, and reset timer for that side
                body.AddForce(-body.transform.right * wallHugForce * Time.deltaTime);
            }
            else
            {
                //Likewise for the right wall
                body.AddForce(body.transform.right * wallHugForce * Time.deltaTime);
            }
        }
    }

    //Assign physic material to idle, unless on a steep slope, a wall, or input given
    //to change it to frictionless or low friction.
    void AssignPhysicMaterial()
    {
        //Reset to idle rest material by default
        physCollider.material = physicMaterials[(int)PhysicMatTypes.IDLE_STANDING];

        //If grabbing adjacent to a wall set high static friction, unless holding down
        if (contactState == ContactState.WALLGRAB)
        {
            if (stateInfo.isSlideCommandGiven)
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
        //If grounded and the slope is steep, a directional button is pressed, or ski is pressed, set to low friction
        if (contactState == ContactState.GROUNDED)
        {
            //And player is grounded
            if (stateInfo.isMoveHorizontalCommandGiven || stateInfo.isSlideCommandGiven)
            {
                //Change to slippery material
                physCollider.material = physicMaterials[(int)PhysicMatTypes.FRICTIONLESS];
            }
        }
        //If on a steep slope, set the material slippery
        if (contactState == ContactState.STEEPSLOPE)
        {
            physCollider.material = physicMaterials[(int)PhysicMatTypes.FRICTIONLESS];
        }
    }

    public void MoveHorizontalCommand(float direction)
    {
        //Horizontal move command was given, so set this true to prevent
        //physicMaterial resetting to idle in AssignPhysicMaterial method.
        stateInfo.isMoveHorizontalCommandGiven = true;

        if (contactState == ContactState.GROUNDED)
        {
            if (direction < 0)
            {
                MoveHorizontal(-1, landSpeedUpForce, landBreakForce, maxWalkSpeed);
            }
            else
            {
                MoveHorizontal(1, landSpeedUpForce, landBreakForce, maxWalkSpeed);
            }
        }

        if (contactState == ContactState.AIRBORNE)
        {
            if (direction < 0)
            {
                MoveHorizontal(-1, airSpeedUpForce, airBreakForce, maxAirSpeed);
            }
            else
            {
                MoveHorizontal(1, airSpeedUpForce, airBreakForce, maxAirSpeed);
            }
        }
    }

    //Determine what type of jump is appropriate when jump pressed, and apply
    public void JumpCommand()
    {
        //Detect ground and add upwards component to velocity if standing.
        //If terrain is too steep, walljump instead
        //Only allow jumping if standing on or adjacent to something
        if (contactState == ContactState.GROUNDED)
        {
            GroundJump();
        }
        else if (contactState == ContactState.STEEPSLOPE)
        {
            //TODO Jump 45 degrees, away from the slope
            //if uphill is top right
            //walljump left
            if (UphillDirection() > 0)
            {
                WallJump(-1);
            }
            else
            {
                //else if uphill is top left
                //walljump right
                WallJump(1);
            }

        }
        else if (stateInfo.leftWallContactTimer < wallJumpTimeWindow)
        {
            WallJump(1f);
        }
        else if (stateInfo.rightWallContactTimer < wallJumpTimeWindow)
        {
            WallJump(-1f);
        }
        //Even if we can't ground jump or wall jump, we might be able to double jump
        else if (stateInfo.remainingDoubleJumps > 0)
        {
            //decrement counter
            stateInfo.remainingDoubleJumps--;
            DoubleJump();
        }
    }


    //If the current vertical speed is < the jump velocity, cancel any existing
    //vertical speed and set to jump velocity
    void GroundJump()
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

    void WallJump(float direction)
    {
        //Cancel all existing velocity, and set equal to a 45 degree jump in the given direction
        body.velocity = body.transform.up * jumpSpeed + body.transform.right * direction * jumpSpeed;
    }

    //If on a ledge, jump straight up
    void LedgeJump()
    {

    }

    void DoubleJump()
    {
        //Same as ground jump for now. Later, give option to change direction on double jump
        GroundJump();
    }
    //Horizontal movement. Takes parameters for the rate of acceleration in the desired direction,
    //breaking movement in the current direction, and max speed to accelerate too.
    void MoveHorizontal(float direction, float speedUpForce, float breakForce, float maxSpeed)
    {
        //Get the current speed in the desired direction
        float currentSpeedInDesiredDirection = Vector3.Dot(body.velocity, direction * body.transform.right);

        //get vector parallel to surface normal.
        Vector3 surfaceNormalParallel = CalculatePerpendicular(collisions.surfaceNormal);

        //The vector we will apply walk forces to
        Vector3 horizontalVector = body.transform.right;

        //if moving down hill, align the force vector perpendicular ot the surface normal

        //If grounded
        //movement to the right should be aligned with the slope
        if (collisions.below)
        {
            horizontalVector = surfaceNormalParallel;
        }
        //If dot product of body right and surface normal is +ve then right is downhill, and left is uphill
        if (UphillDirection() > 0)
        {
            //Left is uphill, so don't align horizontal vector with slope if moving left
            if (direction < 0)
            {
                horizontalVector = body.transform.right;
            }
        }
        else
        {
            //Right is uphill, so don't align with slope if moving right
            if (direction > 0)
            {
                horizontalVector = body.transform.right;
            }
        }

        //dot product of desired and current velocity will be negative if walking the wrong way
        if (Mathf.Sign(currentSpeedInDesiredDirection) < 0)
        {
            body.AddForce(horizontalVector * direction * breakForce * Time.deltaTime);
        }
        else if (currentSpeedInDesiredDirection < maxSpeed)
        {
            //Cut off the applied force as slope approaches limit	
            body.AddForce(horizontalVector * direction * speedUpForce * Time.deltaTime);
        }
    }

    //Return -1 for left being uphill, 1 for right being uphill
    float UphillDirection()
    {
        float surfaceRightProjection = Vector3.Dot(collisions.surfaceNormal, body.transform.right);
        //Surface normal points in the opposite direction to uphill
        return -Mathf.Sign(surfaceRightProjection);
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

    public void SlideCommand(){
        stateInfo.isSlideCommandGiven = true;
    }

    void ApplyGravity()
    {
        body.AddForce(-this.transform.up * gravityForce * Time.deltaTime);
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

    //Find the corners of the square
    void UpdateRaycastOrigins()
    {

        float xoffset = (width / 2f) - skinWidth;
        float yoffset = (height / 2f) - skinWidth;
        float zpos = transform.position.z;
        raycastOrigins.bottomLeft = body.transform.position + body.transform.rotation * new Vector3(-xoffset, -yoffset, zpos);
        raycastOrigins.bottomRight = body.transform.position + body.transform.rotation * new Vector3(xoffset, -yoffset, zpos);
        raycastOrigins.topLeft = body.transform.position + body.transform.rotation * new Vector3(-xoffset, yoffset, zpos);
        raycastOrigins.topRight = body.transform.position + body.transform.rotation * new Vector3(xoffset, yoffset, zpos);
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool leftLedge, rightLedge;
        public Vector3 surfaceNormal;
        public float groundSlopeAngle;
        public float leftSurfaceAngle;
        public float rightSurfaceAngle;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            leftLedge = rightLedge = false;

            surfaceNormal = new Vector3(0f, 0f, 0f);
            groundSlopeAngle = 0f;
            leftSurfaceAngle = 0f;
            rightSurfaceAngle = 0f;
        }
    }

    //Vectors in worldspace where rays are cast from, coresponding to corners of the square.
    struct RaycastOrigins
    {
        public Vector3 topLeft, topRight;
        public Vector3 bottomLeft, bottomRight;
    }

    public struct StateInfo
    {
        public float leftWallContactTimer;
        public float rightWallContactTimer;
        public int remainingDoubleJumps;

        public bool isSlideCommandGiven;
        public bool isMoveHorizontalCommandGiven;
        public void Update()
        {
            leftWallContactTimer += Time.deltaTime;
            if (leftWallContactTimer > 10f)
            {
                leftWallContactTimer = 10f;
            }
            rightWallContactTimer += Time.deltaTime;
            if (rightWallContactTimer > 10f)
            {
                rightWallContactTimer = 10f;
            }
           isSlideCommandGiven = false;
           isMoveHorizontalCommandGiven = false;
        }
    }

    void CalculateRaySpacing()
    {
        verticalRaySpacing = (width - skinWidth * 2f) / (verticalRayCount - 1);
        //Don't cast rays all the way to the ground (eg, height * 0.75f) to prevent 'wall grabbing' bumps in the road
        horizontalRaySpacing = (height * 0.75f - skinWidth * 2f) / (horizontalRayCount - 1);
    }

    //Display raycast origins
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(raycastOrigins.bottomLeft, 0.01f);
        Gizmos.DrawSphere(raycastOrigins.bottomRight, 0.01f);
        Gizmos.DrawSphere(raycastOrigins.topLeft, 0.01f);
        Gizmos.DrawSphere(raycastOrigins.topRight, 0.01f);
    }
}
