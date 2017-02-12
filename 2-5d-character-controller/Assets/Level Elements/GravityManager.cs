using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{

	public static GravityManager instance = null;              //Static instance of GravityManager which allows it to be accessed by any other script.

	public bool radialGravity;
	public Vector3 linearGravityDirection = new Vector3(0f, -1f, 0f);
	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			//if not, set instance to this
			instance = this;
		//If instance already exists and it's not this:
		else if (instance != this)
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);    

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
	}


	public Vector3 GetDownVector(Vector3 position){
		if (!radialGravity) {
			return linearGravityDirection;
		} else {
			Vector3 vecToCenter = -position;
			vecToCenter.Normalize ();
			return vecToCenter;
		}
	}
}