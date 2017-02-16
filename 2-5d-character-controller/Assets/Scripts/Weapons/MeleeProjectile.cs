using UnityEngine;
using System.Collections;

public class MeleeProjectile : Projectile {
	
	float reachRange = 0.7f;
	// Update is called once per frame
	void Update () {
		IncreaseAge();
	}


	protected override void SetupSprite(){
		BoxCollider boxCollider = GetComponent<BoxCollider>() as BoxCollider;
		spriteTransform.localScale = boxCollider.size; 
	}
	public override void Launch(){
		LaunchMelee();
	}

	void LaunchMelee(){
		this.transform.SetParent(characterTransform);
		this.transform.position = this.transform.parent.position +  worldLaunchVector * reachRange;
		AlignToLauncher();
	}

	void AlignToLauncher(){
		Vector3 weaponToProjectile = this.transform.position - characterTransform.position;
		this.transform.rotation = Quaternion.FromToRotation(this.transform.right, weaponToProjectile) * transform.rotation;
	}

	void OnTriggerEnter(Collider other) {
		ResolveCollision (other.gameObject);
    }
}
