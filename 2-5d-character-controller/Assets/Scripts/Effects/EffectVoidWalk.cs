using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectVoidWalk : Effect {
	public float voidwalkSpeed = 10f;
	public float voidwalkDuration = 0.5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect (CharacterComponentData charCompData)
	{
		base.CastEffect (charCompData);
		VoidWalk ();
	}

	private void VoidWalk(){
		componentData.GetMovementActuator().RollCommand(componentData.GetAimingController().GetFacingDirection(), voidwalkSpeed, voidwalkDuration);
	}
}
