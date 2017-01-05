using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour {

	public Weapon [] weapons;
	Weapon currentWeapon;
	FiringController firingController;
	int equipedSlotNumber = 0;
	
	// Use this for initialization
	void Start () {
		firingController = this.transform.parent.FindChild("ActionControllers").GetComponent<FiringController>();
		EquipWeapon(weapons[0]);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire2")){
			CycleWieldable();
		}
	}

	void CycleWieldable(){
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
		currentWeapon.transform.parent = this.transform.parent;
		currentWeapon.transform.rotation = this.transform.parent.rotation;
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
