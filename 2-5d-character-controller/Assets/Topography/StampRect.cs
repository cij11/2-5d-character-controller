using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampRect : MonoBehaviour, IStampable {

	public bool isSolid = true;
	Vector2 position;
	float angle;
	float width;
	float height;


	public void ApplyStamp(MarchingSquaresGrid marchingGrid){
		TransformToRect ();
	//	Vector2 position2d = new Vector2 (this.transform.position.x, this.transform.position.y);
	//	QuadToHulls quad = new QuadToHulls(position2d, 0f, 20f, 20f);

		QuadToHulls quad = new QuadToHulls (position, angle, width, height);
	
		new MarchingSquaresCutTools (marchingGrid.GetNodeArray()).DigConvexHullFromWorld (quad.GetUpperHull (), quad.GetLowerHull (), isSolid);
		Destroy (this.gameObject);
	}

	private void TransformToRect(){
		position = new Vector2 (this.transform.position.x - 0.5f, this.transform.position.y - 0.5f);
		angle = this.transform.eulerAngles.z;
		width = this.transform.lossyScale.x;
		height = this.transform.lossyScale.y;
	}
}
