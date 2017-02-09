using UnityEngine;
using System.Collections;

public class CharacterCorpus : MonoBehaviour {
	HeartBank heartBank;

	Rigidbody body;
	CharacterMovementActuator movementActuator;
	bool isAlive = true;

	float reelingPeriod = 0.2f;
	float reelingTimer = 0f;

	void Start () {
		body = GetComponent<Rigidbody>() as Rigidbody;
		heartBank = GetComponent<HeartBank> () as HeartBank;
		movementActuator = GetComponent<CharacterMovementActuator> () as CharacterMovementActuator;
	}

	void Update(){
		UpdateReeling ();
	}

	void Die(){
		isAlive = false;
		movementActuator.KillCommand ();
	}

	public void TakeDamage(int damage){
		reelingTimer = reelingPeriod;
		heartBank.TakeDamage (damage);
		if (heartBank.GetIsOutOfHearts ()) {
			Die ();
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

	public void TakeKnockback(Vector3 knockbackVector, float knockbackSpeed){
		body.velocity = knockbackVector * knockbackSpeed;
	}

	public bool GetIsAlive(){
		return isAlive;
	}
}