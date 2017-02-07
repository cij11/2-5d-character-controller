using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampOreRect : MonoBehaviour, IStampable {

	public int oreType = 1;
	Vector2 position;
	float angle;
	float width;
	float height;


	public void ApplyStamp(MarchingSquaresGrid marchingGrid, OreGrid oreGrid){
		TransformToRect ();
	//	Vector2 position2d = new Vector2 (this.transform.position.x, this.transform.position.y);
	//	QuadToHulls quad = new QuadToHulls(position2d, 0f, 20f, 20f);

		QuadToHulls quad = new QuadToHulls (position, angle, width, height);
	
		new OreStampTools (oreGrid).StampOreConvexHullFromWorld (quad.GetUpperHull (), quad.GetLowerHull (), oreType);
		Destroy (this.gameObject);
	}

	private void TransformToRect(){
		position = new Vector2 (this.transform.position.x - 0.5f, this.transform.position.y - 0.5f);
		angle = this.transform.eulerAngles.z;
		width = this.transform.lossyScale.x;
		height = this.transform.lossyScale.y;
	}
}
