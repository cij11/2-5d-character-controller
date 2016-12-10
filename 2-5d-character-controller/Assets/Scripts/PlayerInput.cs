using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	public RigidBodyController character;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("Horizontal") < 0){
			character.MoveHorizontalCommand(-1);
		}
		if(Input.GetAxis("Horizontal") > 0){
			character.MoveHorizontalCommand(1);
		}
		if(Input.GetAxis("Vertical") < 0){
			character.SlideCommand();
		}
		if(Input.GetAxis("Vertical") > 0){
			character.JetpackCommand();
		}
		if(Input.GetKeyDown("space")){
			character.JumpCommand(); 
		}
		if(Input.GetKey("left shift")){
			character.ParachuteCommand();
		}

		if(Input.GetKeyDown("g")){
			character.GrabBackgroundCommand();
		}
		if(Input.GetKeyUp("g")){
			character.ReleaseBackgroundCommand();
		}
	}
}
