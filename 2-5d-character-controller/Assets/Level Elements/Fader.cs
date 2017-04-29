using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour {

	Renderer rend;
	float timer;
	public float period = 2f;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> () as Renderer;
		timer = period;
	}
	
	// Update is called once per frame
	void Update () {
		float percentage = 0;
		if (timer > 0) {
			timer -= Time.deltaTime;
			percentage = timer / period;
		} else {
			Destroy (this.gameObject);
		}
		rend.material.color = new Color (rend.material.color.r, rend.material.color.g, rend.material.color.b, percentage);

	}
}
