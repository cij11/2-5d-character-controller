using UnityEngine;
using System.Collections;

public class RangedProjectile : Projectile {

	float muzzleSpeed = 20f;
	// Update is called once per frame
	void Update () {
		IncreaseAge();
	}

	public override void Launch(){
		LaunchRanged();
	}

	void LaunchRanged(){
		this.transform.SetParent(firingWeapon.transform);
		this.transform.localPosition = launchVector;

		Vector3 velocityVector = this.transform.position - firingWeapon.transform.position;
		this.transform.SetParent(null);
		this.GetComponent<Rigidbody>().velocity = velocityVector  * muzzleSpeed;
	}
}
