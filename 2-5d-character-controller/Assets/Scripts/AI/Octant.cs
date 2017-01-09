using UnityEngine;
using System.Collections;

public class Octant {

	public static void PointsToOctant(Vector3 fromPoint, Vector3 toPoint, Vector3 right, Vector3 up, out float hor, out float vert){
		Vector3 vectorToPoint = toPoint - fromPoint;
		float horDist = Vector3.Dot(vectorToPoint, right);
		float vertDist = Vector3.Dot(vectorToPoint, up);

		float radAngle = Mathf.Atan(Mathf.Abs(vertDist)/Mathf.Abs(horDist));
		float angle = Mathf.Rad2Deg * radAngle;

		if (angle < 22.5f){ //Horizontal octant
			hor = Mathf.Sign(horDist);
			vert = 0f;
		}
		else if (angle >= 22.5 && angle < 67.5){ //Diagonal octants
			hor = (Mathf.Sign(horDist));
			vert = (Mathf.Sign(vertDist));
		}
		else{ //Vertical octants
			hor = 0f;
			vert = (Mathf.Sign(vertDist));
		}
	}
}
