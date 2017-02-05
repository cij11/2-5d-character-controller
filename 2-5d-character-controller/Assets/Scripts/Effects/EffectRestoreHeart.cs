﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRestoreHeart : Effect {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect (CharacterComponentData charCompData)
	{
		base.CastEffect (charCompData);
		RestoreHeart ();
	}

	private void RestoreHeart(){
		componentData.GetCharacterCorpus ().RestoreHeart ();
	}
}
