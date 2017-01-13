using UnityEngine;
using System.Collections;

public class MovesetWeapon : Weapon
{
    public GameObject downAttack;
    public GameObject upAttack;
    public GameObject sideAttack;
    public GameObject arialDownAttack;

    Vector3 upSpawnOffset = new Vector3(1f, 1f, 0f);
    Vector3 sideSpawnOffset = new Vector3(2f, 0f, 0f);
    Vector3 downSpawnOffset = new Vector3(1f, -0.5f, 0f);
    Vector3 arialDownSpawnOffset = new Vector3(0f, -1f, 0f);


    protected override void LoadWeaponParameters()
    {
        //Intentionally blank. Using defaults.
    }

    protected override void Fire()
    {
        Vector3 aimingVectorLocalSpace = aimingController.GetAimingVector();
        GameObject fireProjectile = sideAttack;
        Vector3 spawnOffset = sideSpawnOffset;

        if (aimingController.GetHorizontalAiming() == 0)
        {
            if (aimingController.GetVerticalAiming() == 1)
            {
                fireProjectile = upAttack;
                spawnOffset = upSpawnOffset;
            }
            if (aimingController.GetVerticalAiming() == -1)
            {
                {
                    if (contactSensor.GetContactState() == ContactState.AIRBORNE || contactSensor.GetContactState() == ContactState.WALLGRAB)
                    {
						fireProjectile = arialDownAttack;
						spawnOffset = arialDownSpawnOffset;
                    }
                    else
                    {
                        fireProjectile = downAttack;
                        spawnOffset = downSpawnOffset;
                    }
                }
            }
        }
        InstantiateMovesetProjectile(fireProjectile, spawnOffset);
    }

    private void InstantiateMovesetProjectile(GameObject fireProjectile, Vector3 spawnOffset)
    {
        Vector3 spawnLocation = new Vector3((float)aimingController.GetFacingDirection() * spawnOffset.x, spawnOffset.y, spawnOffset.z);
        GameObject newProjectile = (GameObject)Instantiate(fireProjectile, this.transform.position + spawnLocation, Quaternion.identity);
        newProjectile.GetComponent<MovesetProjectile>().LoadLaunchParameters(this, this.character, spawnLocation);
        newProjectile.GetComponent<MovesetProjectile>().Launch();
    }
}
