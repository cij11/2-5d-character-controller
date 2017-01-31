using UnityEngine;
using System.Collections;

//Firing controller passes on button up commands, to avoid polling and lag on state changes.
public class FiringController : MonoBehaviour
{

	bool fireHeld = false;
    Invokable equipedInvokable;

	public void RegisterInvokable(Invokable invocable)
    {
		equipedInvokable = invocable;
    }

    public void InitiateFire()
    {
		if (equipedInvokable != null)
        {
			equipedInvokable.StartInvoking();
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
		if (equipedInvokable != null)
        {
			return equipedInvokable.GetIsEncumbered();
        }
        return false;
    }
}
