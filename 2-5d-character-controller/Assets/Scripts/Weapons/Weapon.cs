using UnityEngine;
using System.Collections;

//Single responsibility: Weapons change state by being fired.
public abstract class Weapon : MonoBehaviour {
	
	public GameObject launchableGO;

	protected AimingController aimingController;
	protected Character character;

	protected WeaponState weaponState = WeaponState.IDLE;

	private float cycleTimer = 0f;
	private float windupPeriod = 0f;
	private float firingPeriod = 0.2f;
	private float winddownPeriod = 0.1f;
	private float cooldownPeriod = 0.4f;

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
		cycleTimer = windupPeriod;
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
		else if (weaponState == WeaponState.WINDINGDOWN){
			UpdateWindingDown();
		}
		else if(weaponState == WeaponState.COOLINGDOWN){
			UpdateCoolingdown();
		}
	}

	void UpdateWindup(){
		cycleTimer -= Time.deltaTime;
		if (cycleTimer <= 0f){
			TransitionToFiring();
		}
	}

	void TransitionToFiring(){
			Fire();
			cycleTimer = firingPeriod;
			weaponState = WeaponState.FIRING;
	}

	 protected abstract void Fire();

	void UpdateFiring(){
		cycleTimer -= Time.deltaTime;
		if(cycleTimer <= 0f){
			TransitionToWindingDown();
		}
	}

	void TransitionToWindingDown(){
		cycleTimer = winddownPeriod;
		weaponState = WeaponState.WINDINGDOWN;
	}
	void UpdateWindingDown(){
		cycleTimer -= Time.deltaTime;
		if(cycleTimer <= 0f) TransitionToCoolingDown();
	}

	void TransitionToCoolingDown(){
		cycleTimer = cooldownPeriod;
		weaponState = WeaponState.COOLINGDOWN;
	}

	void UpdateCoolingdown(){
		cycleTimer -= Time.deltaTime;
		if(cycleTimer <= 0f){
			weaponState = WeaponState.IDLE;
		}
	}
}
