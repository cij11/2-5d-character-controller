using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {

	public Item defaultItemPrefab;
	Item instantiatedDefault;
	public List<Item> inventory;
	Item equipedItem;
	int maxEquipment = 9;

	public LayerMask collisionMask;
	float pickupRadius = 2f;

	bool isSwapping = false; //Set true by pressing swap, returned to false by picking up, throwing, or changing item.

	FiringController firingController;
	AimingController aimingController;
	CharacterMovementActuator movementActuator;
	Character character;
	Hand hand;
	int equipedSlotNumber = 0;

	int highlightedItem = 0;

	ItemWheel itemWheel;

	private float throwMuzzleSpeed = 20f;

	CharacterComponentData componentData;
	
	// Use this for initialization
	void Start () {
		firingController = this.transform.parent.GetComponentInChildren<FiringController> () as FiringController;
		aimingController = this.transform.parent.GetComponentInChildren<AimingController>() as AimingController;
		movementActuator = GetComponentInParent<CharacterMovementActuator> () as CharacterMovementActuator;
		character = GetComponentInParent<Character> () as Character;
		hand = this.transform.parent.GetComponentInChildren<Hand> () as Hand;

		componentData = new CharacterComponentData (character);

		itemWheel = GetComponentInChildren <ItemWheel> () as ItemWheel;

		instantiatedDefault = Instantiate (defaultItemPrefab, this.transform.position, Quaternion.identity) as Item;
		instantiatedDefault.SetItemStateHeld ();
		EquipItem (instantiatedDefault);
		inventory = new List<Item>();
		AddItemToInventory (instantiatedDefault);
	}

	void Update(){
		if (isSwapping) {
			highlightedItem = HighlightFromAimingController (); //Select whichever octant (or neutral) the input is directed towards.
			if (highlightedItem == 0 && equipedItem == instantiatedDefault) {		//Unless the input is neutral, and the default weapon is alread held.
				if (inventory.Count > 1)
					highlightedItem = 1;						//In that case, toggle to the first weapon in the inventory
			}
			itemWheel.HighlightIcon (highlightedItem);
		}
	}

	private int HighlightFromAimingController(){
		int highlighted = 0;
		MovementDirection inputDirection = aimingController.GetInputDirection ();
		switch (inputDirection) {
		case MovementDirection.NEUTRAL:
			highlighted = 0;
			break;
		case MovementDirection.UP:
			highlighted = 1;
			break;
		case MovementDirection.NE:
			highlighted = 2;
			break;
		case MovementDirection.RIGHT:
			highlighted = 3;
			break;
		case MovementDirection.SE:
			highlighted = 4;
			break;
		case MovementDirection.DOWN:
			highlighted = 5;
			break;
		case MovementDirection.SW:
			highlighted = 6;
			break;
		case MovementDirection.LEFT:
			highlighted = 7;
			break;
		case MovementDirection.NW:
			highlighted = 8;
			break;
		default:
			highlighted = 0;
			break;
		}
		return highlighted;
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

	//Start swapping process.
	//Swapping is completed by:
	//Being over an item when swap is pressed (picking up the item)
	//Pressing fire while swap is pressed (throwing the item)
	//Releasing swap (swapping to the next item, or the appropriate item from the inventory wheel once implemented.
	public void StartSwap(){
		if (!firingController.GetIsInvoking ()) { //If fire already held, disable swapping
			Item closestPickupItem = ClosestItemInReach ();
			if (closestPickupItem != null) {
				print ("Pickup item found");
				PickupItem (closestPickupItem);
			} else {
				isSwapping = true;
				itemWheel.LoadSpritesIntoIcons (inventory);
				itemWheel.ShowItemWheel();
			}
		}
	}

	//Fire has been pressed. If swap is held, throw the item
	public void ThrowItemIfSwapping(){
		if (isSwapping) {
			ThrowItem ();
			isSwapping = false;
			itemWheel.HideItemWheel ();
		}
	}

	//Swap has been released. Swap items, if an item hasn't already been picked up or thrown
	public void DischargeSwap(){
		if (isSwapping) {
			EquipWieldableFromInventory (highlightedItem);
			isSwapping = false;
			itemWheel.HideItemWheel ();
		}
	}

	private void EquipWieldableFromInventory(int slotNumber){
		if (slotNumber > inventory.Count - 1) {
			//Fail
		} else {
			equipedSlotNumber = slotNumber;
			StowItem ();
			EquipItem (inventory [slotNumber]);
		}
	}

	public void PickupItem(Item item){
		if (inventory.Count < maxEquipment) {
			AddItemToInventory (item);
			StowItem ();
			EquipItem (item);
		}
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
		StowItem ();
		EquipItem(inventory[equipedSlotNumber]);
	//	itemWheel.HighlightIcon (equipedSlotNumber);
	}

	public void ThrowItem(){
		if (equipedItem != instantiatedDefault) {
			equipedItem.ThrowItem (aimingController.GetAimingVectorWorldSpace() + movementActuator.transform.up * 0.1f, throwMuzzleSpeed, character);
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
		equipedItem.gameObject.SetActive(true);
		equipedItem.transform.parent = hand.transform;
		equipedItem.transform.rotation = hand.transform.rotation;
		hand.OrientHeldItem (); //Call this now, to avoid a one frame delay until the hand is updated next.
		equipedItem.DrawItem ();
	}

	void SetupInvokable(){
		Invokable equipedInvokable = equipedItem.GetComponent<Invokable> () as Invokable;
		RegisterInvokableWithFiringController(equipedInvokable);
		equipedInvokable.RegisterCharacterComponentsWithInvokable (componentData);
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

	public bool GetIsSwapping(){
		return isSwapping;
	}
}