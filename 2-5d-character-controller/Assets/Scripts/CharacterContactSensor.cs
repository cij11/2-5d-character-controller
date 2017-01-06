using UnityEngine;
using System.Collections;

public class CharacterContactSensor : MonoBehaviour
{

    ContactState contactState = ContactState.FLATGROUND;
    MovementDirection sideGrabbed = MovementDirection.NEUTRAL;
    Rigidbody body;
    Collider physCollider;
    public FixedJoint backgroundGrabJoint;
    float width = 0.6f;
    float height = 0.9f;

    CollisionInfo collisions;
    RaycastOrigins raycastOrigins;
    float skinWidth = 0.05f;
    float detectionRayLengthGround = 0.3f;
    float detectionRayLengthSides = 0.1f;
    int verticalRayCount = 4;
    int horizontalRayCount = 2;
    float verticalRaySpacing;
    float horizontalRaySpacing;
    public int updatePeriod = 1;
    int updateTimer;
    public LayerMask collisionMask;
    //Standing on a slope above this is steep - character will not rest on the slope, and will jump away
    float steepSlopeAngle = 60f;
    float minWallGrabAngle = 80f;

    //Force that will be applied to keep character sticking to a wall if grabbing or sliding down it
    float wallHugForce = 200f;

    CharacterIntegrator integrator;

    // Use this for initialization
    void Start()
    {
        updateTimer = Random.Range(0, updatePeriod);
        body = GetComponent<Rigidbody>();
        physCollider = GetComponent<Collider>();
        integrator = GetComponent<CharacterIntegrator>();
        UpdateRaycastOrigins();
        CalculateRaySpacing();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateTimer++;
        if (updateTimer == updatePeriod)
        {
            updateTimer = 0;
            DoFixedUpdate();
        }
    }

    void DoFixedUpdate()
    {
        DetermineContactState();
        collisions.Reset();
        Raycasts();
    }

    void DetermineContactState()
    {
        contactState = ContactState.FLATGROUND;
        sideGrabbed = MovementDirection.NEUTRAL;

        if (backgroundGrabJoint != null)
        {
            contactState = ContactState.BACKGROUNDGRAB;
        }
        //If there is a collision below, the character is either FLATGROUND, a steep slope, or a slanted wall
        else if (collisions.below)
        {
            if (collisions.groundSlopeAngle < steepSlopeAngle)
            {
                contactState = ContactState.FLATGROUND;
            }
            else if (collisions.groundSlopeAngle < minWallGrabAngle)
            {
                contactState = ContactState.STEEPSLOPE;
            }
            else
            {
                contactState = ContactState.WALLGRAB;

                if (UphillDirection() > 0)
                {
                    sideGrabbed = MovementDirection.RIGHT;
                }
                else
                {
                    sideGrabbed = MovementDirection.LEFT;
                }
            }
        }
        //If there is a collision on the side but the slope is not vertical enough, the character is also on a steep slope.
        else if ((collisions.left && collisions.leftSurfaceAngle < minWallGrabAngle) || (collisions.right && collisions.rightSurfaceAngle < minWallGrabAngle))
        {
            contactState = ContactState.STEEPSLOPE;
        }
        //If the character was in contact with a steep surface beside it within a few milliseconds, it counts as
        //grabbing the wall
        else if (collisions.left)
        {
            contactState = ContactState.WALLGRAB;
            sideGrabbed = MovementDirection.LEFT;
        }
        else if (collisions.right)
        {
            contactState = ContactState.WALLGRAB;
            sideGrabbed = MovementDirection.RIGHT;
        }
        else
        {
            contactState = ContactState.AIRBORNE;
        }
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
        float rayLength = detectionRayLengthGround;

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
                collisions.groundNormal = hit.normal;
                collisions.groundSlopeAngle = Vector3.Angle(hit.normal, body.transform.up);
            }
        }
    }

    void RaycastLeft()
    {
        float rayLength = detectionRayLengthSides;

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
                collisions.leftWallNormal = hit.normal;
            }
        }
    }

    void RaycastRight()
    {
        float rayLength = detectionRayLengthSides;

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
                collisions.rightWallNormal = hit.normal;
            }
        }
    }

    //Return -1 for left being uphill, 1 for right being uphill
    float UphillDirection()
    {
        float surfaceRightProjection = Vector3.Dot(collisions.groundNormal, body.transform.right);
        //Surface normal points in the opposite direction to uphill
        return -Mathf.Sign(surfaceRightProjection);
    }

    //Find the corners of the square
    void UpdateRaycastOrigins()
    {

        float xoffset = (width / 2f) - skinWidth;
        float yoffset = (height / 2f) - skinWidth;
        float zpos = transform.position.z;
        raycastOrigins.bottomLeft = physCollider.transform.position + physCollider.transform.rotation * new Vector3(-xoffset, -yoffset, zpos);
        raycastOrigins.bottomRight = physCollider.transform.position + physCollider.transform.rotation * new Vector3(xoffset, -yoffset, zpos);
        raycastOrigins.topLeft = physCollider.transform.position + physCollider.transform.rotation * new Vector3(-xoffset, yoffset, zpos);
        raycastOrigins.topRight = physCollider.transform.position + physCollider.transform.rotation * new Vector3(xoffset, yoffset, zpos);
    }

    public struct CollisionInfo
    {
        public bool below;
        public bool left, right;

        public Vector3 groundNormal;
        public Vector3 leftWallNormal;
        public Vector3 rightWallNormal;
        public float groundSlopeAngle;
        public float leftSurfaceAngle;
        public float rightSurfaceAngle;

        public void Reset()
        {
           below = false;
            left = right = false;

            groundNormal = new Vector3(0f, 0f, 0f);
            leftWallNormal = new Vector3(0f, 0f, 0f);
            rightWallNormal = new Vector3(0f, 0f, 0f);
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

    public ContactState GetContactState()
    {
        return this.contactState;
    }
    public MovementDirection GetSideGrabbed()
    {
        return sideGrabbed;
    }

    public float GetUphillDirection()
    {
        return UphillDirection();
    }

    public Vector3 GetGroundNormal()
    {
        return collisions.groundNormal;
    }

    public Vector3 GetLeftWallNormal()
    {
        return collisions.leftWallNormal;
    }
    public Vector3 GetRightWallNormal()
    {
        return collisions.rightWallNormal;
    }

    public Vector3 GetGrabbedWallNormal()
    {
        if (collisions.left)
        {
            return collisions.leftWallNormal;
        }
        else
        {
            return collisions.rightWallNormal;
        }
    }

    public bool GetIsInContactWithTerrain()
    {
        if (contactState == ContactState.FLATGROUND) return true;
        if (contactState == ContactState.STEEPSLOPE) return true;
        if (contactState == ContactState.WALLGRAB) return true;
        return false;
    }
}
