using UnityEngine;
using System.Collections;

public class MeleeProjectile : Projectile {
	
	float reachRange = 0.7f;
	// Update is called once per frame
	void Update () {
		IncreaseAge();
	}

	public override void Launch(){
		LaunchMelee();
	}

	void LaunchMelee(){
		this.transform.SetParent(firingWeapon.transform);
		this.transform.position = this.transform.parent.position +  worldLaunchVector * reachRange;
		AlignToLauncher();
	}

	void AlignToLauncher(){
		Vector3 weaponToProjectile = this.transform.position - firingWeapon.transform.position;
		this.transform.rotation = Quaternion.FromToRotation(this.transform.right, weaponToProjectile) * transform.rotation;
	}

	void OnTriggerEnter(Collider other) {
		CharacterCorpus corpus = other.GetComponent<CharacterCorpus>() as CharacterCorpus;
		if (corpus != null){
			Character otherCharacter = other.gameObject.GetComponent<Character>() as Character;
			if(!otherCharacter.Equals(firingCharacter)){
				corpus.TakeDamage(damage);
				corpus.TakeKnockback(worldLaunchVector, knockbackSpeed);
			}
		}
    }
}
