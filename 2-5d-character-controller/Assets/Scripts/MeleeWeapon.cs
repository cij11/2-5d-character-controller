using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon {

	// Update is called once per frame
	void Update () {
	
	}

	public override void Fire(){
		Vector3 aimingVector = aimingController.GetAimingVector();
		GameObject newProjectile = (GameObject)Instantiate(projectileGO, this.transform.position + aimingVector, Quaternion.identity);
		newProjectile.GetComponent<Rigidbody>().velocity = aimingVector  * 5f;
	}
}
