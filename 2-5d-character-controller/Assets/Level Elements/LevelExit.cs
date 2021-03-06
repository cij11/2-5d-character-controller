﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour {
	Topography topography;

	Vector3 exitPosition;
	public float exitRadius = 3;

	GameObject player;

	public string nextLevel;

	bool reached = false;
	Transform exitTransform;

	float enlargeSpeed = 100f;

	// Use this for initialization
	void Start () {
		Invoke("InitialisePosition", 1f);
		StorePlayer ();
		ResizeGraphic ();
	}

	private void InitialisePosition (){
		topography = GameObject.Find ("Topography").GetComponent<Topography> () as Topography;

		float worldWidth = topography.numChunks * topography.chunkSize;
		float worldHeight = worldWidth;

		bool clearSpaceFound = false;
		int randomTimeout = 0;
		int proposedX = 0;
		int proposedY = 0;
		while (!clearSpaceFound && randomTimeout < 2000) {
			 proposedX = (int) Random.Range(0, worldWidth);
			 proposedY = (int) Random.Range(0, worldWidth);

			if (topography.TestTileEmpty (proposedX, proposedY)) {
				clearSpaceFound = true;
			}
				//Reject positions too close to the player
			if (proposedX > worldWidth / 4 && proposedX < worldWidth - worldWidth/ 4)
					clearSpaceFound = false;
			if (proposedY > worldHeight / 4 && proposedY < worldHeight -worldHeight/ 4)
					clearSpaceFound = false;
						

		//	print ("  " + proposedX.ToString () + "  " + proposedY.ToString ());
			randomTimeout++;
		}

		exitPosition = new Vector3 ((float)proposedX - worldWidth/2f, (float)proposedY - worldWidth/2f, 0);
		this.transform.position = exitPosition;
	}

	private void StorePlayer (){
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	private void ResizeGraphic(){
		exitTransform = this.transform.FindChild ("Sphere");
		exitTransform.localScale = new Vector3 (exitRadius * 2f, exitRadius * 2f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		if (!reached) {
							if (PlayerWithinRange (exitRadius)) {
				Invoke ("EndLevel", 1f);
				reached = true;
			}
		}

		if (reached) {
			exitTransform.localScale = new Vector3 (exitTransform.localScale.x + Time.deltaTime*enlargeSpeed, exitTransform.localScale.y + Time.deltaTime*enlargeSpeed, 1f);
		}
	}

					private bool PlayerWithinRange(float radius){
		Vector3 offset = player.transform.position - exitPosition;
		if ( (offset.magnitude) < radius) {
			return true;
		}
		return false;
	}

	private void EndLevel(){
		SceneManager.LoadScene (nextLevel, LoadSceneMode.Single);
	}
}
