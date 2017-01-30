using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {

	public Weapon defaultWeapon;
	public Weapon instantiatedDefault;
	public List<Weapon> inventory;
	Weapon currentWeapon;
	int maxEquipment = 4;

	Weapon closestPickupItem;
	Transform closestPickupTransform;
	float closestPickupDistance = 100f;

	FiringController firingController;
	CharacterMovementActuator movementActuator;
	Hand hand;
	int equipedSlotNumber = 0;
	
	// Use this for initialization
	void Start () {
		firingController = this.transform.parent.FindChild("ActionControllers").GetComponent<FiringController>();
		movementActuator = GetComponentInParent<CharacterMovementActuator> () as CharacterMovementActuator;
		hand = this.transform.parent.GetComponentInChildren<Hand> () as Hand;

		instantiatedDefault = Instantiate (defaultWeapon, this.transform.position, Quaternion.identity);
		DrawWeapon (instantiatedDefault);
		inventory = new List<Weapon>();
		AddWeaponToInventory (instantiatedDefault);
	}
		
	public void EquipDefault(){
		DrawWeapon (instantiatedDefault);
	}

	public void SwapCommand(bool isFireHeld){
		//If fire is held, throw the weapon
		if (isFireHeld) {
			ThrowWeapon ();
		//Else pickup an item if one is close
		} else if (closestPickupItem != null) {
			PickupClosestItem ();
		//Else change the currently selected weapon
		} else {
			CycleWieldable ();
		}
	}

	public void PickupClosestItem(){
		if (closestPickupItem != null) {
			print ("Picking up item");
			Item item = closestPickupItem.GetComponent<Item> () as Item;
	
			AddWeaponToInventory (item.GetComponent<Weapon> () as Weapon);
			closestPickupDistance = 100f;
			StowWeapon ();
			DrawWeapon (closestPickupItem);
			closestPickupItem = null;
		}
	}

	private void AddWeaponToInventory(Weapon pickedUpWeapon){
		inventory.Add (pickedUpWeapon);
	}

	private void RemoveWeaponFromInventory(Weapon discardedWeapon){
		inventory.Remove (discardedWeapon);
	}

	public void CycleWieldable(){
		equipedSlotNumber += 1;
		if (equipedSlotNumber > inventory.Count - 1){
			equipedSlotNumber = 0;
		}
		print ("Cycling wieldable to " + equipedSlotNumber.ToString());
		StowWeapon ();
		DrawWeapon(inventory[equipedSlotNumber]);
	}

	public void ThrowWeapon(){
		Item throwableItem = currentWeapon.GetComponent<Item> () as Item;
		if (throwableItem != null) {
			throwableItem.Throw (this.transform.right, 5f);
			RemoveWeaponFromInventory(currentWeapon);
			currentWeapon.CancelFiring ();
			EquipDefault ();
		} else {
			StowWeapon();
			EquipDefault();
		}
	}

	public void StowWeapon(){
		currentWeapon.gameObject.SetActive(false);
	}

	void DrawWeapon(Weapon toWeild){
		currentWeapon = toWeild;
		currentWeapon.gameObject.SetActive(true);
		currentWeapon.name = toWeild.name;
		currentWeapon.transform.parent = hand.transform;
		currentWeapon.transform.rotation = hand.transform.rotation;
		RegisterWieldableWithFiringController();
		currentWeapon.RegisterCharacterComponentsWithWeapon ();

		Item weaponItem = currentWeapon.GetComponent<Item> () as Item;
		if (weaponItem != null) {
			weaponItem.PickUp ();
		}
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

	public Weapon GetCurrentWeapon(){
		return currentWeapon;
	}
}
