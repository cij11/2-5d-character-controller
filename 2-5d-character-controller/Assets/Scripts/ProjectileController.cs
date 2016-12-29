using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	float maxLifespan = 1f;
	float timeSinceCreation;

	// Use this for initialization
	void Start () {
		timeSinceCreation = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		ProcessLifespan();
	}

	void ProcessLifespan(){
		timeSinceCreation += Time.deltaTime;
		if (timeSinceCreation >= maxLifespan){
			DestroyProjectile();
		}
	}

	void DestroyProjectile(){
		Destroy(this.gameObject);
	}
}
