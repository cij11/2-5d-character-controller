using UnityEngine;
using System.Collections;

public class AIRaycastSensors : MonoBehaviour
{

    Rigidbody body;
    public LayerMask collisionMask;

    UpdateTimer updateTimer;
    UpdateTimer LOSUpdateTimer;

    bool leftCliff = false;
    bool rightCliff = false;

    bool targetInLOS = false;
    float cliffLookSidewaysDistance = 1f;
    float cliffLookDownDistance = 2f;
    // Use this for initialization
    void Start()
    {
        body = this.transform.parent.GetComponent<Rigidbody>() as Rigidbody;
        updateTimer = new UpdateTimer(5);
        LOSUpdateTimer = new UpdateTimer(5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (updateTimer.TryUpdateThisTick())
        {
            CliffSensors();
        }
    }

    void CliffSensors()
    {
        ResetCliffSensors();
        CastCliffDetectionRays();
    }

    void ResetCliffSensors()
    {
        leftCliff = false;
        rightCliff = false;
    }

    void CastCliffDetectionRays()
    {
        Color debugRayColor = Color.red;
        //Check for cliff on left
        Vector3 rayOrigin = body.transform.position - body.transform.right * cliffLookSidewaysDistance + body.transform.up;
        RaycastHit hit;
        //There is a cliff to the left if there is NOT ground to the left
        leftCliff = !Physics.Raycast(rayOrigin, -body.transform.up, out hit, cliffLookDownDistance, collisionMask);
        debugRayColor = leftCliff ? Color.yellow : Color.red;
        Debug.DrawRay(rayOrigin, -body.transform.up * cliffLookDownDistance, debugRayColor);

        //Check for cliff on right
        rayOrigin = body.transform.position + body.transform.right * cliffLookSidewaysDistance + body.transform.up;
        rightCliff = !Physics.Raycast(rayOrigin, -body.transform.up, out hit, cliffLookDownDistance, collisionMask);
        debugRayColor = rightCliff ? Color.yellow : Color.red;
        Debug.DrawRay(rayOrigin, -body.transform.up * cliffLookDownDistance, debugRayColor);
    }

    public bool GetLeftCliff()
    {
        return leftCliff;
    }

    public bool GetRightCliff()
    {
        return rightCliff;
    }
    public bool GetAnyCliff()
    {
        return leftCliff || rightCliff;
    }

    public float GetCliffDirection()
    {
        if (leftCliff)
        {
            return -1f;
        }
        else
        {
            return 1f;
        }
    }

    //Note, will only actually on frames where LOSUpdateTimer expires, or if there
    //was previously a target in LOS.
    public bool IsGameobjectInLOS(GameObject gameObject)
    {
        if(LOSUpdateTimer.TryUpdateThisTick() || targetInLOS){
        RaycastHit hit;
        Vector3 rayDirection = gameObject.transform.position - body.position;
        Debug.DrawRay(body.position, rayDirection, Color.green);
        if (Physics.Raycast(body.position, rayDirection, out hit))
        {
            if (hit.transform == gameObject.transform)
            {
                print("Player in los");
                targetInLOS = true;
            }
            else
            {
                print("Raycast hit, not target");
                targetInLOS = false;
            }
        }
        else{
            print("No raycast hit");
            targetInLOS = false;
        }
        }
        return targetInLOS;
    }
}
