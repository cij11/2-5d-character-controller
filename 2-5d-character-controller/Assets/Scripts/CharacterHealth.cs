using UnityEngine;
using System.Collections;

public class CharacterHealth : MonoBehaviour {

	int startingHealth = 100;
	int health;

	// Use this for initialization
	void Start () {
		health = startingHealth;
	}

	public void TakeDamage(int damage){
		health -= damage;
		if (health <= 0){
			Destroy(this.gameObject);
		}
	}
}
