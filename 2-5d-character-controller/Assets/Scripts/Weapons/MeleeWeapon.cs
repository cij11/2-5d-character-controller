using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon
{
    float lungeSpeed = 10f;

    protected override void LoadWeaponParameters()
    {
        //Intentionally blank. Using defaults.
    }

    protected override void Fire()
    {
        if (aimingController.GetHorizontalAiming() == 0 && aimingController.GetVerticalAiming() == -1)
        {
            if (!(contactSensor.GetContactState() == ContactState.AIRBORNE) && !(contactSensor.GetContactState() == ContactState.WALLGRAB))
            {
                CancelFiring();
            }
            else
            {
                LaunchMeleeProjectile();
				movementController.Lunge(aimingController.GetAimingVector(), lungeSpeed);
            }
        }
        else
        {
            LaunchMeleeProjectile();
			movementController.Lunge(aimingController.GetAimingVector(), lungeSpeed);
        }
    }

    private void LaunchMeleeProjectile()
    {
        Vector3 aimingVectorWorldSpace = aimingController.GetAimingVectorWorldSpace();
        GameObject newProjectile = (GameObject)Instantiate(launchableGO, this.transform.position + aimingVectorWorldSpace, Quaternion.identity);
        newProjectile.GetComponent<MeleeProjectile>().LoadLaunchParameters(this, this.character, aimingVectorWorldSpace, aimingController.GetFacingDirection());
        newProjectile.GetComponent<MeleeProjectile>().Launch();
    }
}