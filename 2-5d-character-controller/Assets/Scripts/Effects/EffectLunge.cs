using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLunge : Effect {
	public float lungeSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect (CharacterComponentData charCompData)
	{
		base.CastEffect (charCompData);
		Lunge ();
	}

	private void Lunge(){
		componentData.GetMovementController ().Lunge (componentData.GetAimingController ().GetAimingVectorWorldSpace (), lungeSpeed);
	}
}
