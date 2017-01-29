using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour {

	public Weapon [] weapons;
	Weapon currentWeapon;
	FiringController firingController;
	Hand hand;
	int equipedSlotNumber = 0;
	
	// Use this for initialization
	void Start () {
		firingController = this.transform.parent.FindChild("ActionControllers").GetComponent<FiringController>();
		hand = this.transform.parent.GetComponentInChildren<Hand> () as Hand;
		EquipWeapon(weapons[0]);
	}

	public void CycleWieldable(){
		equipedSlotNumber += 1;
		if (equipedSlotNumber > weapons.Length - 1){
			equipedSlotNumber = 0;
		}
		EquipWeapon(weapons[equipedSlotNumber]);
	}

	public void ThrowWeapon(){
		Throwable throwable = currentWeapon.GetComponent<Throwable> () as Throwable;
		if (throwable != null) {
			throwable.Throw (this.transform.right, 5f);
			currentWeapon.CancelFiring ();
			CycleWieldable ();
		} else {
			SwapWeapon ();
		}
	}

	public void SwapWeapon(){
		DestroycurrentWeapon ();
		CycleWieldable ();
	}


	void EquipWeapon(Weapon toWeild){
		currentWeapon = Instantiate(toWeild, this.transform.position, Quaternion.identity) as Weapon;
		currentWeapon.name = toWeild.name;
		currentWeapon.transform.parent = hand.transform;
		currentWeapon.transform.rotation = hand.transform.rotation;
		RegisterWieldableWithFiringController();
	}

	void DestroycurrentWeapon(){
		if (currentWeapon != null){
			Destroy(currentWeapon.gameObject);
		}
	}
	void RegisterWieldableWithFiringController(){
		firingController.RegisterWeapon(currentWeapon);
	}
}
