using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRangedProjectile : Effect {
	public GameObject projectile;
	public int numberOfProjectiles = 1;
	public float spreadAngle = 20f;
	public float lateralSpread = 0f;
	public float sprayAngle = 0f;
	public float lateralSpray = 0f;
	public float sprayTime = 0.0f;

	private float startingAngle;
	private float startingLatDistance;
	private float separationAngle;
	private float separationLatDistance;

	private int ithProjectile;

	List<int> randomIntBag;

	// Use this for initialization
	void Start () {
		if (numberOfProjectiles == 1) {
			startingAngle = 0f;
			separationAngle = 0f;

			startingLatDistance = 0f;
			separationLatDistance = 0f;


		} else {
			startingAngle = -spreadAngle / 2f;
			separationAngle = spreadAngle / (numberOfProjectiles - 1);

			startingLatDistance = -lateralSpread / 2f;
			separationLatDistance = lateralSpread / (numberOfProjectiles - 1);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect (CharacterComponentData importComponentData)
	{
		base.CastEffect (importComponentData);
		LaunchRangedProjectile ();

	}

	private void LaunchRangedProjectile(){
		randomIntBag = FillRandomIntBag (numberOfProjectiles);
		ithProjectile = numberOfProjectiles - 1;
		for (int i = 0; i < numberOfProjectiles; i++) {
			float timeDelay = Random.Range (0f, sprayTime); //Launch projectiles at a random time spread
			Invoke ("LaunchIthProjectile", timeDelay);
		}
	//	}
	}

	private List<int> FillRandomIntBag(int numberOfInts){
		List<int> intList = new List<int> ();
		for (int i = 0; i < numberOfInts; i++) {
			intList.Add (i);
		}
		return intList;
	}

	private int GetRandomIntFromBag(){
		int randomIndex = Random.Range (0, randomIntBag.Count);
		int randomInt = randomIntBag [randomIndex];
		randomIntBag.RemoveAt (randomIndex);
		return randomInt;
	}


	private void LaunchIthProjectile(){
		int i = GetRandomIntFromBag ();

		float divergenceAngle = startingAngle + i * separationAngle;
		divergenceAngle = divergenceAngle + Random.Range (-sprayAngle/2f, sprayAngle/2f);

		float divergenceLat = startingLatDistance + i * separationLatDistance;
		divergenceLat = divergenceLat + Random.Range (-lateralSpray / 2f, lateralSpray / 2f);

		//	if (componentData.IsCharacterInsantiated ()) {
		Vector3 worldAimingVector = componentData.GetAimingController ().GetAimingVectorWorldSpace ();

		Vector3 perpToAiming = new Vector3 (-worldAimingVector.y, worldAimingVector.x, 0f);
		Vector3 lateralOffset = perpToAiming * divergenceLat;

		worldAimingVector = Quaternion.Euler (0f, 0f, divergenceAngle) * worldAimingVector;
		GameObject newProjectile = (GameObject)Instantiate (projectile, componentData.GetItemManager ().GetCurrentItem ().GetBarrelTransform ().position + lateralOffset, Quaternion.identity);
		newProjectile.GetComponent<RangedProjectile> ().LoadLaunchParameters (componentData, worldAimingVector, componentData.GetAimingController ().GetFacingDirection ());
		newProjectile.GetComponent<RangedProjectile> ().Launch ();
	}
}
