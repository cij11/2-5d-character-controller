﻿using UnityEngine;
using System.Collections;

public class CharacterCorpus : MonoBehaviour {
	HeartBank heartBank;

	Rigidbody body;
	bool isAlive = true;

	void Start () {
		body = GetComponent<Rigidbody>() as Rigidbody;
		heartBank = GetComponent<HeartBank> () as HeartBank;
	}

	void Die(){
		//Disable input to player

		//Change to death animation
	}

	public void TakeDamage(int damage){
		heartBank.TakeDamage (damage);
		if (heartBank.GetIsOutOfHearts ()) {
			isAlive = false;
			Die ();
		}
	}

	public void TakeKnockback(Vector3 knockbackVector, float knockbackSpeed){
		body.velocity = knockbackVector * knockbackSpeed;
	}

	public bool GetIsAlive(){
		return isAlive;
	}
}