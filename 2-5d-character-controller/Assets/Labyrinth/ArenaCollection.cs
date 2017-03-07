using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaCollection : MonoBehaviour {

	public GameObject[] arenas;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject GetRandomArena(){
		return arenas [Random.Range (0, arenas.Length)];
	}
}
