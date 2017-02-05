using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroyInvokable : Effect {
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
		DestroyInvokable ();
	}

	private void DestroyInvokable(){
		componentData.GetItemManager ().ThrowItem ();
		Destroy (this.gameObject);
	}
}
