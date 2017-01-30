using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {

	public Weapon defaultWeapon;
	public List<Weapon> inventory;
	Weapon currentWeapon;
	int maxEquipment = 4;

	Weapon closestPickupItem;
	Transform closestPickupTransform;
	float closestPickupDistance = 100f;
	float pickupRadius = 2f;

	FiringController firingController;
	CharacterMovementActuator movementActuator;
	Hand hand;
	int equipedSlotNumber = 0;
	
	// Use this for initialization
	void Start () {
		firingController = this.transform.parent.FindChild("ActionControllers").GetComponent<FiringController>();
		movementActuator = GetComponentInParent<CharacterMovementActuator> () as CharacterMovementActuator;
		hand = this.transform.parent.GetComponentInChildren<Hand> () as Hand;
		EquipWeapon (defaultWeapon);
	}

	void Update(){
		EvaluateClosestPickups ();
		print ("Distance to closest pickup is " + closestPickupDistance.ToString ());
	}

	void EvaluateClosestPickups(){

	}

	public void EquipDefault(){
		EquipWeapon (defaultWeapon);
	}


	public void CycleWieldable(){
		equipedSlotNumber += 1;
		if (equipedSlotNumber > inventory.Count - 1){
			equipedSlotNumber = 0;
		}
		StowWeapon ();
		EquipWeapon(inventory[equipedSlotNumber]);
	}

	public void ThrowWeapon(){
		Throwable throwable = currentWeapon.GetComponent<Throwable> () as Throwable;
		if (throwable != null) {
			throwable.Throw (this.transform.right, 5f);
			currentWeapon.CancelFiring ();
			EquipDefault ();
		} else {
			StowWeapon();
			EquipDefault();
		}
	}

	public void StowWeapon(){
		DestroycurrentWeapon ();
	}

	void EquipWeapon(Weapon toWeild){
		currentWeapon = Instantiate(toWeild, this.transform.position, Quaternion.identity) as Weapon;
		currentWeapon.name = toWeild.name;
		currentWeapon.transform.parent = hand.transform;
		currentWeapon.transform.rotation = hand.transform.rotation;
		RegisterWieldableWithFiringController();
		currentWeapon.RegisterCharacterComponentsWithWeapon ();
	}

	void DestroycurrentWeapon(){
		if (currentWeapon != null){
			Destroy(currentWeapon.gameObject);
		}
	}
	void RegisterWieldableWithFiringController(){
		firingController.RegisterWeapon(currentWeapon);
	}

	public void RegisterItem(Weapon pickupWeapon){
		Vector3 vectorToPickup = movementActuator.transform.position - pickupWeapon.transform.position;
		float distanceToPickup = vectorToPickup.magnitude;

		if (distanceToPickup < closestPickupDistance) {
			closestPickupItem = pickupWeapon;
			closestPickupTransform = pickupWeapon.transform;
			closestPickupDistance = distanceToPickup;
		}
	}

	public void ForgetItem(Weapon pickupWeapon){
		if (closestPickupItem == pickupWeapon) {
			closestPickupItem = null;
			closestPickupTransform = null;
			closestPickupDistance = 100f;
		}
	}
}
