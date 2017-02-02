using UnityEngine;
using System.Collections;

public class RangedWeapon : Weapon {

	// Use this for initialization
	protected override void LoadWeaponParameters(){
		SetCooldown(0.0f);
		SetFiringPeriod(0.0f);
	}
	protected override void Fire(){
        if (aimingController.GetHorizontalAiming() == 0 && aimingController.GetVerticalAiming() == -1)
        {
            if (!(contactSensor.GetContactState() == ContactState.AIRBORNE) && !(contactSensor.GetContactState() == ContactState.WALLGRAB))
            {
                CancelFiring();
            }
            else
            {
                LaunchRangedProjectile();
            }
        }
        else
        {
            LaunchRangedProjectile();
        }
	}

	private void LaunchRangedProjectile(){
		Vector3 worldAimingVector = aimingController.GetAimingVectorWorldSpace();
		GameObject newProjectile = (GameObject)Instantiate(launchableGO, this.transform.position + worldAimingVector, Quaternion.identity);
		newProjectile.GetComponent<RangedProjectile>().LoadLaunchParameters(this.character, worldAimingVector, aimingController.GetFacingDirection());
		newProjectile.GetComponent<RangedProjectile>().Launch();
	}
}
