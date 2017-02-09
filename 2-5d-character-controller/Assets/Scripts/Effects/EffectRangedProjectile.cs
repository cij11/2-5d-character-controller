using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRangedProjectile : Effect {
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
		LaunchRangedProjectile ();

	}

	private void LaunchRangedProjectile(){
	//	if (componentData.IsCharacterInsantiated ()) {
			Vector3 worldAimingVector = componentData.GetAimingController ().GetAimingVectorWorldSpace ();
		GameObject newProjectile = (GameObject)Instantiate (projectile, componentData.GetItemManager().GetCurrentItem().GetBarrelTransform().position, Quaternion.identity);
			newProjectile.GetComponent<RangedProjectile> ().LoadLaunchParameters (componentData, worldAimingVector, componentData.GetAimingController().GetFacingDirection ());
			newProjectile.GetComponent<RangedProjectile> ().Launch ();
	//	}
	}
}
