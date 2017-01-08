using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {

	protected Weapon firingWeapon;
	protected Character firingCharacter;
	protected Vector3 worldLaunchVector;

	protected float maxLifespan = 1f;

	protected int damage = 4;
	protected float knockbackSpeed = 30f;
	float age;

	// Use this for initialization
	void Start () {
		age = 0f;
	}

	public void LoadLaunchParameters(Weapon firingWeapon, Character firingCharacter, Vector3 worldLaunch){
		this.firingWeapon = firingWeapon;
		this.firingCharacter = firingCharacter;
		worldLaunchVector = worldLaunch;
	}

	public abstract void Launch();

	protected void IncreaseAge(){
		age += Time.deltaTime;
		if (age >= maxLifespan){
			DestroyProjectile();
		}
	}

	protected void DestroyProjectile(){
		Destroy(this.gameObject);
	}

	protected void ApplyKnockback(CharacterCorpus knockedBackCorpus){
		knockedBackCorpus.TakeKnockback(worldLaunchVector, knockbackSpeed);
	}

	protected void ApplyDamage(CharacterCorpus damagedCharacter){
		damagedCharacter.TakeDamage(damage);
	}
}
