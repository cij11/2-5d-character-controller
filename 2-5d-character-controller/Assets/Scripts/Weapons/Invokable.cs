using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invokable : MonoBehaviour {
	public Effect[] StartEffects;
	public Effect[] SustainEffects;
	public Effect[] ReleaseEffects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartInvoking(){
		CastEffectsInArray (StartEffects);
	}

	public void SustainInvoking(){
		CastEffectsInArray (SustainEffects);
	}

	public void ReleaseInvoking(){
		CastEffectsInArray (ReleaseEffects);
	}

	public void RegisterCharacterComponentsWithInvokable(){

	}

	private void CastEffectsInArray(Effect[] effectArray){
		foreach (Effect effect in effectArray) {
			effect.CastEffect ();
		}
	}

	public bool GetIsEncumbered(){
		return false;
	}
}
