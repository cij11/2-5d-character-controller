﻿using UnityEngine;
using System.Collections;

public class AimingController : MonoBehaviour
{

    float horizontalStored;
    float verticalStored;
	float horizontalInput;
	float verticalInput;

    Vector3 aimingVector;
    bool isAiming;

    //Default aiming will align with the direction the sprite is facing.
    public SpriteStateController characterSprite;

    // Use this for initialization
    void Start()
    {
        horizontalStored = 0f;
        verticalStored = 0f;
		horizontalInput = 0f;
		verticalInput = 0f;
		isAiming = false;

    }

    // Update is called once per frame
    void Update()
    {
		MatchAimingToControlsIfFireAndDirectionHeld();
		aimingVector = new Vector3(horizontalStored, verticalStored, 0f);
    }

	public void StartTargetting(){
		isAiming = true;
		DefaultAimingStraightIfNoDirectionHeld();
	}

    void DefaultAimingStraightIfNoDirectionHeld()
    {

        if (Mathf.Abs(horizontalInput) < 0.001f && Mathf.Abs(verticalInput) < 0.001f)
        {
            SetHorizontalToSpriteDirection();
            verticalStored = 0f;
        }

    }

	public void StopTargetting(){
		isAiming = false;
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
		if (isAiming && ((Mathf.Abs(horizontalInput) > 0.001f) || (Mathf.Abs(verticalInput) > 0.001f) )){
			horizontalStored = horizontalInput;
			verticalStored = verticalInput;
		}
	}

    public Vector3 GetAimingVector()
    {
        return aimingVector;
    }
    public bool GetIsAiming()
    {
        return isAiming;
    }

	public void SetHorizontalInput(float hor){
		horizontalInput = hor;
	}
		public void SetVerticalInput(float vert){
		verticalInput = vert;
	}
}
