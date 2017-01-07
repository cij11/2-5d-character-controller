using UnityEngine;
using System.Collections;

//Single responsibility: Weapons change state by being fired.
public abstract class Weapon : MonoBehaviour {
	
	public GameObject launchableGO;

	protected AimingController aimingController;
	protected Character character;

	protected WeaponState weaponState = WeaponState.IDLE;

	private float windupPeriod = 0f;
	private float windupTimer = 0f;
	private float firingPeriod = 0.2f;
	private float firingTimer = 0f;
	private float cooldownPeriod = 0.4f;
	private float cooldownTimer = 0f;

	// Use this for initialization
	void Start () {
		aimingController = this.transform.parent.Find("ActionControllers").GetComponent<AimingController>();
		character = this.transform.parent.GetComponent<Character>() as Character;
		LoadWeaponParameters();
	}

	protected abstract void LoadWeaponParameters();
	protected void SetCooldown(float newCooldown){
		cooldownPeriod = newCooldown;
	}
	
	//FireCommand only does anything if the weapon is not already winding up, firing, or on cooldown.
	//The weapon does not fire immediately. The FireCommand just starts the firing cycle.
	//The weapon fires during the FIRING state, after the windup.
	//The weapon is ready to fire again once the cooldown completes.
	public void FireCommand(){
		if (weaponState == WeaponState.IDLE){
			WindupWeapon();
		}
	}

	private void WindupWeapon(){
		weaponState = WeaponState.WINDINGUP;
		windupTimer = windupPeriod;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateFiringCycle();
	}

	void UpdateFiringCycle(){
		if (weaponState == WeaponState.WINDINGUP){
			UpdateWindup();
		}
		else if (weaponState == WeaponState.FIRING){
			UpdateFiring();
		}
		else if(weaponState == WeaponState.COOLINGDOWN){
			UpdateCoolingdown();
		}
	}

	void UpdateWindup(){
		windupTimer -= Time.deltaTime;
		if (windupTimer <= 0f){
			TransitionToFiring();
		}
	}

	void TransitionToFiring(){
			Fire();
			firingTimer = firingPeriod;
			weaponState = WeaponState.FIRING;
	}

	 protected abstract void Fire();

	void UpdateFiring(){
		firingTimer -= Time.deltaTime;
		if(firingTimer <= 0f){
			TransitionToCoolingDown();
		}
	}

	void TransitionToCoolingDown(){
		cooldownTimer = cooldownPeriod;
		weaponState = WeaponState.COOLINGDOWN;
	}

	void UpdateCoolingdown(){
		cooldownTimer -= Time.deltaTime;
		if(cooldownTimer <= 0f){
			weaponState = WeaponState.IDLE;
		}
	}
}
