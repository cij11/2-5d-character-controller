using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {

	protected Weapon firingWeapon;
	protected CharacterComponentData componentData;
	protected Vector3 worldLaunchVector;
	protected Transform characterTransform;

	public float maxLifespan = 0.4f;

	public int damage = 50;
	protected float knockbackSpeed = 30f;

	protected Transform spriteTransform;
	protected int direction = 1;
	float age;

	public Effect[] DestroyEffects;	//Cast when the item is destroyed
	public ProjectileBehaviour[] behaviours;

	public bool destroyOnContact = false;

	// Use this for initialization
	void Start () {
		age = 0f;
		spriteTransform = this.transform.GetChild(0);
		SetupSprite();
	}

	protected virtual void SetupSprite(){

	}

	public void LoadLaunchParameters(CharacterComponentData charData, Vector3 worldLaunch, int facingDirection){
		this.componentData = charData;
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
		CastEffectsInArray (DestroyEffects);
		Destroy(this.gameObject);
	}

	protected void ApplyKnockback(CharacterCorpus knockedBackCorpus){
		knockedBackCorpus.TakeKnockback(worldLaunchVector, knockbackSpeed);
	}

	protected void ApplyDamage(CharacterCorpus damagedCharacter){
		damagedCharacter.TakeDamage(damage);
	}

	protected void CastEffectsInArray(Effect[] effectArray){
		foreach (Effect effect in effectArray) {
			effect.CastEffect (this.componentData);
		}
	}
}
