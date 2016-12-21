using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

	public GameObject projectileGO;

	AimingController aimingController;

	float muzzleSpeed = 20f;

	// Use this for initialization
	void Start () {
		aimingController = this.transform.parent.Find("Aiming").GetComponent<AimingController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonUp("Fire1")){
			Vector3 aimingVector = aimingController.GetAimingVector();
			GameObject newProjectile = (GameObject)Instantiate(projectileGO, this.transform.position + aimingVector, Quaternion.identity);
			newProjectile.GetComponent<Rigidbody>().velocity = aimingVector  * muzzleSpeed;
		}
	}
}
