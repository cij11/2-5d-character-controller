using UnityEngine;
using System.Collections;

public class AimingController : MonoBehaviour
{

    float horizontalStored;
    float verticalStored;
	float horizontalInput;
	float verticalInput;

    Vector3 aimingVector;
    bool isHoldingFire;
	bool initiateAiming;

    //Default aiming will align with the direction the sprite is facing.
    public SpriteStateController characterSprite;

    // Use this for initialization
    void Start()
    {
        horizontalStored = 0f;
        verticalStored = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        DefaultAimingToStraightForwardWhenFiringStarted();
		MatchAimingToControlsIfFireAndDirectionHeld();
		aimingVector = new Vector3(horizontalStored, verticalStored, 0f);
    }

    void DefaultAimingToStraightForwardWhenFiringStarted()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.001f && Mathf.Abs(Input.GetAxis("Vertical")) < 0.001f)
            {
                SetHorizontalToSpriteDirection();
                verticalStored = 0f;
            }
        }
    }
    void SetHorizontalToSpriteDirection()
    {
        if (characterSprite.getXFlip())
        {
            horizontalStored = -1.0f;
        }
        else
        {
            horizontalStored = 1.0f;
        }
    }

	void MatchAimingToControlsIfFireAndDirectionHeld(){
		isHoldingFire = Input.GetButton("Fire1");
		if (isHoldingFire && ((Mathf.Abs(Input.GetAxis("Horizontal")) > 0.001f) || (Mathf.Abs(Input.GetAxis("Vertical")) > 0.001f) )){
			if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.001f){
				horizontalStored = Mathf.Sign(Input.GetAxis("Horizontal"));
			}
			else{
				horizontalStored = 0f;
			}
			if ( Mathf.Abs(Input.GetAxis("Vertical")) > 0.001f){
				verticalStored = Mathf.Sign(Input.GetAxis("Vertical"));
			}
			else{
				verticalStored = 0f;
			}
		}
	}

    public Vector3 GetAimingVector()
    {
        return aimingVector;
    }
    public bool GetIsHoldingFire()
    {
        return isHoldingFire;
    }

	public void SetHorizontalInput(float hor){
		horizontalInput = hor;
	}
		public void SetVerticalInput(float vert){
		verticalInput = hor;
	}
}
