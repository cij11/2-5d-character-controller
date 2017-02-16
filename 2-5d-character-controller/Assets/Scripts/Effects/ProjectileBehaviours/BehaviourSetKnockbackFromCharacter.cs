using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourSetKnockbackFromCharacter : ProjectileBehaviour {

	public override void PerformBehaviour (CharacterComponentData compData)
	{
		base.PerformBehaviour (compData);
		projectile.SetWorldLaunchVector ((this.transform.position - compData.GetCharacterTransform ().position).normalized);
	}
}
