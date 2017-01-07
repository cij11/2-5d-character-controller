using UnityEngine;
using System.Collections;

public class CharacterCorpus : MonoBehaviour {

	int startingHealth = 100;
	int health;

	Rigidbody body;

	void Start () {
		health = startingHealth;
		body = GetComponent<Rigidbody>() as Rigidbody;
	}

	public void TakeDamage(int damage){
		health -= damage;
		if (health <= 0){
			Destroy(this.gameObject);
		}
	}

	public void TakeKnockback(Vector3 knockbackVector, float knockbackSpeed){
		body.velocity = knockbackVector * knockbackSpeed;
	}
}