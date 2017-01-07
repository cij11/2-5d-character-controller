using UnityEngine;
using System.Collections;

public class AIVirtualController : MonoBehaviour {

	float horAxis;
	float vertAxis;
	bool jump;
	bool fire;
	bool swap;
	// Use this for initialization
	void Start () {
		horAxis = 0f;
		vertAxis = 0f;
		jump = false;
		fire = false;
		swap = false;
	}


	public void PushHorizAxis(float direction){
		horAxis = direction;
	}
	public void PushVertAxis(float direction){
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
	public float GetHorAxis(){
		return horAxis;
	}
	public float GetVertAxis(){
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
}
