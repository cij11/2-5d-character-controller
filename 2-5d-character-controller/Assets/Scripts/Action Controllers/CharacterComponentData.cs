using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Information about the character that an effect might require to function.
//Characters may die while effects are ongoing, so access in a safe 'if != null' manner.
public class CharacterComponentData {
	CharacterMovementActuator movementActuator;
	AimingController aimingController;
	FiringController firingController;
	MovementController movementController;
	ItemManager itemManager;
	Character character;
	CharacterCorpus corpus;
	Transform characterTransform;

	public CharacterComponentData(Character C){
		character = C;
		aimingController = C.GetComponentInChildren<AimingController> () as AimingController;
		firingController = C.GetComponentInChildren<FiringController> () as FiringController;
		movementController = C.GetComponentInChildren<MovementController> () as MovementController;
		movementActuator = C.GetComponent<CharacterMovementActuator> () as CharacterMovementActuator;
		itemManager = C.GetComponentInChildren<ItemManager>() as ItemManager;
		corpus = C.GetComponent<CharacterCorpus> () as CharacterCorpus;
		characterTransform = movementActuator.transform;
	}

	public bool IsCharacterInsantiated(){
		if (character == null || character.Equals (null)) {
			return false;
		} else {
			return true;
		}
	}

	public CharacterMovementActuator GetMovementActuator(){
		if (IsCharacterInsantiated()) {
			return movementActuator;
		} else {
			return null;
		}
	}
	public AimingController GetAimingController(){
		if (IsCharacterInsantiated()) {
			return aimingController;
		} else {
			return null;
		}
	}
	public FiringController GetFiringController(){
		if (IsCharacterInsantiated()) {
			return firingController;
		} else {
			return null;
		}
	}

	public MovementController GetMovementController(){
		if (IsCharacterInsantiated()) {
			return movementController;
		} else {
			return null;
		}
	}

	public ItemManager GetItemManager(){
		if (IsCharacterInsantiated()) {
			return itemManager;
		} else {
			return null;
		}
	}

	public Character GetCharacter(){
		if (IsCharacterInsantiated()) {
			return character;
		} else {
			return null;
		}
	}

	public CharacterCorpus GetCharacterCorpus(){
		if (IsCharacterInsantiated()) {
			return corpus;
		} else {
			return null;
		}
	}


	public Transform GetCharacterTransform(){
		if (IsCharacterInsantiated ()) {
			return characterTransform;
		} else {
			return null;
		}
	}
}
