using UnityEngine;
using System.Collections;

public abstract class VirtualInput : MonoBehaviour{
	protected float horAxis;
	protected float vertAxis;
	protected bool jumpDown;
	protected bool jump;
	protected bool jumpUp;
	protected bool fireDown;
	protected bool fire;
	protected bool fireUp;
	protected bool swapDown;

	void Start(){
		TakeControlOfCharacter();
	}

	public void TakeControlOfCharacter(){
		this.transform.parent.FindChild("ActionControllers").gameObject.GetComponent<InputDispatcher>().SetControllingVirtualInput(this);
	}
	//To use a virtual controller, must call UpdateInput() prior To
	//getting any axis/button states.
	public abstract void UpdateInput();
	public float GetHorAxis(){
		return horAxis;
	}
	public float GetVertAxis(){
		return vertAxis;
	}
	public bool GetJumpDown(){
		return jumpDown;
	}
	public bool GetJump(){
		return jump;
	}
	public bool GetJumpUp(){
		return jumpUp;
	}
	public bool GetFireDown(){
		return fireDown;
	}
	public bool GetFireUp(){
		return fireUp;
	}
	public bool GetFire(){
		return fire;
	}
	public bool GetSwapDown(){
		return swapDown;
	}
}
