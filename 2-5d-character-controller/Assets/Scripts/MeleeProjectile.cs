using UnityEngine;
using System.Collections;

public class MeleeProjectile : Projectile {
	
	float reachRange = 2f;
	// Update is called once per frame
	void Update () {
		IncreaseAge();
	}

	public override void Launch(){
		LaunchMelee();
	}

	void LaunchMelee(){
		this.transform.SetParent(firingWeapon.transform);
		this.transform.localPosition = launchVector * reachRange;
		AlignToLauncher();
	}

	void AlignToLauncher(){
		Vector3 weaponToProjectile = this.transform.position - firingWeapon.transform.position;
		this.transform.rotation = Quaternion.FromToRotation(this.transform.right, weaponToProjectile) * transform.rotation;
	}

	void OnTriggerEnter(Collider other) {
		CharacterHealth characterHealth = other.GetComponent<CharacterHealth>() as CharacterHealth;
		if (characterHealth != null){
			characterHealth.TakeDamage(40);
		}
    }
}
