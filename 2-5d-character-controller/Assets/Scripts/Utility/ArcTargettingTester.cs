using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcTargettingTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject gameObject in ObjectsInArc.GetGameObjectsInRadius(this.transform.position, 5f)){
			Debug.DrawLine (this.transform.position, gameObject.transform.position);
		}
	}
}
