using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {

	protected Weapon firingWeapon;
	protected CharacterComponentData componentData;
	protected Invokable firingInvokable;
	protected Vector3 worldLaunchVector;
	protected Transform characterTransform;

	public float maxLifespan = 0.4f;

	public int damage = 50;
	protected float knockbackSpeed = 10f;

	protected Transform spriteTransform;
	protected int direction = 1;
	float age;

	public Effect[] DestroyEffects;	//Cast when the item is destroyed
	public ProjectileBehaviour[] behaviours;

	protected int firingTeam;

	public bool destroyOnTerrainContact = true;
	public bool destroyOnCharacterContact = true;
	public bool destroyOnWeaponChange = false;
	public bool boundToReticule = false;
	public bool boundToFeet = false;
	public bool hasLifespan = true;
	public bool bounceCasterOnHit = false;
	public bool deflectsProjectiles = false;
	public bool destroyOnCharDeath = false;

	// Use this for initialization
	void Start () {
		age = 0f;
		spriteTransform = this.transform.GetChild(0);
		SetupSprite();
	}

	void Update(){
		if (hasLifespan) {
			IncreaseAge ();
		}
		RunProjectileBehaviours ();
		if (boundToReticule) {
			this.transform.position = componentData.GetAimingController ().GetAimingVectorWorldSpace () + componentData.GetCharacterTransform ().position;
		}

		if (boundToFeet) {
			this.transform.position = componentData.GetCharacterTransform ().position - componentData.GetCharacterTransform ().up * 0.5f;
		}

		if (destroyOnWeaponChange) {
			CheckWeaponChangeAndDestroy ();
		}

		if (destroyOnCharDeath) {
			CheckCharDeathAndDestroy ();
		}
	}

	void RunProjectileBehaviours(){
		foreach (ProjectileBehaviour behaviour in behaviours) {
			behaviour.PerformBehaviour (componentData);
		}
	}

	private void CheckWeaponChangeAndDestroy(){
		if (componentData.GetCharacter () == null) {
			DestroyProjectile ();
		}
		else if (componentData.GetFiringController().GetEquipedInvokable() != this.firingInvokable) {
			DestroyProjectile ();
		}
	}

	protected virtual void SetupSprite(){

	}

	public void LoadLaunchParameters(CharacterComponentData charData, Vector3 worldLaunch, int facingDirection){
		this.componentData = charData;
		firingInvokable = componentData.GetFiringController ().GetEquipedInvokable ();
		characterTransform = charData.GetCharacterTransform ();
		worldLaunchVector = worldLaunch;
		direction = facingDirection;

		firingTeam = charData.GetCharacterCorpus ().GetTeam ();

	}

	public abstract void Launch();

	protected void IncreaseAge(){
		age += Time.deltaTime;
		if (age >= maxLifespan) {
			DestroyProjectile ();
		}
	}

	protected void DestroyProjectile(){
		CastEffectsInArray (DestroyEffects);
		Destroy(this.gameObject);
	}
		
	protected void CastEffectsInArray(Effect[] effectArray){
		foreach (Effect effect in effectArray) {
			effect.CastEffect (this.componentData);
		}
	}

	protected void ResolveCollision(GameObject other){
		CharacterCorpus corpus = other.GetComponent<CharacterCorpus> () as CharacterCorpus;
		Character character = other.GetComponent<Character> () as Character;
		if (corpus != null){
			if (character != null) {
				if (character != this.componentData.GetCharacter ()) {


					if (bounceCasterOnHit) {
						CharacterMovementActuator castingAcuator = componentData.GetMovementActuator ();
						if (castingAcuator != null) {
							if (castingAcuator.GetVerticalSpeed () < -1f) {
								if (castingAcuator.transform.position.y > corpus.transform.position.y) {
									castingAcuator.BounceCommand (8f);
									corpus.TakeDamage (damage, firingTeam);
									corpus.TakeKnockback (worldLaunchVector, knockbackSpeed);
								}
							}
						}
					} else {
						corpus.TakeDamage (damage, firingTeam);
						corpus.TakeKnockback (worldLaunchVector, knockbackSpeed);
					}

					if (destroyOnCharacterContact) {
						DestroyProjectile ();
					}
				}
			}
		}
		if (other.gameObject.layer == 8) { //If other is terrain
			if (destroyOnTerrainContact) {
				DestroyProjectile ();
			}
		}

		if (deflectsProjectiles) {
			if (other.gameObject.layer == 13) { //If other is a projectile(ranged)
				Rigidbody body = other.GetComponent<Rigidbody>() as Rigidbody;
				if (body != null) {
					Transform castingTransform = componentData.GetCharacterTransform ();
					if (castingTransform != null) {
						Vector3 toOtherProj = other.transform.position - castingTransform.position;
						toOtherProj.Normalize ();
						float otherProjSpeed = body.velocity.magnitude;

						body.velocity = otherProjSpeed * toOtherProj;
					}
				}
			}
		}
	}

	protected void CheckCharDeathAndDestroy(){
		CharacterCorpus casterCorpus = componentData.GetCharacterCorpus ();
		if (casterCorpus != null) {
			if(!casterCorpus.GetIsAlive())
				Destroy (this.gameObject);
		}
	}

	public void SetWorldLaunchVector(Vector3 newVec){
		worldLaunchVector = newVec;
	}
}
