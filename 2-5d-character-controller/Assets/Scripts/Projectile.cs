using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {

	protected Weapon firingWeapon;
	protected Character firingCharacter;
	protected Vector3 launchVector;
	protected float maxLifespan = 1f;
	float age;

	// Use this for initialization
	void Start () {
		age = 0f;
	}

	public void LoadLaunchParameters(Weapon firingWeapon, Character firingCharacter, Vector3 launchedAt){
		this.firingWeapon = firingWeapon;
		this.firingCharacter = firingCharacter;
		launchVector = launchedAt;
	}

	public abstract void Launch();

	protected void IncreaseAge(){
		age += Time.deltaTime;
		if (age >= maxLifespan){
			DestroyProjectile();
		}
	}

	void DestroyProjectile(){
		Destroy(this.gameObject);
	}
}
