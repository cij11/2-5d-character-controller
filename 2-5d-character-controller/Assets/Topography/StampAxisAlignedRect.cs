using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampAxisAlignedRect : MonoBehaviour, IStampable {

	public bool isSolid = true;
	public bool sharpCorners = true;
	Vector2 position;
	float angle;
	float width;
	float height;


	public void ApplyStamp(MarchingSquaresGrid marchingGrid){
		TransformToRect ();
		//	Vector2 position2d = new Vector2 (this.transform.position.x, this.transform.position.y);
		//	QuadToHulls quad = new QuadToHulls(position2d, 0f, 20f, 20f);

		QuadToHulls quad = new QuadToHulls (position, angle, width, height);

		new MarchingSquaresCutTools (marchingGrid.GetNodeArray ()).DigAxisAlignedRectFromWorld (position, width, height, isSolid, sharpCorners);

		Destroy (this.gameObject);
	}

	private void TransformToRect(){
		position = new Vector2 (this.transform.position.x, this.transform.position.y);
		angle = this.transform.eulerAngles.z;
		width = this.transform.lossyScale.x;
		height = this.transform.lossyScale.y;
	}
}
