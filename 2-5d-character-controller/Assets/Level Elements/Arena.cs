using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

	bool spawnersExhausted = false;

	Spawner[] spawners;

	// Use this for initialization
	void Start () {
		StoreSpawners ();
	}

	void StoreSpawners(){
		spawners = GetComponentsInChildren<Spawner> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!spawnersExhausted) {
			if (CheckSpawnersExhausted ()) {
				spawnersExhausted = true;
				print ("Arena complete");
			}
		}
	}

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
}
