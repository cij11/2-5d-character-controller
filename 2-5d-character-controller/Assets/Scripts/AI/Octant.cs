using UnityEngine;
using System.Collections;

public class Octant {

	public static void PointsToDistances(Vector3 fromPoint, Vector3 toPoint, Vector3 right, Vector3 up, out float horDist, out float vertDist){
		Vector3 vectorToPoint = toPoint - fromPoint;
		horDist = Vector3.Dot(vectorToPoint, right);
		vertDist = Vector3.Dot(vectorToPoint, up);
	}

	public static void PointsToOctant(Vector3 fromPoint, Vector3 toPoint, Vector3 right, Vector3 up, out int horOct, out int vertOct){
		Vector3 vectorToPoint = toPoint - fromPoint;
		float horDist = Vector3.Dot(vectorToPoint, right);
		float vertDist = Vector3.Dot(vectorToPoint, up);

		float radAngle = Mathf.Atan(Mathf.Abs(vertDist)/Mathf.Abs(horDist));
		float angle = Mathf.Rad2Deg * radAngle;

		if (angle < 22.5f){ //Horizontal octant
			horOct = (int)Mathf.Sign(horDist);
			vertOct = 0;
		}
		else if (angle >= 22.5 && angle < 67.5){ //Diagonal octants
			horOct = (int)Mathf.Round(Mathf.Sign(horDist));
			vertOct = (int)Mathf.Round(Mathf.Sign(vertDist));
		}
		else{ //Vertical octants
			horOct = 0;
			vertOct = (int)(Mathf.Sign(vertDist));
		}
	}
}
