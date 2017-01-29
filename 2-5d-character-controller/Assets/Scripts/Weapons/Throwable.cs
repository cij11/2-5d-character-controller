using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour {
	Rigidbody body;
	// Use this for initialization
	void Start () {
		body = this.GetComponent<Rigidbody>() as Rigidbody;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Throw(Vector3 direction, float speed){
		this.transform.parent = null;
		body.isKinematic = false;
		body.velocity = direction.normalized * speed;
	}
}
