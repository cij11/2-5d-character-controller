using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

	public RigidBodyController character;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void MoveHorizontal(float direction){
		character.MoveHorizontalCommand(direction);
	}
	public void MoveVertical(float direction){
		if(direction > 0){
			character.JetpackCommand();
		}
		if(direction < 0){
			character.SlideCommand();
		}
	}
	public void Grab(){

	}

	public void Jump(){
		character.JumpCommand();
	}

}
