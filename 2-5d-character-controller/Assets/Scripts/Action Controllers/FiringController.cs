using UnityEngine;
using System.Collections;

//Firing controller passes on button up commands, to avoid polling and lag on state changes.
public class FiringController : MonoBehaviour
{
    Invokable equipedInvokable;

	ItemManager itemManager;

	public bool cancelCausesCooldown = false;  //For invocables that have a marked sustain effect, like rapid fire weapons, grav gun, or shields.
	public float cooldownPeriod = 0f;

	float cooldownTimer = 0f;
	bool isInvoking = false;
	bool isCoolingDown = false;
	bool fireQueued = false;

	bool fireHeld = false;

	void Start(){
		itemManager = GetComponent<ItemManager> () as ItemManager;
	}

	public void RegisterInvokable(Invokable invocable)
    {
		equipedInvokable = invocable;
		Invoke("CastEquipEffects", 0.05f);
		cooldownPeriod = invocable.cooldownPeriod;
    }

	private void CastEquipEffects(){
		equipedInvokable.CastEquipEffects ();
	}

	public void SetFireHeld(bool held){
		fireHeld = held;
	}

    public void InitiateFire()
    {
		if (!itemManager.GetIsSwapping ()) { //isSwapping and isInvoking are mutually exclusive.
			if (!isInvoking && !isCoolingDown) {
				if (equipedInvokable != null) {
					equipedInvokable.StartInvoking ();
					isInvoking = true;
				}
			} else {
				fireQueued = true;
			}
		}
    }

	void Update(){
		UpdateCooldowns ();
		DischargeFireQueue ();
		if (isInvoking) {
			SustainFire ();
			CheckReleaseFire ();
		}
	}

	private void UpdateCooldowns(){
		if (cooldownTimer > 0) {
			cooldownTimer -= Time.deltaTime;
		} else {
			isCoolingDown = false;
		}
	}

	private void DischargeFireQueue(){
		if (fireQueued) {
			fireQueued = false;
			InitiateFire ();
		}
	}

	private void SustainFire(){
		equipedInvokable.SustainInvoking ();
	}

	//Invoking may be started even when fire hasn't been pressed (eg, by a queued fire press).
	//So, need to ReleaseFire/Invoking any time isInvoking is true and fire is not held.
	private void CheckReleaseFire(){
		if (isInvoking) {
			if (!fireHeld) {
				ReleaseFire ();
			}
		}
	}

    private void ReleaseFire()
    {
		isInvoking = false;
		StartCooldowns ();
		equipedInvokable.ReleaseInvoking ();
    }

	void StartCooldowns(){
		isCoolingDown = true;
		cooldownTimer = cooldownPeriod;
	}

	public void CancelFire(){
		isInvoking = false;
		if (cancelCausesCooldown) {
			StartCooldowns ();
		}
	}

	public bool GetIsInvoking(){
		return isInvoking;
	}

    public bool GetIsEncumbered()
	{
		if (isInvoking) {
			if (equipedInvokable.GetLocksMovement ()) {
				return true;
			}
		}
		return false;
	}

	public bool GetIsAimingLocked(){
		if (isInvoking && !(isCoolingDown)) { //Can't retarget if invoking, unless it's just in the cooldown phase.
			if (equipedInvokable.GetLocksAiming ()) {
				return true;
			}
		}
		return false;
	}

	public Invokable GetEquipedInvokable(){
		return equipedInvokable;
	}
}
