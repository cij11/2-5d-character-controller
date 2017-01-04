using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	public GameObject enemy;

	int maxEnemiesToSpawn = 300;
	int enemiesSpawned = 0;
	float spawnTimer = 0f;
	float spawnPeriod = 0.02f;
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
			if(enemiesSpawned < maxEnemiesToSpawn){
				enemiesSpawned++;
				SpawnEnemy();
				spawnTimer = 0;
			}
		}
	}

	void SpawnEnemy(){
		Instantiate(enemy, this.transform.position, Quaternion.identity);
	}
}
