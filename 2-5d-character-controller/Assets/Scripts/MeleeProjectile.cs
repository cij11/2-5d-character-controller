using UnityEngine;
using System.Collections;

public class MeleeProjectile : Projectile {
	
	float reachRange = 2f;
	// Update is called once per frame
	void Update () {
		IncreaseAge();
	}

	public override void Launch(){
		LaunchMelee();
	}

	void LaunchMelee(){
		this.transform.SetParent(firingCharacter.transform);
		this.transform.localPosition = launchVector * reachRange;
	}

}
