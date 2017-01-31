using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {
	public float SustainCooldown = 1f;
	protected int castedCounter = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void CastEffect(){
		castedCounter++;
		print ("Casted effect " + castedCounter.ToString () + " times.");
	}
}
