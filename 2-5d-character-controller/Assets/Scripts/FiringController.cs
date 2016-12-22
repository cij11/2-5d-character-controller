using UnityEngine;
using System.Collections;

//Firing controller passes on button up commands, to avoid polling and lag on state changes.
public class FiringController : MonoBehaviour {

	WeaponController activeWeapon;
	// Use this for initialization
	void Start () {
		RegisterActiveWeapon();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void RegisterActiveWeapon(){
		GameObject weaponGO = this.transform.parent.FindChild("Weapon").gameObject;
		activeWeapon = weaponGO.GetComponent<WeaponController>();
	}

	public void InitiateFire(){

	}

	public void SustainFire(){

	}

	public void ReleaseFire(){
		activeWeapon.Fire();
	}
}
