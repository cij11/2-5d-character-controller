using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawnObject : Effect {

	public GameObject objectToSpawn;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void CastEffect(CharacterComponentData importComponentData){
		base.CastEffect (importComponentData);
		SpawnObject ();
	}

	void SpawnObject(){
		Instantiate (objectToSpawn, this.transform.position, Quaternion.identity);
	}
}
