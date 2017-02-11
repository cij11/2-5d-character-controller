using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRangedProjectile : Effect {
	public GameObject projectile;
	public int numberOfProjectiles = 1;
	public float spreadAngle = 20f;
	public float sprayAngle = 0f;

	private float startingAngle;
	private float separationAngle;
	// Use this for initialization
	void Start () {
		if (numberOfProjectiles == 1) {
			startingAngle = 0f;
			separationAngle = 0f;
		} else {
			startingAngle = -spreadAngle / 2f;
			separationAngle = spreadAngle / (numberOfProjectiles - 1);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect (CharacterComponentData importComponentData)
	{
		base.CastEffect (importComponentData);
		LaunchRangedProjectile ();

	}

	private void LaunchRangedProjectile(){
		for (int i = 0; i < numberOfProjectiles; i++) {
			float divergenceAngle = startingAngle + i * separationAngle;

			//	if (componentData.IsCharacterInsantiated ()) {
			Vector3 worldAimingVector = componentData.GetAimingController ().GetAimingVectorWorldSpace ();

			worldAimingVector = Quaternion.Euler (0f, 0f, divergenceAngle) * worldAimingVector;
			GameObject newProjectile = (GameObject)Instantiate (projectile, componentData.GetItemManager ().GetCurrentItem ().GetBarrelTransform ().position, Quaternion.identity);
			newProjectile.GetComponent<RangedProjectile> ().LoadLaunchParameters (componentData, worldAimingVector, componentData.GetAimingController ().GetFacingDirection ());
			newProjectile.GetComponent<RangedProjectile> ().Launch ();
		}
	//	}
	}
}
