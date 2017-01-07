using UnityEngine;
using System.Collections;

//Firing controller passes on button up commands, to avoid polling and lag on state changes.
public class FiringController : MonoBehaviour {

	Weapon activeWeapon;

	public void RegisterWeapon(Weapon Weapon){
		activeWeapon = Weapon;
	}

	public void InitiateFire(){

	}

	public void SustainFire(){

	}

	public void ReleaseFire(){
		if(activeWeapon != null){
			activeWeapon.FireCommand();
		}
	}
}
