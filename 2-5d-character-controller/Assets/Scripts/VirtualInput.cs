using UnityEngine;
using System.Collections;

public abstract class VirtualInput {
	protected float horAxis;
	protected float vertAxis;
	protected bool jumpDown;
	protected bool jump;
	protected bool jumpUp;
	protected bool fireDown;
	protected bool fire;
	protected bool fireUp;
	protected bool swapDown;

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
