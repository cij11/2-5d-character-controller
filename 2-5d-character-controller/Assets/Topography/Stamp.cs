using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour, IStampable {

	public bool isSolid = true;

	public void ApplyStamp(MarchingSquaresGrid marchingGrid){
		print ("Apply stamp invoked in stamp");

		Vector2 position2d = new Vector2 (this.transform.position.x, this.transform.position.y);
		QuadToHulls quad = new QuadToHulls (position2d, 1f, 20f, 20f);
		new MarchingSquaresCutTools (marchingGrid.GetNodeArray()).DigConvexHull (quad.GetUpperHull (), quad.GetLowerHull (), false);
	//	Destroy (this.gameObject);
	}

	//Establish shape

	//Convert transform elements into cutting tool elements (position, radius, width, etc)

	//Apply stamp to MarchingGrid
}
