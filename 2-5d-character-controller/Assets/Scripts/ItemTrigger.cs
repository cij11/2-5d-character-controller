using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{

	Weapon weapon;
	Transform weaponTransform;
	// Use this for initialization
	void Start ()
	{
		weapon = GetComponentInParent<Weapon> () as Weapon;
		weaponTransform = weapon.transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter (Collider other)
	{
		WeaponManager weaponManager = other.transform.GetComponentInChildren<WeaponManager> () as WeaponManager;
		if (weaponManager != null) {
			weaponManager.RegisterItem (weapon);
		}
	}

	void OnTriggerExit (Collider other)
	{
		WeaponManager weaponManager = other.transform.GetComponentInChildren<WeaponManager> () as WeaponManager;
		if (weaponManager != null) {
			weaponManager.ForgetItem (weapon);
		}
	}

	void OnTriggerStay (Collider other)
	{
		WeaponManager weaponManager = other.transform.GetComponentInChildren<WeaponManager> () as WeaponManager;
		if (weaponManager != null) {
			weaponManager.RegisterItem (weapon);
		}
	}

	public Transform GetWeaponTransform ()
	{
		return weaponTransform;
	
	}
}
