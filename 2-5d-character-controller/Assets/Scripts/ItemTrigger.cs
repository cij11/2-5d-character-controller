using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{

	Weapon weapon;
	Transform weaponTransform;
	SphereCollider sphereCollider;
	// Use this for initialization
	void Start ()
	{
		weapon = GetComponentInParent<Weapon> () as Weapon;
		weaponTransform = weapon.transform;
		sphereCollider = GetComponent<SphereCollider>() as SphereCollider;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerExit (Collider other)
	{
		print ("Exited trigger");
		WeaponManager weaponManager = other.transform.GetComponentInChildren<WeaponManager> () as WeaponManager;
		if (weaponManager != null) {
			weaponManager.ForgetItem (weapon);
		}
	}

	void OnTriggerStay (Collider other)
	{
		print ("Inside trigger");
		WeaponManager weaponManager = other.transform.GetComponentInChildren<WeaponManager> () as WeaponManager;
		if (weaponManager != null) {
			weaponManager.RegisterItem (weapon);
		}
	}

	public Transform GetWeaponTransform ()
	{
		return weaponTransform;
	
	}

	public void EnableSphereCollider(){
		sphereCollider.enabled = true;
	}
	public void DisableSphereCollider(){
		sphereCollider.enabled = false;
	}
}
