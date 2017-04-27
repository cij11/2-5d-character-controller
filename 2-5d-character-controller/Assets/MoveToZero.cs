using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToZero : MonoBehaviour {
	Rigidbody body;
	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody> () as Rigidbody;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		body.AddForce (-this.transform.position * 300 * Time.deltaTime);
		body.velocity = body.velocity * 0.95f;
	}
}
