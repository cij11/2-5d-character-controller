using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Arena : MonoBehaviour {
	public bool activated = false;
	public Arena nextArena = null;
	public bool clearsExit = false;
	public bool endsLevel = false;
	public bool endsGame = false;
	public string sceneToLoad = "level_1";
	public Vector3 botLeftClearRect;
	public Vector3 topRightClearRect;

	public StampCollection stampCollection;


	bool spawnersExhausted = false;
	Spawner[] spawners;

	float digExplosionRadius = 4f;
	int digExplosionNodeSpacing = 2;
	float digExplosionVariation = 0.1f;

	Topography topography;
	List<Vector3> explosionLocationList;

	// Use this for initialization
	void Start () {
		StoreSpawners ();
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

		if(nextArena != null)
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
		topography = (Topography)FindObjectOfType (typeof(Topography));
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

	}

	void LoadNextLevel(){
		SceneManager.LoadScene (sceneToLoad, LoadSceneMode.Single);
	}
}
