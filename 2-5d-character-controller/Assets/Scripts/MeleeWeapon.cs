using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon {

	public override void Fire(){
		Vector3 aimingVectorWorldSpace = aimingController.GetAimingVectorWorldSpace();
		GameObject newProjectile = (GameObject)Instantiate(launchableGO, this.transform.position + aimingVectorWorldSpace, Quaternion.identity);
		newProjectile.GetComponent<MeleeProjectile>().LoadLaunchParameters(this, this.character, aimingVectorWorldSpace);
		newProjectile.GetComponent<MeleeProjectile>().Launch();
	}
}
