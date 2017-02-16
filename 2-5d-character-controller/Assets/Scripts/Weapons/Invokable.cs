using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invokable : MonoBehaviour {
	public Effect[] EquipEffects; 	//Cast once the item is equiped to hand
	public Effect[] StartEffects;	//Cast once when item invoked
	public Effect[] RecurrentEffects; //Cast at regular intervals while item invoked
	public Effect[] ReleaseEffects; //Cast once when invocation released
	public Effect[] DeEquipEffects; //Cast once the item is stowed.

	public float recurrentEffectPeriod = 1f;
	private float recurrentEffectTimer = 0f;
	private float minimumInvokingTimer = 0f;

	public float cooldownPeriod = 0.3f;
	public float windupPeriod = 0.3f; //Time until first recurrent effect happens

	CharacterComponentData componentData;

	public bool locksMovement = false;
	public bool locksAiming = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CastEquipEffects(){
		CastEffectsInArray (EquipEffects);
	}

	public void StartInvoking(){
		CastEffectsInArray (StartEffects);
		recurrentEffectTimer = windupPeriod;
	}
		
	void ResetRecurrentEffectTimer(){
		recurrentEffectTimer = recurrentEffectPeriod;
	}

	public void SustainInvoking(){
		UpdateRecurrentEffectTimer ();
		if (recurrentEffectTimer <= 0f){
			CastEffectsInArray (RecurrentEffects);
			ResetRecurrentEffectTimer ();
		}
	}

	void UpdateRecurrentEffectTimer(){
		if (recurrentEffectTimer > 0) {
			recurrentEffectTimer -= Time.deltaTime;
		}
	}

	public void ReleaseInvoking(){
		CastEffectsInArray (ReleaseEffects);
	}

	public void RegisterCharacterComponentsWithInvokable(CharacterComponentData charComponentData){
		componentData = charComponentData;
	}

	private void CastEffectsInArray(Effect[] effectArray){
		foreach (Effect effect in effectArray) {
			effect.CastEffect (componentData);
		}
	}

	public bool GetLocksMovement(){
		return locksMovement;
	}

	public bool GetLocksAiming(){
		return locksAiming;

	}
}
