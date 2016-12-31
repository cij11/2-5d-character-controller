using UnityEngine;
using System.Collections;

public class WieldableManager : MonoBehaviour {

	public Wieldable [] wieldables;
	Wieldable currentWieldable;
	FiringController firingController;
	int equipedSlotNumber = 0;
	
	// Use this for initialization
	void Start () {
		firingController = this.transform.parent.FindChild("ActionControllers").GetComponent<FiringController>();
		EquipWieldable(wieldables[0]);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire2")){
			CycleWieldable();
		}
	}

	void CycleWieldable(){
		equipedSlotNumber += 1;
		if (equipedSlotNumber > wieldables.Length - 1){
			equipedSlotNumber = 0;
		}
		EquipWieldable(wieldables[equipedSlotNumber]);
	}

	void EquipWieldable(Wieldable toWeild){
		DestroyCurrentWieldable();
		currentWieldable = Instantiate(toWeild, this.transform.position, Quaternion.identity) as Wieldable;
		currentWieldable.name = toWeild.name;
		currentWieldable.transform.parent = this.transform.parent;
		currentWieldable.transform.rotation = this.transform.parent.rotation;
		RegisterWieldableWithFiringController();
	}

	void DestroyCurrentWieldable(){
		if (currentWieldable != null){
			Destroy(currentWieldable.gameObject);
		}
	}
	void RegisterWieldableWithFiringController(){
		firingController.RegisterWieldable(currentWieldable);
	}
}
