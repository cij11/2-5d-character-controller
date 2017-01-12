using UnityEngine;
using System.Collections;

public abstract class AbstractInput : MonoBehaviour{
	protected int horAxis;
	protected int vertAxis;
	protected bool jumpDown;
	protected bool jump;
	protected bool jumpUp;
	protected bool fireDown;
	protected bool fire;
	protected bool fireUp;
	protected bool swapDown;
	protected bool swap;
	protected bool swapUp;

	void Start(){
		TakeControlOfCharacter();
	}

	public void TakeControlOfCharacter(){
		this.transform.parent.FindChild("ActionControllers").gameObject.GetComponent<InputDispatcher>().SetControllingAbstractInput(this);
	}
	//To use a virtual controller, must call UpdateInput() prior To
	//getting any axis/button states.
	public abstract void UpdateInput();
	public int GetHorAxis(){
		return horAxis;
	}
	public int GetVertAxis(){
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
	public bool GetSwap(){
		return swap;
	}
	public bool GetSwapUp(){
		return swapUp;
	}
}
