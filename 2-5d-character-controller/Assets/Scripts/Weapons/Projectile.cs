using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {

	protected Weapon firingWeapon;
	protected CharacterComponentData firingCharData;
	protected Vector3 worldLaunchVector;
	protected Transform characterTransform;

	protected float maxLifespan = 0.4f;

	protected int damage = 50;
	protected float knockbackSpeed = 30f;

	protected Transform spriteTransform;
	protected int direction = 1;
	float age;

	// Use this for initialization
	void Start () {
		age = 0f;
		spriteTransform = this.transform.GetChild(0);
		SetupSprite();
	}

	protected virtual void SetupSprite(){

	}

	public void LoadLaunchParameters(CharacterComponentData charData, Vector3 worldLaunch, int facingDirection){
		this.firingCharData = charData;
		characterTransform = charData.GetCharacterTransform ();
		worldLaunchVector = worldLaunch;
		direction = facingDirection;

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
