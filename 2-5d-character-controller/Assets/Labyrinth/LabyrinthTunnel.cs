using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthTunnel {

	Vector3 start;
	Vector3 end;
	Vector3 position;
	float length;
	float width;

	public LabyrinthTunnel(Vector3 start, Vector3 end, float width){
		this.start = start;
		this.end = end;
		this.width = width;

		this.position = (start + end) / 2f;
		this.length = (start - end).magnitude + 4f;
	}

	public Vector3 GetStart(){
		return start;
	}

	public Vector3 GetEnd(){
		return end;
	}

	public Vector3 GetPosition(){
		return position;
	}

	public float GetLength(){
		return length;
	}

	public float GetWidth(){
		return width;
	}
}
