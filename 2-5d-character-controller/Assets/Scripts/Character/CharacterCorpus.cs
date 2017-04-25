using UnityEngine;
using System.Collections;

public class CharacterCorpus : MonoBehaviour {
	HeartBank heartBank;

	Rigidbody body;
	CharacterMovementActuator movementActuator;
	bool isAlive = true;

	public int team = 2;

	float reelingPeriod = 0.2f;
	float reelingTimer = 0f;

	public AudioClip hitClip;

	void Start () {
		body = GetComponent<Rigidbody>() as Rigidbody;
		heartBank = GetComponent<HeartBank> () as HeartBank;
		movementActuator = GetComponent<CharacterMovementActuator> () as CharacterMovementActuator;
	}

	void Update(){
		UpdateReeling ();
		UpdateFallOffWorldDeath ();
	}

	void Die(){
		isAlive = false;
		movementActuator.KillCommand ();
	}

	public void TakeDamage(int damage, int projectileTeam){
		if (projectileTeam != team) {
			reelingTimer = reelingPeriod;
			heartBank.TakeDamage (damage);
			PlayImpactSound ();
			if (heartBank.GetIsOutOfHearts ()) {
				Die ();
			}
		}
	}

	private void UpdateReeling(){
		if (reelingTimer > 0) {
			reelingTimer -= Time.deltaTime;
		}
	}

	public bool GetIsReeling(){
		if (reelingTimer > 0) {
			return true;
		}
		return false;
	}

	public void RestoreHeart(){
		heartBank.RestoreHeart ();
	}

	private void PlayImpactSound(){
		if (hitClip != null) {
			SoundEffectPlayer.instance.PlaySoundClip (hitClip);
		}
	}

	public void TakeKnockback(Vector3 knockbackVector, float knockbackSpeed){
		body.velocity = knockbackVector * knockbackSpeed;
	}

	public bool GetIsAlive(){
		return isAlive;
	}

	public void UpdateFallOffWorldDeath(){
		if (this.transform.position.y < -300) {
			Die ();
		}
	}

	public int GetTeam(){
		return team;
	}
}