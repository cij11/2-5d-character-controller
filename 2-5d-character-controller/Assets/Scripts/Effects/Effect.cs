﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {
	protected int castedCounter = 0;
	protected CharacterComponentData componentData;

	public AudioSource castSound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void CastEffect(CharacterComponentData charCompData){
		componentData = charCompData;
		castedCounter++;
		print ("Casted effect " + castedCounter.ToString () + " times.");

		PlayCastSound ();
	}

	void PlayCastSound(){
		if (castSound != null) {
			if (!castSound.isPlaying) {
				castSound.Play ();
			}
		}
	}
}
