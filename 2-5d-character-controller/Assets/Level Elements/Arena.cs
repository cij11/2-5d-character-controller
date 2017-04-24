using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Arena : MonoBehaviour {
	public bool activated = false;
	public GameObject nextArenaGO = null;
	public bool clearsExit = false;
	public bool endsLevel = false;
	public bool endsGame = false;
	public bool restartsLevelOnPlayerDeath = false;
	public string sceneToLoad = "level_1";
	public string sceneToRestart = "bunker_arena";
	public Vector3 botLeftClearRect;
	public Vector3 topRightClearRect;

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

	// Use this for initialization
	void Start () {
		topography = (Topography)FindObjectOfType (typeof(Topography));

		if (fillsRectWithSpawners) {
			GenerateSpawners ();
		}
		StoreSpawners ();

		GameObject playerGO = GameObject.FindGameObjectWithTag ("Player");
		playerCharacter = playerGO.GetComponent<CharacterCorpus> () as CharacterCorpus;

		//Find the door 'stamp', and store the corner coordinates.
		Transform doorTransform = transform.FindChild ("Door");
		if (doorTransform != null) {
			Vector3 doorLocation = doorTransform.position;
			float halfDoorWidth = doorTransform.lossyScale.x / 2f;
			float halfDoorHeight = doorTransform.lossyScale.y / 2f;

			botLeftClearRect = new Vector3 (doorLocation.x - halfDoorWidth, doorLocation.y - halfDoorHeight, 0f);
			topRightClearRect = new Vector3 (doorLocation.x + halfDoorWidth, doorLocation.y + halfDoorHeight, 0f);

			Destroy (doorTransform.gameObject);
		}
	}

	void GenerateSpawners(){
		for (int i = 0; i < numEnemySpawners; i++) {
			float proposedLocationX = Random.Range (-rectWidth / 2f, rectWidth / 2f);
			float proposedLocationY = Random.Range (-rectHeight / 2f, rectHeight / 2f);
/*			while (!topography.TestTileEmpty ((int)proposedLocationX, (int)proposedLocationY)) {
				proposedLocationX = Random.Range (-rectWidth / 2f, rectWidth / 2f);
				proposedLocationY = Random.Range (-rectHeight / 2f, rectHeight / 2f);
			}*/
			Vector3 spawnerLocation = new Vector3 (Random.Range (-rectWidth / 2f, rectWidth / 2f), Random.Range (-rectHeight / 2f, rectHeight / 2f), 0f);
			GameObject newSpawnerGO = Instantiate (spawnerPrefab, spawnerLocation, Quaternion.identity);
			Spawner newSpawner = newSpawnerGO.GetComponent<Spawner> () as Spawner;
			newSpawner.SetSpawnables (spawnableEnemies);
			newSpawnerGO.transform.parent = this.transform;
		}

		for (int i = 0; i < numBuffSpawners; i++) {
			Vector3 spawnerLocation = new Vector3 (Random.Range (-rectWidth / 2f, rectWidth / 2f), Random.Range (-rectHeight / 2f, rectHeight / 2f), 0f);
			GameObject newSpawnerGO = Instantiate (spawnerPrefab, spawnerLocation, Quaternion.identity);
			Spawner newSpawner = newSpawnerGO.GetComponent<Spawner> () as Spawner;
			newSpawner.SetSpawnables (spawnableBuffs);
			newSpawnerGO.transform.parent = this.transform;
		}
	}

	// Get an array of all the Spawner's that are children of this arena's transform.
	void StoreSpawners(){
		spawners = GetComponentsInChildren<Spawner> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!spawnersExhausted) {
			if (CheckSpawnersExhausted ()) {
				spawnersExhausted = true;
				activated = false;
				EndArena ();
			}
		}

		if (!playerCharacter.GetIsAlive ()) {
			ReloadLevel ();
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

		explosionLocationList = new List<Vector3> ();
		for (int i = (int)botLeftClearRect.x; i < (int)topRightClearRect.x; i+=digExplosionNodeSpacing) {
			for (int j = (int)botLeftClearRect.y; j < (int)topRightClearRect.y; j+=digExplosionNodeSpacing) {
				explosionLocationList.Add (new Vector3 ((float)i, (float)j + Random.Range(-digExplosionVariation, digExplosionVariation), 0f));
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
	}

	void LoadNextLevel(){
		SceneManager.LoadScene (sceneToLoad, LoadSceneMode.Single);
	}

	void ReloadLevel(){
		SceneManager.LoadScene (sceneToRestart, LoadSceneMode.Single);
	}
}
