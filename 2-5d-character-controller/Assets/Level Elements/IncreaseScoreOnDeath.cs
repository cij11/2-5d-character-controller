using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseScoreOnDeath : MonoBehaviour {

	ScoreCounter scoreCounter;
	// Use this for initialization
	void Start () {
		scoreCounter = FindObjectOfType<ScoreCounter> ();
	}
	
	void OnDestroy(){
		if (scoreCounter != null) {
			scoreCounter.IncrementKills ();
		}
	}
}
