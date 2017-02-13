using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampCircle : MonoBehaviour, IStampable {

	public bool isSolid = true;
	Vector2 position;
	float radius;


	public void ApplyStamp(MarchingSquaresGrid marchingGrid, OreGrid oreGrid){
		TransformToCircle ();

		new MarchingSquaresCutTools (marchingGrid).DigCircleFromWorld (position, radius, isSolid);
	//	Topography topography = (Topography)FindObjectOfType (typeof(Topography));
	//	topography.DigCircle (this.transform.position, radius, isSolid);
		Destroy (this.gameObject);
	}

	private void TransformToCircle(){
		position = new Vector2 (this.transform.position.x, this.transform.position.y);
		radius = this.transform.lossyScale.x/2f;
	}
}
