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
	protected float knockbackSpeed = 30f;

	protected Transform spriteTransform;
	protected int direction = 1;
	float age;

	public Effect[] DestroyEffects;	//Cast when the item is destroyed
	public ProjectileBehaviour[] behaviours;

	public bool destroyOnTerrainContact = true;
	public bool destroyOnCharacterContact = true;
	public bool destroyOnWeaponChange = false;
	public bool boundToReticule = false;
	public bool boundToFeet = false;
	public bool hasLifespan = true;

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

		if (destroyOnWeaponChange) {
			CheckWeaponChangeAndDestroy ();
		}
	}

	void RunProjectileBehaviours(){
		foreach (ProjectileBehaviour behaviour in behaviours) {
			behaviour.PerformBehaviour (componentData);
		}
	}

	private void CheckWeaponChangeAndDestroy(){
		if (componentData.GetFiringController().GetEquipedInvokable() != this.firingInvokable) {
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
					corpus.TakeDamage (damage);
					corpus.TakeKnockback (worldLaunchVector, knockbackSpeed);
					if (destroyOnCharacterContact) {
						DestroyProjectile ();
					}
				}
			}
		}
		if (other.gameObject.layer == 8) {
			if (destroyOnTerrainContact) {
				DestroyProjectile ();
			}
		}
	}

	public void SetWorldLaunchVector(Vector3 newVec){
		worldLaunchVector = newVec;
	}
}
