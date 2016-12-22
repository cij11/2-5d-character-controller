using UnityEngine;
using System.Collections;

public class WieldableManager : MonoBehaviour {

	public Wieldable [] wieldables;
	Wieldable equipedWieldable;
	
	// Use this for initialization
	void Start () {
		EquipWieldable(wieldables[0]);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void EquipWieldable(Wieldable toWeild){
		if (equipedWieldable != null){
			Destroy(equipedWieldable);
		}
		equipedWieldable = Instantiate(toWeild, this.transform.position, Quaternion.identity) as Wieldable;
		equipedWieldable.name = toWeild.name;
		equipedWieldable.transform.parent = this.transform.parent;
	}
}
