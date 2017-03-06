﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthBuilder : MonoBehaviour {
	public float WorldRadius = 100f;
	public float roomDiameter = 8f;
	public float roomSeparation = 12f;
	public float diamaterVariation = 4f;
	public float separationVariation = 8f;
	public float overlapAllowance = 1f;
	public GameObject roomBoundryPrefab;
	public GameObject corridorPrefab;

	List<LabyrinthRoom> unfinishedRoomList;
	List<LabyrinthRoom> finishedRoomList;

	// Use this for initialization
	void Start () {
		unfinishedRoomList = new List<LabyrinthRoom> ();
		finishedRoomList = new List<LabyrinthRoom> ();
		BuildLabyrinth ();
	}

	//Build a labyrinth room by room. Pick a random room, and explore a random direction from that room.
	//Once every direction has been explored from a room (and either been extended, or discarded), remove
	//that room from the pool of unfinished rooms.
	void BuildLabyrinth(){
		//Build a starting room
		BuildRoom (new Vector3 (1f, 0f, 0f), roomDiameter);

		//While there are unfinished rooms
		while (unfinishedRoomList.Count != 0 && finishedRoomList.Count < 200) {
			int nextRoomIndex = Random.Range (0, unfinishedRoomList.Count);
			LabyrinthRoom testingRoom = unfinishedRoomList [nextRoomIndex];
			print ("Testing room number " + nextRoomIndex.ToString());
			//If this room has remaining directions, pick a random direction to try and extend
			if (testingRoom.HasUnexploredDirection ()) {
				MovementDirection exploringDirection = testingRoom.PopRandomUnexploredDirection ();
				print ("Testing direction " + exploringDirection.ToString ());
				Vector3 exploringVector = testingRoom.DirectionToVector (exploringDirection);

				Vector3 proposedLocation = testingRoom.GetPosition () + exploringVector * (roomSeparation + Random.Range(0f, separationVariation));
				float proposedDiameter = roomDiameter + Random.Range (0f, diamaterVariation);
				if (TestProposedLocation (proposedLocation, proposedDiameter)) {
					BuildRoom (proposedLocation, proposedDiameter);
					BuildCorridor (proposedLocation, testingRoom.GetPosition());
				}

			} else { //Otherwise, add to finished room list, and remove from unfinished room list.
				finishedRoomList.Add (testingRoom);
				unfinishedRoomList.RemoveAt (nextRoomIndex);
			}
		}
	}


	bool TestProposedLocation(Vector3 proposedLocation, float proposedDiameter){
		if (proposedLocation.magnitude > WorldRadius) {
			return false;
		}

		//Make the overlap sphere slightly smaller than the room, so that rooms can touch/merge
		Collider[] overlappingRooms = (Physics.OverlapSphere (proposedLocation, (proposedDiameter-overlapAllowance) / 2f));
		if (overlappingRooms.Length != 0){
			return false;
		}
		return true;
		/*
		foreach (LabyrinthRoom existingRoom in unfinishedRoomList) {
			float distanceBetweenExistingAndProposed = (existingRoom.GetPosition () - proposedLocation).magnitude;
			if (distanceBetweenExistingAndProposed < roomSeparation) {
				return false;
			}
		}

		foreach (LabyrinthRoom existingRoom in finishedRoomList) {
			float distanceBetweenExistingAndProposed = (existingRoom.GetPosition () - proposedLocation).magnitude;
			if (distanceBetweenExistingAndProposed < roomSeparation) {
				return false;
			}
		}
		*/
	}

	void BuildRoom(Vector3 location, float diameter){
		LabyrinthRoom newRoom = new LabyrinthRoom (location);
		unfinishedRoomList.Add (newRoom);
		GameObject newRoomBoundry = Instantiate (roomBoundryPrefab, location, Quaternion.identity);
		newRoomBoundry.transform.localScale = new Vector3 (diameter, diameter, 1f);
	}

	void BuildCorridor(Vector3 start, Vector3 end){
		Vector3 corridorPosition = (start + end) / 2f;
		float corridorLength = (start - end).magnitude;
		GameObject newCorridor = Instantiate (corridorPrefab, corridorPosition, Quaternion.identity);
		newCorridor.transform.localScale = new Vector3 (1f, corridorLength, 1f);
		newCorridor.transform.rotation = Quaternion.LookRotation (transform.forward, (start - end));
	}
}