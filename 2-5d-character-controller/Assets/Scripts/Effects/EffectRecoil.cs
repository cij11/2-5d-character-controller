using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRecoil : Effect {
	public float recoilSpeed;
	public float maxRecoilSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect (CharacterComponentData charCompData)
	{
		base.CastEffect (charCompData);
		Recoil ();
	}

	private void Recoil(){
		componentData.GetMovementActuator ().RecoilCommand (-componentData.GetAimingController ().GetAimingVectorWorldSpace (), recoilSpeed, maxRecoilSpeed);
	}
}
