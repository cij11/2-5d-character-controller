using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour {

	public Weapon [] weapons;
	Weapon currentWeapon;
	FiringController firingController;
	Hand hand;
	int equipedSlotNumber = 0;

	Vector3 gripOffset = new Vector3(0.3f, 0.05f, 0f);
	
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

	void EquipWeapon(Weapon toWeild){
		DestroycurrentWeapon();
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
