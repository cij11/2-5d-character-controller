using UnityEngine;
using System.Collections;

//Firing controller passes on button up commands, to avoid polling and lag on state changes.
public class FiringController : MonoBehaviour
{
    Invokable equipedInvokable;

	public bool cancelCausesCooldown = false;  //For invocables that have a marked sustain effect, like rapid fire weapons, grav gun, or shields.
	public float cooldownPeriod = 0f;

	float cooldownTimer = 0f;
	bool isInvoking = false;
	bool isCoolingDown = false;
	bool fireQueued = false;

	bool fireHeld = false;

	CharacterComponentData characterComponentData;

	void Start(){
		Character character = GetComponentInParent<Character>() as Character;
		characterComponentData = new CharacterComponentData (character);
	}

	public void RegisterInvokable(Invokable invocable)
    {
		equipedInvokable = invocable;
		cooldownPeriod = invocable.cooldownPeriod;
		invocable.RegisterCharacterComponentsWithInvokable (characterComponentData);
    }

	public void SetFireHeld(bool held){
		fireHeld = held;
	}

    public void InitiateFire()
    {
		if (!isInvoking && !isCoolingDown) {
			if (equipedInvokable != null) {
				equipedInvokable.StartInvoking ();
				isInvoking = true;
			}
		} else {
			fireQueued = true;
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
		if (equipedInvokable != null)
        {
			return equipedInvokable.GetIsEncumbered();
        }
        return false;
    }
}
