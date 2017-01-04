using UnityEngine;
using System.Collections;

public abstract class Wieldable : MonoBehaviour {
	
	protected AimingController aimingController;
	protected Character character;

	// Use this for initialization
	void Start () {
		aimingController = this.transform.parent.Find("ActionControllers").GetComponent<AimingController>();
		character = this.transform.parent.GetComponent<Character>() as Character;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public abstract void Fire();
}
