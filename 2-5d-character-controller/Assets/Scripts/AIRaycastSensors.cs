using UnityEngine;
using System.Collections;

public class AIRaycastSensors : MonoBehaviour
{

    Rigidbody body;
    public LayerMask collisionMask;

    bool leftCliff = false;
    bool rightCliff = false;

    float cliffLookSidewaysDistance = 1f;
    float cliffLookDownDistance = 1f;
    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>() as Rigidbody;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CliffSensors();
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
        Vector3 rayOrigin = body.transform.position - body.transform.right * cliffLookSidewaysDistance;
        RaycastHit hit;
        //There is a cliff to the left if there is NOT ground to the left
        leftCliff = !Physics.Raycast(rayOrigin, -body.transform.up, out hit, cliffLookDownDistance, collisionMask);
        debugRayColor = leftCliff ? Color.yellow : Color.red;
        Debug.DrawRay(rayOrigin, -body.transform.up * cliffLookDownDistance, debugRayColor);

        //Check for cliff on right
        rayOrigin = body.transform.position + body.transform.right * cliffLookSidewaysDistance;
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
}
