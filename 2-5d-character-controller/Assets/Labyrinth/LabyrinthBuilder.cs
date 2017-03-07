using System.Collections;
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

	public GameObject circleHoleStampPrefab;
	public GameObject rectHoleStampPrefab;

	List<LabyrinthRoom> unfinishedRoomList;
	List<LabyrinthRoom> finishedRoomList;

	List<LabyrinthTunnel> tunnelList;

	ArenaCollection arenaCollection;

	// Use this for initialization
	void Start () {

	}

	public void GenerateLabyrinth(){
		unfinishedRoomList = new List<LabyrinthRoom> ();
		finishedRoomList = new List<LabyrinthRoom> ();
		tunnelList = new List<LabyrinthTunnel> ();

		BuildLabyrinth ();
		ConvertLabyrinthToStampCollection ();
		Destroy (this.gameObject);
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

				float proposedDiameter = roomDiameter + Random.Range (0f, diamaterVariation);

				//Ensure that the separation radius is always sufficient to prevent the new room overlapping the current room.
				float randomRoomSeparation = (roomSeparation + Random.Range(0f, separationVariation));
				if (randomRoomSeparation < (proposedDiameter / 2f + testingRoom.GetRadius())){
					randomRoomSeparation = (proposedDiameter / 2f + testingRoom.GetRadius () + 2f);
				}

				Vector3 proposedLocation = testingRoom.GetPosition () + exploringVector * randomRoomSeparation;
				
				if (TestProposedLocation (proposedLocation, proposedDiameter)) {
					LabyrinthRoom newRoom = BuildRoom (proposedLocation, proposedDiameter);
					BuildCorridor (newRoom.GetIncomingConnectionPoint(exploringDirection), testingRoom.GetOutgoingConnectionPoint(exploringDirection));
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

	LabyrinthRoom BuildRoom(Vector3 location, float diameter){
		LabyrinthRoom newRoom = new LabyrinthRoom (location, diameter);
		unfinishedRoomList.Add (newRoom);
		GameObject newRoomBoundry = Instantiate (roomBoundryPrefab, location, Quaternion.identity);
		newRoomBoundry.transform.localScale = new Vector3 (diameter, diameter, 1f);
		newRoomBoundry.transform.parent = this.transform;
		return newRoom;
	}

	void BuildCorridor(Vector3 start, Vector3 end){
		LabyrinthTunnel newTunnel = new LabyrinthTunnel (start, end);
		tunnelList.Add (newTunnel);

		GameObject newCorridor = Instantiate (corridorPrefab, newTunnel.GetPosition(), Quaternion.identity);
		newCorridor.transform.localScale = new Vector3 (1f, newTunnel.GetLength(), 1f);
		newCorridor.transform.rotation = Quaternion.LookRotation (transform.forward, (start - end));

		newCorridor.transform.parent = this.transform;
	}

	void ConvertLabyrinthToStampCollection(){
		arenaCollection = GetComponent<ArenaCollection> () as ArenaCollection;

		StampCollection rootStampCollection = this.transform.parent.GetComponentInChildren<StampCollection> () as StampCollection;

		foreach (LabyrinthRoom room in finishedRoomList) {
			//GameObject newCircleStamp = Instantiate (circleHoleStampPrefab, room.GetPosition(), Quaternion.identity);
			GameObject newArenaStamp = Instantiate(arenaCollection.GetRandomArena(), room.GetPosition(), Quaternion.identity);
			newArenaStamp.transform.localScale = new Vector3 (room.GetDiameter(), room.GetDiameter (), 1f);
			newArenaStamp.transform.rotation = Quaternion.LookRotation (transform.forward, room.GetUpVector ());
			rootStampCollection.AddChildStamp (newArenaStamp);
		}

		foreach (LabyrinthTunnel tunnel in tunnelList) {
			GameObject newRectStamp = Instantiate (rectHoleStampPrefab, tunnel.GetPosition (), Quaternion.identity);
			newRectStamp.transform.localScale = new Vector3 (3f, tunnel.GetLength(), 1f);
			newRectStamp.transform.rotation = Quaternion.LookRotation (transform.forward, (tunnel.GetStart() - tunnel.GetEnd()));
			rootStampCollection.AddChildStamp (newRectStamp);
		}
	}
}
