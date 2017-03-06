using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthBuilder : MonoBehaviour {
	public float WorldRadius = 100f;
	public float roomDiameter = 5f;
	public float roomSeparation = 2f;
	public GameObject roomBoundryPrefab;

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
		BuildRoom (new Vector3 (1f, 0f, 0f));

		//While there are unfinished rooms
		while (unfinishedRoomList.Count != 0 && finishedRoomList.Count < 20) {
			int nextRoomIndex = Random.Range (0, unfinishedRoomList.Count);
			LabyrinthRoom testingRoom = unfinishedRoomList [nextRoomIndex];
			print ("Testing room number " + nextRoomIndex.ToString());
			//If this room has remaining directions, pick a random direction to try and extend
			if (testingRoom.HasUnexploredDirection ()) {
				MovementDirection exploringDirection = testingRoom.PopRandomUnexploredDirection ();
				print ("Testing direction " + exploringDirection.ToString ());
				Vector3 exploringVector = testingRoom.DirectionToVector (exploringDirection);

				Vector3 proposedLocation = testingRoom.GetPosition () + exploringVector * roomSeparation;
				BuildRoom (proposedLocation);

			} else { //Otherwise, add to finished room list, and remove from unfinished room list.
				finishedRoomList.Add (testingRoom);
				unfinishedRoomList.RemoveAt (nextRoomIndex);
			}
		}
	}

	void BuildRoom(Vector3 location){
		LabyrinthRoom newRoom = new LabyrinthRoom (location);
		unfinishedRoomList.Add (newRoom);
		GameObject newRoomBoundry = Instantiate (roomBoundryPrefab, location, Quaternion.identity);
	}
}
