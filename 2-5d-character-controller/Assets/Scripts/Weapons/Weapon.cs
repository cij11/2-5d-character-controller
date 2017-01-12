using UnityEngine;
using System.Collections;

//Single responsibility: Weapons change state by being fired.
public abstract class Weapon : MonoBehaviour
{

    public GameObject launchableGO;

    protected AimingController aimingController;
    protected FiringController firingController;
    protected Character character;

    protected WeaponState weaponState = WeaponState.IDLE;
    protected bool stationaryWindup = true;
	protected bool stationaryFiring = true;
	protected bool stationaryWindDown = true;

    private float cycleTimer = 0f;
    private float windupPeriod = 0.2f;
    private float firingPeriod = 0.2f;
    private float winddownPeriod = 0.2f;
    private float cooldownPeriod = 1f;

    // Use this for initialization
    void Start()
    {
        aimingController = this.transform.parent.Find("ActionControllers").GetComponent<AimingController>();
        firingController = this.transform.parent.Find("ActionControllers").GetComponent<FiringController>();
        character = this.transform.parent.GetComponent<Character>() as Character;
        LoadWeaponParameters();
    }

    protected abstract void LoadWeaponParameters();
    protected void SetCooldown(float newCooldown)
    {
        cooldownPeriod = newCooldown;
    }

    //Start the windup timer, and enter winding up mode.
    //At the end of winding up, enter wound up mode.
    //If in wound up mode and fire released, fire weapon.
    public void WindupCommand()
    {
        weaponState = WeaponState.WINDING_UP;
        cycleTimer = windupPeriod;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFiringCycle();
    }

    void UpdateFiringCycle()
    {
        if (weaponState == WeaponState.WINDING_UP)
        {
            UpdateWindup();
        }
        else if (weaponState == WeaponState.WOUND_UP)
        {
            UpdateWoundup();
        }
        else if (weaponState == WeaponState.FIRING)
        {
            UpdateFiring();
        }
        else if (weaponState == WeaponState.WINDING_DOWN)
        {
            UpdateWindingDown();
        }
        else if (weaponState == WeaponState.COOLING_DOWN)
        {
            UpdateCoolingdown();
        }
    }

    void UpdateWindup()
    {
        cycleTimer -= Time.deltaTime;
        if (cycleTimer <= 0f)
        {
            TransitionToWoundUp();
        }
    }

    void TransitionToWoundUp()
    {
        //No time limit for how long the weapon can stay wound up
        weaponState = WeaponState.WOUND_UP;
    }

    void UpdateWoundup()
    {
        //Leave Wound up state when fire button is no longer being pressed
        if (!firingController.GetFireHeld())
        {
            TransitionToFiring();
        }
    }

    void TransitionToFiring()
    {
        Fire();
        cycleTimer = firingPeriod;
        weaponState = WeaponState.FIRING;
    }

    protected abstract void Fire();

    void UpdateFiring()
    {
        cycleTimer -= Time.deltaTime;
        if (cycleTimer <= 0f)
        {
            TransitionToWindingDown();
        }
    }

    void TransitionToWindingDown()
    {
        cycleTimer = winddownPeriod;
        weaponState = WeaponState.WINDING_DOWN;
    }
    void UpdateWindingDown()
    {
        cycleTimer -= Time.deltaTime;
        if (cycleTimer <= 0f) TransitionToCoolingDown();
    }

    void TransitionToCoolingDown()
    {
        cycleTimer = cooldownPeriod;
        weaponState = WeaponState.COOLING_DOWN;
    }

    void UpdateCoolingdown()
    {
        cycleTimer -= Time.deltaTime;
        if (cycleTimer <= 0f)
        {
            weaponState = WeaponState.IDLE;
        }
    }

    public bool GetIsEncumbered()
    {
        if (stationaryWindup)
        {
            if (weaponState == WeaponState.WINDING_UP) return true;
            if (weaponState == WeaponState.WOUND_UP) return true;
        }

		if(stationaryFiring){
			if(weaponState == WeaponState.FIRING){
				return true;
			}
		}

		if(stationaryWindDown){
			if(weaponState == WeaponState.WINDING_DOWN){
				return true;
			}
		}
        return false;
    }
}
