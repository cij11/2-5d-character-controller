using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	Rigidbody body;
	ItemTrigger itemTrigger;
	// Use this for initialization
	void Start () {
		body = this.GetComponent<Rigidbody>() as Rigidbody;
		itemTrigger = GetComponentInChildren<ItemTrigger> () as ItemTrigger;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Throw(Vector3 direction, float speed){
		this.transform.parent = null;
		body.isKinematic = false;
		body.velocity = direction.normalized * speed;
		itemTrigger.enabled = true;
		itemTrigger.EnableSphereCollider();
	}

	public void PickUp(){
		body.isKinematic = true;
		itemTrigger.DisableSphereCollider();
		itemTrigger.enabled = false;
	}
}
