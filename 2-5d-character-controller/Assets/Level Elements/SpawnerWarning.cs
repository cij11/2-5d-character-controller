using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerWarning : MonoBehaviour {
	float maxSize = 2.5f;
	float minSize = 0.0f;

	float lifespan = 1.5f;
	float age = 0.0f;

	Transform warningTransform;

	// Use this for initialization
	void Start () {
		warningTransform = this.transform;
		Invoke("DestroyWarning", lifespan);
	}
	
	// Update is called once per frame
	void Update () {
		age += Time.deltaTime;
		float percentDone = age / lifespan;
		float size = (percentDone * maxSize);

		if (age > 1f) {
			transform.localScale = new Vector3 (1 - percentDone, size, 1);
		} else {
			transform.localScale = new Vector3 (1, size, 1);
		}
	}

	void DestroyWarning(){
		Destroy (this.gameObject);
	}
}
