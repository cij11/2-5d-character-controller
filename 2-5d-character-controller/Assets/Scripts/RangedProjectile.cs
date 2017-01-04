using UnityEngine;
using System.Collections;

public class RangedProjectile : Projectile {

	float muzzleSpeed = 20f;
	Vector3 velocityVector;
	// Update is called once per frame
	void Update () {
		IncreaseAge();
		TranslateProjectile();
	}

	void TranslateProjectile(){
		this.transform.position = this.transform.position + velocityVector * muzzleSpeed * Time.deltaTime;
	}

	public override void Launch(){
		LaunchRanged();
	}

	void LaunchRanged(){
		this.transform.SetParent(firingWeapon.transform);
		this.transform.localPosition = launchVector;

		velocityVector = this.transform.position - firingWeapon.transform.position;
		velocityVector.Normalize();
		this.transform.SetParent(null);
	//	this.GetComponent<Rigidbody>().velocity = velocityVector  * muzzleSpeed;
	}

	void OnTriggerEnter(Collider other) {
		CharacterHealth characterHealth = other.GetComponent<CharacterHealth>() as CharacterHealth;
		if (characterHealth != null){
			characterHealth.TakeDamage(40);
		}

		Destroy(this.gameObject);
    }
}
