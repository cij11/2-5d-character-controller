using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {

	protected GameObject firingWeapon;
	protected Vector3 launchVector;
	protected float maxLifespan = 1f;
	float age;

	// Use this for initialization
	void Start () {
		age = 0f;
	}

	public void LoadLaunchParameters(GameObject firedBy, Vector3 launchedAt){
		firingWeapon = firedBy;
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
