using UnityEngine;
using System.Collections;

public class AIVirtualController : MonoBehaviour {

	int horAxis;
	int vertAxis;
	bool jump;
	bool fire;
	bool swap;

	bool jumpTapped;
	bool fireTapped;
	bool swapTapped;
	// Use this for initialization
	void Start () {
		horAxis = 0;
		vertAxis = 0;
		jump = false;
		fire = false;
		swap = false;
		jumpTapped = false;
		fireTapped = false;
		jumpTapped = false;
	}


	public void PushHorAxis(int direction){
		horAxis = direction;
	}
	public void PushVertAxis(int direction){
		vertAxis = direction;
	}

	public void PushJump(){
		jump = true;
	}
	public void ReleaseJump(){
		jump = false;
	}
	public void PushFire(){
		fire = true;
	}
	public void ReleaseFire(){
		fire = false;
	}
	public void PushSwap(){
		swap = true;
	}
	public void ReleaseSwap(){
		swap = false;
	}

	//Tapping a button triggers a button down and a button up event this frame, and 
	//leaves the button in a button up state.
	public void TapJump(){
		jumpTapped = true;
		jump = false;
	}
	public void TapFire(){
		fireTapped = true;
		fire = false;
	}
	public void TapSwap(){
		swapTapped = true;
		swap = false;
	}

	public int GetHorAxis(){
		return horAxis;
	}
	public int GetVertAxis(){
		return vertAxis;
	}
	public bool GetJump(){
		return jump;
	}
	public bool GetFire(){
		return fire;
	}
	public bool GetSwap(){
		return swap;
	}

	public bool ExtractJumpTap(){
		if(jumpTapped){
			jumpTapped = false;
			return true;
		}
		else return false;
	}

	public bool ExtractFireTap(){
		if(fireTapped){
			fireTapped = false;
			return true;
		}
		else return false;
	}

	public bool ExtractSwapTap(){
		if(swapTapped){
			swapTapped = false;
			return true;
		}
		else return false;
	}
}
