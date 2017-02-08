using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invokable : MonoBehaviour {
	public Effect[] StartEffects;	//Cast once when item invoked
	public Effect[] RecurrentEffects; //Cast at regular intervals while item invoked
	public Effect[] ReleaseEffects; //Cast once when invocation released
	public float recurrentEffectPeriod = 1f;
	public float recurrentEffectTimer = 0f;

	public float cooldownPeriod = 0.3f;

	CharacterComponentData componentData;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartInvoking(){
		CastEffectsInArray (StartEffects);
		ResetRecurrentEffectTimer ();
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

	public bool GetIsEncumbered(){
		return false;
	}
}
