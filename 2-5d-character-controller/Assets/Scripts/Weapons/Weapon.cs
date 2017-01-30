using UnityEngine;
using System.Collections;

//Single responsibility: Weapons change state by being fired.
public abstract class Weapon : MonoBehaviour
{

    public GameObject launchableGO;

    protected AimingController aimingController;
    protected FiringController firingController;
    protected CharacterContactSensor contactSensor;
    protected Character character;
    protected MovementController movementController;
	protected SpriteRenderer spriteRenderer;

    protected WeaponState weaponState = WeaponState.IDLE;
    protected bool stationaryWindup = true;
	protected bool stationaryFiring = true;
	protected bool stationaryWindDown = true;

    private float cycleTimer = 0f;
    private float windupPeriod = 0.05f;
    private float firingPeriod = 0.1f;
    private float winddownPeriod = 0.01f;
    private float cooldownPeriod = 0.1f;

	bool windupQueued = false;

	public Vector3 gripOffset = new Vector3(0.4f, 0.05f, 0f);

    // Use this for initialization
    void Start()
    {
        LoadWeaponParameters();
    }

	public void RegisterCharacterComponentsWithWeapon(){
		aimingController = this.transform.parent.parent.Find("ActionControllers").GetComponent<AimingController>();
		firingController = this.transform.parent.parent.Find("ActionControllers").GetComponent<FiringController>();
		movementController = this.transform.parent.parent.Find("ActionControllers").GetComponent<MovementController>();
		spriteRenderer = this.transform.GetComponentInChildren<SpriteRenderer> () as SpriteRenderer;
		character = this.transform.parent.parent.GetComponent<Character>() as Character;
		contactSensor = this.transform.parent.parent.GetComponent<CharacterContactSensor>() as CharacterContactSensor;
	}

    protected abstract void LoadWeaponParameters();
    protected void SetCooldown(float newCooldown)
    {
        cooldownPeriod = newCooldown;
    }
    protected void SetFiringPeriod(float newFiringTime){
        firingPeriod = newFiringTime;
    }

    //Start the windup timer, and enter winding up mode.
    //At the end of winding up, enter wound up mode.
    //If in wound up mode and fire released, fire weapon.
    public void WindupCommand()
    {
		if (weaponState == WeaponState.IDLE) {
			weaponState = WeaponState.WINDING_UP;
			cycleTimer = windupPeriod;
		} else {
			windupQueued = true;
		}
    }

    // Update is called once per frame
    void Update()
    {
		PerformQueuedWindups ();
        UpdateFiringCycle();
    }

	void PerformQueuedWindups(){
		if (windupQueued) {
			if (weaponState == WeaponState.IDLE) {
				WindupCommand ();
				windupQueued = false;
			}
		}
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
            TransitionToCoolingDown();
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

    public void CancelFiring(){
        weaponState = WeaponState.IDLE;
    }

	public SpriteRenderer GetSpriteRenderer(){
		if (spriteRenderer != null) {
			return spriteRenderer;
		} else {
			return null;
		}
	}

	public virtual bool GetIsSwinging(){
		return false;
	}

	public void EnableSprite(){
		if(spriteRenderer != null)
		spriteRenderer.enabled = true;
	}
	public void DisableSprite(){
		if(spriteRenderer != null)
		spriteRenderer.enabled = false;
	}
}
