using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject[] spawnableObjects;

	float spawnTimer = 0f;
	public float spawnPeriod = 10f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		IncrementSpawnTimer();
	}

	void IncrementSpawnTimer(){
		spawnTimer += Time.deltaTime;
		if (spawnTimer > spawnPeriod){
				SpawnRandomObject();
				spawnTimer = 0;
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
		Instantiate(objectToSpawn, this.transform.position, Quaternion.identity);
	}
}
