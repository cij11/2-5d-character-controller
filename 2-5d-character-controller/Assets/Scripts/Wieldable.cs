﻿using UnityEngine;
using System.Collections;

public class Wieldable : MonoBehaviour {

	public GameObject projectileGO;

	AimingController aimingController;

	float muzzleSpeed = 20f;

	// Use this for initialization
	void Start () {
		aimingController = this.transform.parent.Find("ActionControllers").GetComponent<AimingController>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Fire(){
			Vector3 aimingVector = aimingController.GetAimingVector();
			GameObject newProjectile = (GameObject)Instantiate(projectileGO, this.transform.position + aimingVector, Quaternion.identity);
			newProjectile.GetComponent<Rigidbody>().velocity = aimingVector  * muzzleSpeed;
	}
}
