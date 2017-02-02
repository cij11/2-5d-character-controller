using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEffect : Effect {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect(CharacterComponentData compData){
		castedCounter++;
		print ("Start effect " + castedCounter.ToString () + " times.");
	}
}
