using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthRoom {
	Vector3 position;
	List<MovementDirection> corridorDirections;

	public LabyrinthRoom(Vector3 position){
		this.position = position;
		corridorDirections = new List<MovementDirection> ();
		corridorDirections.Add (MovementDirection.RIGHT);
		corridorDirections.Add (MovementDirection.LEFT);
		corridorDirections.Add (MovementDirection.UP);
		corridorDirections.Add (MovementDirection.DOWN);
	}

	public bool HasUnexploredDirection(){
		if (corridorDirections.Count != 0) {
			return true;
		} else {
			return false;
		}
	}

	//Pop a random direction from the set of 4 possibilities and return it
	public MovementDirection PopRandomUnexploredDirection(){
		int randomIndex = Random.Range (0, corridorDirections.Count);
		MovementDirection selectedDirection = corridorDirections [randomIndex];
		corridorDirections.RemoveAt (randomIndex);
		return selectedDirection;
	}

	public Vector3 GetPosition(){
		return position;
	}

	public Vector3 DirectionToVector(MovementDirection direction){
		Vector3 vec = new Vector3 (1f, 0f, 0f);
		Vector3 directionFromCenter = position;
		directionFromCenter.Normalize ();
		Vector3 tangentVec = new Vector3 (directionFromCenter.y, -directionFromCenter.x, 0);

		switch (direction) {
		case MovementDirection.RIGHT:
			vec = tangentVec;
			break;
		case MovementDirection.LEFT:
			vec = -tangentVec;
			break;
		case MovementDirection.UP:
			vec = directionFromCenter;
			break;
		case MovementDirection.DOWN:
			vec = -directionFromCenter;
			break;
		}
		return vec;
	}
}
