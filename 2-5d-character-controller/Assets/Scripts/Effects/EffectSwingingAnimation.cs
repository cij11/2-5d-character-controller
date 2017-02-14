using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSwingingAnimation : Effect {

	// Update is called once per frame
	void Update () {

	}

	public override void CastEffect (CharacterComponentData charCompData)
	{
		base.CastEffect (charCompData);
		print ("Swinging effect cast");
		TriggerSwingingAnimation ();
	}

	void TriggerSwingingAnimation(){
		SpriteStateController spriteStateController = componentData.GetCharacter ().GetComponentInChildren<SpriteStateController> () as SpriteStateController;
		if (spriteStateController != null) {
			spriteStateController.TriggerSwingAnimation ();
			print ("Sprite state controller found");
		}
	}
}
