﻿using UnityEngine;
using System.Collections;

public class RangedWeapon : Weapon {

	float muzzleSpeed = 20f;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void Fire(){
		Vector3 aimingVector = aimingController.GetAimingVector();
		GameObject newProjectile = (GameObject)Instantiate(projectileGO, this.transform.position + aimingVector, Quaternion.identity);
		newProjectile.GetComponent<Rigidbody>().velocity = aimingVector  * muzzleSpeed;
	}
}
