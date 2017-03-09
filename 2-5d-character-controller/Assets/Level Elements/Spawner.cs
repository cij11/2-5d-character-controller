using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public GameObject[] spawnableObjects;

	float spawnTimer = 0f;
	public float spawnPeriod = 10f;
	public float firstSpawnDelay = 2f;
	public int numToSpawn = 4;

	int numSpawned = 0;
	List<CharacterCorpus> spawnedCorpusus;

	bool exhausted = false;

	// Use this for initialization
	void Start () {
		spawnedCorpusus = new List<CharacterCorpus> ();
		spawnTimer = firstSpawnDelay;
	}
	
	// Update is called once per frame
	void Update () {
		if (!IsAllObjectsSpawned()) {
			IncrementSpawnTimer();
		}

		if (!exhausted) {
			if (CheckSpawnerExhausted()) {
				exhausted = true;
				print ("Spawner exhausted");
			}
		}
	}

	void IncrementSpawnTimer(){
		spawnTimer -= Time.deltaTime;
		if (spawnTimer <= 0){
			SpawnRandomObject();
			spawnTimer = spawnPeriod;
			numSpawned++;
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
}
