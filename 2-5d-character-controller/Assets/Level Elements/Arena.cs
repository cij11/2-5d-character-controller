using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Arena : MonoBehaviour {
	public bool startingArena = false;
	public bool activated = false;
	public GameObject nextArenaGO = null;
	public bool clearsExit = false;
	public bool endsLevel = false;
	public bool endsGame = false;
	public bool restartsLevelOnPlayerDeath = false;
	public string sceneToLoad = "level_1";
	public string sceneToRestart = "bunker_arena";
	public Vector3[] botLeftClearRect;
	public Vector3[] topRightClearRect;

	public StampCollection stampCollection;

	CharacterCorpus playerCharacter;

	public bool fillsRectWithSpawners = false;
	public float rectWidth;
	public float rectHeight;
	public GameObject spawnerPrefab;
	public GameObject[] spawnableEnemies;
	public GameObject[] spawnableBuffs;
	public int numEnemySpawners = 0;
	public int numBuffSpawners = 0;

	bool spawnersExhausted = false;
	Spawner[] spawners;

	float digExplosionRadius = 4f;
	int digExplosionNodeSpacing = 2;
	float digExplosionVariation = 0.1f;

	Topography topography;
	List<Vector3> explosionLocationList;

	ActivationZone activationZone;
	float activationRadius;
	Vector3 activationPosition;
	bool hasActivationZone = false;

	// Use this for initialization
	void Start () {
		topography = (Topography)FindObjectOfType (typeof(Topography));

		StoreSpawners ();

		if (startingArena) {
			ActivateArena ();
		}

		GameObject playerGO = GameObject.FindGameObjectWithTag ("Player");
		playerCharacter = playerGO.GetComponent<CharacterCorpus> () as CharacterCorpus;

		//Find the door 'stamp', and store the corner coordinates.
		ParseDoor();

		ParseActivationZone ();

	}

	void GenerateSpawners(){
		for (int i = 0; i < numEnemySpawners; i++) {
			float proposedLocationX = Random.Range (-rectWidth / 2f, rectWidth / 2f);
			float proposedLocationY = Random.Range (-rectHeight / 2f, rectHeight / 2f);
/*			while (!topography.TestTileEmpty ((int)proposedLocationX, (int)proposedLocationY)) {
				proposedLocationX = Random.Range (-rectWidth / 2f, rectWidth / 2f);
				proposedLocationY = Random.Range (-rectHeight / 2f, rectHeight / 2f);
			}*/
	//		Vector3 spawnerLocation = new Vector3 (Random.Range (-rectWidth / 2f, rectWidth / 2f), Random.Range (-rectHeight / 2f, rectHeight / 2f), 0f);
			Vector3 spawnerLocation = topography.FindUnoccupiedTile();

			GameObject newSpawnerGO = Instantiate (spawnerPrefab, spawnerLocation, Quaternion.identity);
			Spawner newSpawner = newSpawnerGO.GetComponent<Spawner> () as Spawner;
			newSpawner.SetSpawnables (spawnableEnemies);
			newSpawnerGO.transform.parent = this.transform;
		}

		for (int i = 0; i < numBuffSpawners; i++) {
		//	Vector3 spawnerLocation = new Vector3 (Random.Range (-rectWidth / 2f, rectWidth / 2f), Random.Range (-rectHeight / 2f, rectHeight / 2f), 0f);
			Vector3 spawnerLocation = topography.FindUnoccupiedTile();
			GameObject newSpawnerGO = Instantiate (spawnerPrefab, spawnerLocation, Quaternion.identity);
			Spawner newSpawner = newSpawnerGO.GetComponent<Spawner> () as Spawner;
			newSpawner.SetSpawnables (spawnableBuffs);
			newSpawnerGO.transform.parent = this.transform;
		}

		StoreSpawners (); //Otherwise this will be missed when this is invoked on a delay
		SetSpawnersIndependent ();
	}

	// Get an array of all the Spawner's that are children of this arena's transform.
	void StoreSpawners(){
		spawners = GetComponentsInChildren<Spawner> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (activated) {
			if (!spawnersExhausted) {
				if (CheckSpawnersExhausted ()) {
					spawnersExhausted = true;
					activated = false;
					EndArena ();
				}
			}
		}

		if (startingArena) {
			if (!playerCharacter.GetIsAlive ()) {
				ReloadLevel ();
			}
		}

		if (!spawnersExhausted) {
			if (!activated) {
				if (hasActivationZone) {
					Vector3 playerOffset = activationPosition - playerCharacter.transform.position;
					if (playerOffset.magnitude < activationRadius) {
						print ("Player in activation radius");
						ActivateArena ();
					}
				}
			}
		}
	}

	// Return true if every child Spawner is exhausted (has nothing else to spawn, and everything it spawned is dead).
	bool CheckSpawnersExhausted(){
		bool exhausted = true;
		foreach (Spawner spawner in spawners) {
			//If any spawners are not exhausted, then the arena is not complete
			if (!spawner.GetIsSpawnerExhausted ()) {
				exhausted = false;
			}
		}
		return exhausted;
	}

	// Perform events that occur when this arena is completed.
	void EndArena(){
		print ("Arena complete");

		if(clearsExit)
			DigExit ();

		if(nextArenaGO != null)
			ActivateNextArena ();

		if (endsLevel) {
			Invoke ("LoadNextLevel", 2);
		}

	}

	void DigExit(){
	/*	if (stampCollection != null) {
			Topography topography = (Topography)FindObjectOfType (typeof(Topography));
			topography.ApplyStampCollection (stampCollection, botLeftClearRect, topRightClearRect);
		}*/

		//Loop through all clear rects, and add explosions to dig out their area.
		explosionLocationList = new List<Vector3> ();
		for (int k = 0; k < botLeftClearRect.Length; k++) {
			for (int i = (int)botLeftClearRect[k].x; i < (int)topRightClearRect[k].x; i += digExplosionNodeSpacing) {
				for (int j = (int)botLeftClearRect[k].y; j < (int)topRightClearRect[k].y; j += digExplosionNodeSpacing) {
					explosionLocationList.Add (new Vector3 ((float)i, (float)j + Random.Range (-digExplosionVariation, digExplosionVariation), 0f));
				}
			}
		}
		Invoke ("BlastClearanceHole", 0.05f);

	}

	void BlastClearanceHole(){
		if (explosionLocationList.Count > 0) {
			int nextLocationIndex = Random.Range (0, explosionLocationList.Count);
			topography.DigCircle (explosionLocationList [nextLocationIndex], digExplosionRadius, false, true);
			explosionLocationList.RemoveAt (nextLocationIndex);
			Invoke("BlastClearanceHole", 0.05f);
		}
	}

	void ActivateNextArena(){
		Arena nextArena = nextArenaGO.GetComponent<Arena> () as Arena;
		nextArenaGO.SetActive (true);
		nextArena.ActivateArena ();
	}

	void LoadNextLevel(){
		SceneManager.LoadScene (sceneToLoad, LoadSceneMode.Single);
	}

	void ReloadLevel(){
		SceneManager.LoadScene (sceneToRestart, LoadSceneMode.Single);
	}

	public void ActivateArena(){
		activated = true;

		if (fillsRectWithSpawners) {
			Invoke("GenerateSpawners", 1f);
		}
		StoreSpawners ();

		if(!fillsRectWithSpawners)
			ActivateSpawners ();
	}

	private void SetSpawnersIndependent(){
		foreach (Spawner spawner in spawners) {
			spawner.SetIndependent ();
		}
	}

	private void ActivateSpawners(){
		foreach (Spawner spawner in spawners) {
			spawner.ActivateSpawner ();
		}
		print ("Stored number of spawners " + spawners.Length);
	}

	private void ParseDoor(){

		BlastZone[] blastZones = GetComponentsInChildren<BlastZone> ();

		botLeftClearRect = new Vector3[blastZones.Length];
		topRightClearRect = new Vector3[blastZones.Length];
		int i = 0;

		foreach (BlastZone zone in blastZones) {

			Transform zoneTransform = zone.transform;
			if (zoneTransform != null) {
				Vector3 doorLocation = zoneTransform.position;
				float halfDoorWidth = zoneTransform.lossyScale.x / 2f;
				float halfDoorHeight = zoneTransform.lossyScale.y / 2f;

				Vector3 botLeftClearRectCoord = new Vector3 (doorLocation.x - halfDoorWidth, doorLocation.y - halfDoorHeight, 0f);
				Vector3 topRightClearRectCoord = new Vector3 (doorLocation.x + halfDoorWidth, doorLocation.y + halfDoorHeight, 0f);

				botLeftClearRect [i] = botLeftClearRectCoord;
				topRightClearRect [i] = topRightClearRectCoord;

				Destroy (zoneTransform.gameObject);

				i++;
			}
		}
	}

	private void ParseActivationZone(){
		activationZone = GetComponentInChildren<ActivationZone>() as ActivationZone;
		if (activationZone != null) {
			hasActivationZone = true;
			print ("activation zone found");
			activationPosition = activationZone.transform.position;
			print (activationPosition.ToString ());
			activationRadius = activationZone.transform.lossyScale.x / 2f;
			print (activationRadius.ToString ());
			Destroy (activationZone.gameObject);
		}
	}
}
