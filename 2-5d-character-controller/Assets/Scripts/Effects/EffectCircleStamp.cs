using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCircleStamp : Effect {
	public float stampSize = 3f;
	public bool isSolid = false;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect (CharacterComponentData charCompData)
	{
		base.CastEffect (charCompData);
		CircleStamp ();
	}

	void CircleStamp(){
		Topography topography = (Topography)FindObjectOfType (typeof(Topography));
		topography.DigCircle (this.transform.position, stampSize, isSolid);
	}
}
