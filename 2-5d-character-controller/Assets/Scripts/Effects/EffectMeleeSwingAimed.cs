using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMeleeSwingAimed : Effect {
	public GameObject projectile;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect (CharacterComponentData charCompData)
	{
		base.CastEffect (charCompData);
		LaunchMeleeProjectile ();

	}

	private void LaunchMeleeProjectile()
	{
		Vector3 worldAimingVector = componentData.GetAimingController ().GetAimingVectorWorldSpace ();
		GameObject newProjectile = (GameObject)Instantiate (projectile, componentData.GetCharacterTransform ().position + worldAimingVector, Quaternion.identity);
		newProjectile.GetComponent<MeleeProjectile> ().LoadLaunchParameters (componentData.GetCharacter(), worldAimingVector, componentData.GetAimingController().GetFacingDirection ());
		newProjectile.GetComponent<MeleeProjectile> ().Launch ();
	}
}
