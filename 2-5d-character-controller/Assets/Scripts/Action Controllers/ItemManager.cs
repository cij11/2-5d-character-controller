using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {

	public Item defaultItem;
	public Item instantiatedDefault;
	public List<Item> inventory;
	Item equipedItem;
	int maxEquipment = 4;

	Item closestPickupItem;
	Transform closestPickupTransform;
	float closestPickupDistance = 100f;

	FiringController firingController;
	AimingController aimingController;
	CharacterMovementActuator movementActuator;
	Hand hand;
	int equipedSlotNumber = 0;
	
	// Use this for initialization
	void Start () {
		firingController = this.transform.parent.GetComponentInChildren<FiringController> () as FiringController;
		aimingController = this.transform.parent.GetComponentInChildren<AimingController>() as AimingController;
		movementActuator = GetComponentInParent<CharacterMovementActuator> () as CharacterMovementActuator;
		hand = this.transform.parent.GetComponentInChildren<Hand> () as Hand;

		instantiatedDefault = Instantiate (defaultItem, this.transform.position, Quaternion.identity) as Item;
		EquipItem (instantiatedDefault);
		inventory = new List<Item>();
		AddItemToInventory (instantiatedDefault);
	}
		
	public void EquipDefault(){
		EquipItem (instantiatedDefault);
	}

	public void SwapCommand(bool isFireHeld){
		//If fire is held, throw the weapon
		if (isFireHeld) {
			ThrowItem ();
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
			AddItemToInventory (closestPickupItem);
			closestPickupDistance = 100f;
			StowItem ();
			EquipItem (closestPickupItem);
			closestPickupItem = null;
		}
	}

	public void PickupItem(Item item){

	}

	private void AddItemToInventory(Item pickedUpItem){
		inventory.Add (pickedUpItem);
	}

	private void RemoveItemFromInventory(Item discardedItem){
		inventory.Remove (discardedItem);
	}

	public void CycleWieldable(){
		equipedSlotNumber += 1;
		if (equipedSlotNumber > inventory.Count - 1){
			equipedSlotNumber = 0;
		}
		print ("Cycling wieldable to " + equipedSlotNumber.ToString());
		StowItem ();
		EquipItem(inventory[equipedSlotNumber]);
	}

	public void ThrowItem(){
		if (equipedItem != instantiatedDefault) {
			equipedItem.ThrowItem (aimingController.GetAimingVectorWorldSpace(), 5f);
			RemoveItemFromInventory (equipedItem);
		//	currentItem.CancelInvoking ();
			EquipDefault ();
		}
	}

	public void StowItem(){
		equipedItem.gameObject.SetActive(false);
	}

	void EquipItem(Item toWeild){
		SetCurrentItem (toWeild);
		PlaceItemInHand (toWeild);
		SetupInvokable ();
	}

	void SetCurrentItem(Item toWield){
		equipedItem = toWield;
	}

	void PlaceItemInHand(Item toWield){
		equipedItem = toWield;
		equipedItem.gameObject.SetActive(true);
		equipedItem.transform.parent = hand.transform;
		equipedItem.transform.rotation = hand.transform.rotation;
		equipedItem.DrawItem ();
	}

	void SetupInvokable(){
		Invokable equipedInvokable = equipedItem.GetComponent<Invokable> () as Invokable;
		RegisterInvokableWithFiringController(equipedInvokable);
		equipedInvokable.RegisterCharacterComponentsWithInvokable ();
	}

	void DestroycurrentItem(){
		if (equipedItem != null){
			Destroy(equipedItem.gameObject);
		}
	}
	void RegisterInvokableWithFiringController(Invokable equipedInvocable){
		firingController.RegisterInvokable(equipedInvocable);
	}

	public void RegisterNearbyPickupItem(Item pickupItem){
		Vector3 vectorToPickup = movementActuator.transform.position - pickupItem.transform.position;
		float distanceToPickup = vectorToPickup.magnitude;

		if (distanceToPickup < closestPickupDistance) {
			closestPickupItem = pickupItem;
			closestPickupTransform = pickupItem.transform;
			closestPickupDistance = distanceToPickup;
		}
	}

	public void ForgetNearbyPickupItem(Item pickupItem){
		if (closestPickupItem == pickupItem) {
			closestPickupItem = null;
			closestPickupTransform = null;
			closestPickupDistance = 100f;
		}
	}

	public Item GetCurrentItem(){
		return equipedItem;
	}
}