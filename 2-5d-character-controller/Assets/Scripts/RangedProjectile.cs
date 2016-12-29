using UnityEngine;
using System.Collections;

public class RangedProjectile : Projectile {

	float muzzleSpeed = 20f;
	// Update is called once per frame
	void Update () {
		IncreaseAge();
	}

	public override void Launch(){
		this.GetComponent<Rigidbody>().velocity = launchVector  * muzzleSpeed;
	}


}
