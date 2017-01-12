using UnityEngine;
using System.Collections;

//Firing controller passes on button up commands, to avoid polling and lag on state changes.
public class FiringController : MonoBehaviour
{

	bool fireHeld = false;
    Weapon activeWeapon;

    public void RegisterWeapon(Weapon Weapon)
    {
        activeWeapon = Weapon;
    }

    public void InitiateFire()
    {
        if (activeWeapon != null)
        {
            activeWeapon.WindupCommand();
        }
		fireHeld = true;
    }

    public void ReleaseFire()
    {
		fireHeld = false;
    }

	public bool GetFireHeld(){
		return fireHeld;
	}
    public bool GetIsEncumbered()
    {
        if (activeWeapon != null)
        {
            return activeWeapon.GetIsEncumbered();
        }
        return false;
    }
}
