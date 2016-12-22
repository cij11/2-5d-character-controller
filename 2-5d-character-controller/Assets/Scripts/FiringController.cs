using UnityEngine;
using System.Collections;

//Firing controller passes on button up commands, to avoid polling and lag on state changes.
public class FiringController : MonoBehaviour {

	Wieldable activeWieldable;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RegisterWieldable(Wieldable wieldable){
		activeWieldable = wieldable;
	}

	public void InitiateFire(){

	}

	public void SustainFire(){

	}

	public void ReleaseFire(){
		if(activeWieldable != null){
			activeWieldable.Fire();
		}
	}
}
