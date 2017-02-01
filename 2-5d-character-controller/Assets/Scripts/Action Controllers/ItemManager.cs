using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {

	public Item defaultItem;
	public Item instantiatedDefault;
	public List<Item> inventory;
	Item equipedItem;
	int maxEquipment = 4;

	public LayerMask collisionMask;
	float pickupRadius = 2f;

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
		equipedSlotNumber = 0;
		EquipItem (instantiatedDefault);
	}

	private Item ClosestItemInReach(){
		float closestItemDistance = Mathf.Infinity;
		Collider[] hitColliders = Physics.OverlapSphere(movementActuator.transform.position, pickupRadius, collisionMask);
		Collider closestCollider = null;
		print (hitColliders.Length);
		foreach (Collider collider in hitColliders) {
			float separationDistance = (collider.transform.position - movementActuator.transform.position).magnitude;
			if (separationDistance < closestItemDistance) {
				closestCollider = collider;
				closestItemDistance = separationDistance;
			}
		}

		if (closestCollider == null) {
			return null;
		} else {
			return closestCollider.gameObject.GetComponentInParent<Item> () as Item;
		}
	}

	public void SwapCommand(bool isFireHeld){
		Item closestPickupItem = ClosestItemInReach ();
		//If fire is held, throw the weapon
		if (isFireHeld) {
			ThrowItem ();
		//Else pickup an item if one is close
		} else if (closestPickupItem != null) {
			print ("Pickup item found");
			PickupItem (closestPickupItem);
		//Else change the currently selected weapon
		} else {
			CycleWieldable ();
		}
	}

	public void PickupItem(Item item){
			print ("Picking up item");
			AddItemToInventory (item);
			StowItem ();
			EquipItem (item);
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

	public Item GetCurrentItem(){
		return equipedItem;
	}
}