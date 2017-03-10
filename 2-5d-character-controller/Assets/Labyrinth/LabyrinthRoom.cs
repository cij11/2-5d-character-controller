using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthRoom {
	Vector3 position;
	float diameter;
	float radius;
	List<MovementDirection> corridorDirections;
	bool radiallyAligned;

	Vector3 upVec;
	Vector3 rightVec;

	public LabyrinthRoom(Vector3 position, float diameter, bool radiallyAligned = false){
		this.position = position;
		this.diameter = diameter;
		this.radius = diameter / 2f;
		this.radiallyAligned = radiallyAligned;

		BuildExplorationDirectionList ();
		CalculateRelativeVectors ();
	}

	//Build a list with the 4 potential directions to explore.
	void BuildExplorationDirectionList(){
		corridorDirections = new List<MovementDirection> ();
		corridorDirections.Add (MovementDirection.RIGHT);
		corridorDirections.Add (MovementDirection.LEFT);
		corridorDirections.Add (MovementDirection.UP);
		corridorDirections.Add (MovementDirection.DOWN);
	}

	//
	void CalculateRelativeVectors(){
		if (radiallyAligned) {
			Vector3 directionFromCenter = position;
			if (directionFromCenter.magnitude < 0.001f) {
				directionFromCenter = new Vector3 (0.1f, 0f, 0f);
			}
			directionFromCenter.Normalize ();
			upVec = directionFromCenter;
			rightVec = new Vector3 (directionFromCenter.y, -directionFromCenter.x, 0);
		} else {
			upVec = new Vector3 (0f, 1f, 0f);
			rightVec = new Vector3 (1f, 0f, 0f);
		}
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

		switch (direction) {
		case MovementDirection.RIGHT:
			vec = rightVec;
			break;
		case MovementDirection.LEFT:
			vec = -rightVec;
			break;
		case MovementDirection.UP:
			vec = upVec;
			break;
		case MovementDirection.DOWN:
			vec = -upVec;
			break;
		}
		return vec;
	}

	//Get the point that a tunnel exploring outwards in the Up, Down, Left, or Right direction should sprout from
	public Vector3 GetOutgoingConnectionPoint(MovementDirection outgoingDirection){
		Vector3 connectionPoint = this.position;

		switch (outgoingDirection) {
		case MovementDirection.RIGHT:
			connectionPoint = position + rightVec * radius;
			break;
		case MovementDirection.LEFT:
			connectionPoint = position - rightVec * radius;
			break;
		case MovementDirection.UP:
			connectionPoint = position + upVec * radius;
			break;
		case MovementDirection.DOWN:
			connectionPoint = position - upVec * radius;
			break;
		}
		return connectionPoint;
	}

	//Get the point that a connecting to a room FROM the right, left, up, or down should attach to.
	public Vector3 GetIncomingConnectionPoint(MovementDirection outgoingDirection){
		Vector3 connectionPoint = this.position;

		switch (outgoingDirection) {
		case MovementDirection.LEFT:
			connectionPoint = position + rightVec * radius;
			break;
		case MovementDirection.RIGHT:
			connectionPoint = position - rightVec * radius;
			break;
		case MovementDirection.DOWN:
			connectionPoint = position + upVec * radius;
			break;
		case MovementDirection.UP:
			connectionPoint = position - upVec * radius;
			break;
		}
		return connectionPoint;
	}

	public float GetRadius(){
		return radius;
	}

	public float GetDiameter(){
		return diameter;
	}

	public Vector3 GetUpVector(){
		return upVec;
	}
}
