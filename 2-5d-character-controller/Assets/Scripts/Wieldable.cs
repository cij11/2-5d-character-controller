using UnityEngine;
using System.Collections;

public abstract class Wieldable : MonoBehaviour {
	
	protected AimingController aimingController;

	// Use this for initialization
	void Start () {
		aimingController = this.transform.parent.Find("ActionControllers").GetComponent<AimingController>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public abstract void Fire();
}
