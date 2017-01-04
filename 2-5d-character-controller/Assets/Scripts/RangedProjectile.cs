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
		velocityVector = worldLaunchVector;
	//	this.GetComponent<Rigidbody>().velocity = velocityVector  * muzzleSpeed;
	}

	void OnTriggerEnter(Collider other) {
		CharacterCorpus corpus = other.GetComponent<CharacterCorpus>() as CharacterCorpus;
		if (corpus != null){
			corpus.TakeDamage(40);
		}

		Destroy(this.gameObject);
    }
}
