using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour {

	AimingController aimingController;
	CharacterCorpus corpus;
	SpriteRenderer targetSprite;
	Vector3 displacementFromBody;
	Vector3 defaultDisplacement = new Vector3(0f, 0f, 0f);
	bool isVisible = false;
	// Use this for initialization
	void Start () {
		//Try this pattern: Check for component.
		//If not present, set null.
		//Implement default behaviour for null commponent, and log.

		aimingController = this.transform.parent.Find("ActionControllers").GetComponent<AimingController>();
		corpus = this.transform.parent.GetComponent<CharacterCorpus> () as CharacterCorpus;
		targetSprite = GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
		QueryAimingController();
		PositionTargettingReticule();
		ShowOrHideTargettingReticule();
	}

	void QueryAimingController(){
		if (aimingController != null){
			displacementFromBody = aimingController.GetAimingVector();
			isVisible = aimingController.GetIsAiming();
		}
		else{
			displacementFromBody = defaultDisplacement;
		}
	}

	void PositionTargettingReticule(){
		this.transform.localPosition = displacementFromBody;
	}

	void ShowOrHideTargettingReticule(){
				//Only render reticule if fire held
		if(isVisible){
			targetSprite.enabled = true;
		}
		else{
			targetSprite.enabled = false;
		}

		if (!corpus.GetIsAlive ()) {
			targetSprite.enabled = false;
		}
	}
}
