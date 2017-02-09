using UnityEngine;
using System.Collections;

public class RangedProjectile : Projectile {

	float muzzleSpeed = 20f;
	Rigidbody body;

	// Update is called once per frame
	void Update () {
		IncreaseAge();
	}
		
	public override void Launch(){
		LaunchRanged();
	}

	void LaunchRanged(){
		damage = 10;
		body = GetComponent<Rigidbody> () as Rigidbody;
		body.velocity = worldLaunchVector * muzzleSpeed + firingCharData.GetMovementActuator().GetVelocity();
		Physics.IgnoreCollision(GetComponent<Collider>(), firingCharData.GetCharacter().GetComponent<Collider>());
	//	this.GetComponent<Rigidbody>().velocity = velocityVector  * muzzleSpeed;
	}

	void OnCollisionEnter(Collision other) {
		CharacterCorpus corpus = other.collider.GetComponent<CharacterCorpus>() as CharacterCorpus;
		if (corpus != null){
			corpus.TakeDamage(damage);
			Destroy(this.gameObject);
		}
		if (other.gameObject.layer == 8) {
			Destroy (this.gameObject);
		}
    }
}
