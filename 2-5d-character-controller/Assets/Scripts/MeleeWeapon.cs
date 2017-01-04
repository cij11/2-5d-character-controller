using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon {

	public override void Fire(){
		Vector3 aimingVector = aimingController.GetAimingVector();
		GameObject newProjectile = (GameObject)Instantiate(launchableGO, this.transform.position + aimingVector, Quaternion.identity);
		newProjectile.GetComponent<MeleeProjectile>().LoadLaunchParameters(this, this.character, aimingVector);
		newProjectile.GetComponent<MeleeProjectile>().Launch();
	}
}
