using UnityEngine;
using System.Collections;

public class AIRadiusObjectSensor : MonoBehaviour
{

    float sensorRadius = 10f;

    bool playerDetected = false;
    UpdateTimer updateTimer;
    Rigidbody body;
    public LayerMask collisionMask;
    Collider[] hitColliders;
    // Use this for initialization
    void Start()
    {
        body = this.transform.parent.transform.GetComponent<Rigidbody>() as Rigidbody;
        updateTimer = new UpdateTimer(10);
        hitColliders = new Collider[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (updateTimer.TryUpdateThisTick())
        {
            hitColliders = Physics.OverlapSphere(body.transform.position, sensorRadius, collisionMask);
        }
    }

    public bool PlayerDetected()
    {
        GameObject playerObject = GetPlayerGameObject();
		if (playerObject != null){
			return true;
		}
		else{
			return false;
		}
    }

    public GameObject GetPlayerGameObject()
    {
		GameObject playerObject = null;
		foreach (Collider collider in hitColliders)
        {
            if (collider.tag == "Player")
            {
                playerObject = collider.gameObject;
            }
        }
		return playerObject;
    }
}
