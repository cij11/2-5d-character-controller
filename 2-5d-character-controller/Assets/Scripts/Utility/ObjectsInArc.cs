using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsInArc{

	public static List<GameObject> GetGameObjectsInRadius(Vector3 origin, float radius){
		Collider[] colliders = Physics.OverlapSphere (origin, radius);
		List<GameObject> objectsInRadius = new List<GameObject> ();
		foreach (Collider collider in colliders) {
			objectsInRadius.Add (collider.gameObject);
		}
		return objectsInRadius;
	}



	//Get objects within arc of vector

	//Get closest object within arc of vector

	//Get objects tagged 'x' within arc of vector

	//Want: Radius, Vector, arc, closest/all, 

}
	