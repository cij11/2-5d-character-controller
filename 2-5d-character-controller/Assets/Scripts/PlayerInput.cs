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
		//	character.DownCommand();
		}
		if(Input.GetAxis("Vertical") > 0){
		//	character.UpCommand();
		}
		if(Input.GetKeyDown("space")){
			character.JumpCommand(); 
		}
	}
}
