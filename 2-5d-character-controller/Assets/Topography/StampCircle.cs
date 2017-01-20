using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampCircle : MonoBehaviour, IStampable {

	public bool isSolid = true;
	Vector2 position;
	float radius;


	public void ApplyStamp(MarchingSquaresGrid marchingGrid){
		print ("Apply stamp invoked in stamp");

		TransformToCircle ();

		new MarchingSquaresCutTools (marchingGrid.GetNodeArray ()).DigCircleFromWorld (position, radius, isSolid);
		//Destroy (this.gameObject);
	}

	private void TransformToCircle(){
		position = new Vector2 (this.transform.position.x, this.transform.position.y);
		radius = this.transform.localScale.x/2f;
	}
}
