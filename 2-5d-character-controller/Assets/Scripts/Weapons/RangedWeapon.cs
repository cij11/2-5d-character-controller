using UnityEngine;
using System.Collections;

public class RangedWeapon : Weapon {

	float muzzleSpeed = 20f;

	// Use this for initialization
	protected override void LoadWeaponParameters(){
		SetCooldown(0.0f);
	}
	protected override void Fire(){
		Vector3 worldAimingVector = aimingController.GetAimingVectorWorldSpace();
		GameObject newProjectile = (GameObject)Instantiate(launchableGO, this.transform.position + worldAimingVector, Quaternion.identity);
		newProjectile.GetComponent<RangedProjectile>().LoadLaunchParameters(this, this.character, worldAimingVector);
		newProjectile.GetComponent<RangedProjectile>().Launch();
	}
}
