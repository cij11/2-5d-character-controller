﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseEffect : Effect {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect(){
		castedCounter++;
		print ("Release effect " + castedCounter.ToString () + " times.");
	}
}