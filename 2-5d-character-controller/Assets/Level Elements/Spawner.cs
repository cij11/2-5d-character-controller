using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public GameObject[] spawnableObjects;
	public GameObject spawnerWarning;

	float spawnTimer = 0f;
	public float spawnPeriod = 10f;
	public float firstSpawnDelay = 2f;
	public int numToSpawn = 4;
	public bool activated = false;

	int numSpawned = 0;
	List<CharacterCorpus> spawnedCorpusus;

	bool exhausted = false;

	public bool independentSpawner = false;
	float activationRange = 10f;
	float activationVariability = 5f;

	GameObject playerGO;

	// Use this for initialization
	void Start () {
		spawnedCorpusus = new List<CharacterCorpus> ();
		spawnTimer = firstSpawnDelay;
	}
	
	// Update is called once per frame
	void Update () {
		if (independentSpawner) {
			CheckPlayerNearby ();
		}

		if (activated) {
			if (!IsAllObjectsSpawned ()) {
				IncrementSpawnTimer ();
			}

			if (!exhausted) {
				if (CheckSpawnerExhausted ()) {
					exhausted = true;
					print ("Spawner exhausted");
				}
			}
		}
	}

	void IncrementSpawnTimer(){
		spawnTimer -= Time.deltaTime;
		if (spawnTimer <= 0){
			ShowSpawnWarning ();
			Invoke("SpawnRandomObject", 1f);
			spawnTimer = spawnPeriod;
		}
	}

	void SpawnRandomObject(){
		GameObject objectToSpawn = RandomlyChooseObject ();
		SpawnObject (objectToSpawn);
	}

	GameObject RandomlyChooseObject(){
		return spawnableObjects [Random.Range (0, spawnableObjects.Length)];
	}

	void SpawnObject(GameObject objectToSpawn){
		GameObject spawnedObject = Instantiate(objectToSpawn, this.transform.position, Quaternion.identity);

		CharacterCorpus spawnedCorpus = spawnedObject.GetComponent<CharacterCorpus> () as CharacterCorpus;
		if (spawnedCorpus != null) {
			spawnedCorpusus.Add (spawnedCorpus);
		}
		numSpawned++;
	}

	public bool CheckSpawnerExhausted(){
		if (IsAllObjectsSpawned () && AreAllObjectsSpawnedDead ()) {
			return true;
		} else {
			return false;
		}
	}

	public bool IsAllObjectsSpawned(){
		if (numSpawned >= numToSpawn) {
			return true;
		}
		return false;
	}

	public bool AreAllObjectsSpawnedDead(){
		RemoveDeadCorpusesFromList ();
		if (spawnedCorpusus.Count == 0) {
			return true;
		} else {
			return false;
		}
	}

	private void RemoveDeadCorpusesFromList(){
		for (int i = spawnedCorpusus.Count - 1; i >= 0; i--) {
			if (spawnedCorpusus [i] == null || spawnedCorpusus [i].Equals (null) || !spawnedCorpusus [i].GetIsAlive ()) {
				spawnedCorpusus.RemoveAt (i);
			}
		}
	}

	public bool GetIsSpawnerExhausted(){
		return exhausted;
	}

	public void SetSpawnables(GameObject[] newSpawnables){
		spawnableObjects = newSpawnables;
	}

	public void ActivateSpawner(){
		activated = true;
	}

	private void ShowSpawnWarning(){
		Instantiate (spawnerWarning, this.transform.position, Quaternion.identity);
	}

	private void InitialiseIndependentSpawner(){
		if (independentSpawner) {
			activated = false;
			playerGO = GameObject.FindGameObjectWithTag ("Player");
		}
	}

	private void CheckPlayerNearby(){
		Vector3 playerLocation = playerGO.transform.position;
		Vector3 offset = playerLocation - this.transform.position;

		if (offset.magnitude < activationRange) {
			activated = true;
		}
	}

	public void SetIndependent(){
		independentSpawner = true;
		InitialiseIndependentSpawner ();
	}
}
