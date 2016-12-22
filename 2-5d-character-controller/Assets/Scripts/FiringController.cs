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

	void RegisterActiveWieldable(){
		GameObject wieldableGO = this.transform.parent.FindChild("Wieldable").gameObject;
		if(wieldableGO!= null){
			activeWieldable = wieldableGO.GetComponent<Wieldable>();
		}
	}

	public void InitiateFire(){

	}

	public void SustainFire(){

	}

	public void ReleaseFire(){
		if(activeWieldable == null){
			RegisterActiveWieldable();
		}
		
		if(activeWieldable != null){
			activeWieldable.Fire();
		}
	}
}
