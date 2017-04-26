using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthBuilder : MonoBehaviour {
	public float worldRadius = 100f;
	public float worldWidth = 200f;
	public float worldHeight = 150f;

	public float diggableWidth = 150f;
	public float diggableHeight = 100f;

	public bool circularWorld = false;
	public bool radiallyAlignedRooms = false;

	public float roomDiameter = 8f;
	public float roomSeparation = 12f;
	public float diamaterVariation = 4f;
	public float separationVariation = 8f;
	public float overlapAllowance = 1f;
	public float tunnelWidth = 3f;
	public float widthVariation = 3f;

	public GameObject roomBoundryPrefab;
	public GameObject corridorPrefab;

	public GameObject circleHoleStampPrefab;
	public GameObject rectHoleStampPrefab;
	public GameObject rectSolidStampPrefab;
	public GameObject impassableStampPrefab;

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
		BuildRoom (new Vector3 (1f, 0f, 0f), roomDiameter * 2 + Random.Range(0f, diamaterVariation));

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
		//Test that the room is inside the circular world radius
		if (circularWorld) {
			if (proposedLocation.magnitude > worldRadius) {
				return false;
			}
		} else { //Test that the room is inside the rectangular world bounds
			if (proposedLocation.x > diggableWidth / 2f)
				return false;
			if (proposedLocation.x < -diggableWidth / 2f)
				return false;
			if (proposedLocation.y > diggableHeight / 2f)
				return false;
			if (proposedLocation.y < -diggableHeight / 2f)
				return false;
		}

		//Test that the room doesn't overlap an existing room.
		//Make the overlap sphere slightly smaller than the room, so that rooms can touch/merge
		Collider[] overlappingRooms = (Physics.OverlapSphere (proposedLocation, (proposedDiameter-overlapAllowance) / 2f));
		if (overlappingRooms.Length != 0){
			return false;
		}
		return true;
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
		LabyrinthTunnel newTunnel = new LabyrinthTunnel (start, end, tunnelWidth + Random.Range(0f, widthVariation));
		tunnelList.Add (newTunnel);

		GameObject newCorridor = Instantiate (corridorPrefab, newTunnel.GetPosition(), Quaternion.identity);
		newCorridor.transform.localScale = new Vector3 (newTunnel.GetWidth(), newTunnel.GetLength(), 1f);
		newCorridor.transform.rotation = Quaternion.LookRotation (transform.forward, (start - end));

		newCorridor.transform.parent = this.transform;
	}

	void ConvertLabyrinthToStampCollection(){
		arenaCollection = GetComponent<ArenaCollection> () as ArenaCollection;

		StampCollection rootStampCollection = this.transform.parent.GetComponentInChildren<StampCollection> () as StampCollection;

		foreach (LabyrinthRoom room in finishedRoomList) {
			//GameObject newCircleStamp = Instantiate (circleHoleStampPrefab, room.GetPosition(), Quaternion.identity);
			GameObject newArenaStamp = Instantiate(arenaCollection.GetRandomArena(), room.GetPosition(), Quaternion.identity);

			float horFlip = 1;
			float vertFlip = 1;
			//Flip horizontally or vertically randomly.
			if (Random.Range (0, 1) > 0.5f) {
				horFlip = -1;
			}
			if (Random.Range (0, 1) > 0.5f) {
				vertFlip = -1;
			}
			newArenaStamp.transform.localScale = new Vector3 (room.GetDiameter() * horFlip, room.GetDiameter () * vertFlip, 1f);

			//Rotate 90 degrees half the time
			if (Random.Range (0, 1) > 0.5f) {
				newArenaStamp.transform.rotation = Quaternion.LookRotation (transform.forward, room.GetUpVector ());
			} else {
				newArenaStamp.transform.rotation = Quaternion.LookRotation (transform.forward, room.GetRightVector ());
			}
			rootStampCollection.AddChildStamp (newArenaStamp);
		}

		foreach (LabyrinthTunnel tunnel in tunnelList) {
			GameObject newRectStamp = Instantiate (rectHoleStampPrefab, tunnel.GetPosition (), Quaternion.identity);
			newRectStamp.transform.localScale = new Vector3 (tunnel.GetWidth(), tunnel.GetLength(), 1f);
			newRectStamp.transform.rotation = Quaternion.LookRotation (transform.forward, (tunnel.GetStart() - tunnel.GetEnd()));
			rootStampCollection.AddChildStamp (newRectStamp);
		}

		AddBoundingStamps (rootStampCollection);
	}

	void AddBoundingStamps(StampCollection rootStampCollection){
		Vector3 topPos = new Vector3 (0f, worldHeight / 2f, 0f);
		MakeRectStamp (topPos, worldWidth, 3f, rootStampCollection);
		MakeImpassableStamp (topPos, worldWidth, 5f, rootStampCollection);

		Vector3 botPos = new Vector3 (0f, -worldHeight / 2f, 0f);
		MakeRectStamp (botPos, worldWidth, 3f, rootStampCollection);
		MakeImpassableStamp (botPos, worldWidth, 5f, rootStampCollection);

		Vector3 rightPos = new Vector3 (worldWidth / 2f, 0f, 0f);
		MakeRectStamp (rightPos, 3f, worldHeight, rootStampCollection);
		MakeImpassableStamp (rightPos, 10f, worldHeight, rootStampCollection);

		Vector3 leftPos = new Vector3 (-worldWidth / 2f, 0f, 0f);
		MakeRectStamp (leftPos, 3f, worldHeight, rootStampCollection);
		MakeImpassableStamp (leftPos, 10f, worldHeight, rootStampCollection);
	}

	void MakeRectStamp(Vector3 location, float width, float height, StampCollection rootStampCollection){
		width = width + 0.1f;
		height = height + 0.1f;
		GameObject newRectStamp = Instantiate (rectSolidStampPrefab, location, Quaternion.identity);
		newRectStamp.transform.localScale = new Vector3 (width, height, 1f);
		rootStampCollection.AddChildStamp (newRectStamp);
	}

	void MakeImpassableStamp(Vector3 location, float width, float height, StampCollection rootStampCollection){
		width = width + 0.1f;
		height = height + 0.1f;
		GameObject newRectStamp = Instantiate (impassableStampPrefab, location, Quaternion.identity);
		newRectStamp.transform.localScale = new Vector3 (width, height, 1f);
		rootStampCollection.AddChildStamp (newRectStamp);
	}
}
