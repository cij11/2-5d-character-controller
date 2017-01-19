using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Take a quad (position, rotation, height, width) build upper and lower convex hulls
public class QuadToHulls {
	List<Vector2> upperHull;
	List<Vector2> lowerHull;

	public QuadToHulls(Vector2 position, float angle, float width, float height){

//		X = x*cos(θ) - y*sin(θ)
//			Y = x*sin(θ) + y*cos(θ)

		angle = angle * Mathf.Deg2Rad;

		float halfWidth = width / 2f;
		float halfHeight = height / 2f;
		//Find which corner is highest. Knowing this, the two adjacent corners make up the upper hull

		Vector2 topLeft = rotateVector (-halfWidth, halfHeight, angle);
		Vector2 topRight = rotateVector (halfWidth, halfHeight, angle);
		Vector2 botLeft = rotateVector (-halfWidth, -halfHeight, angle);
		Vector2 botRight = rotateVector (halfWidth, -halfHeight, angle);

		QuadCorner highestCorner = QuadCorner.TOP_LEFT;
		Vector2 highestVector = topLeft;
		Vector2 lowestVector = botRight;
		Vector2 leftMostVector = botLeft;
		Vector2 rightMostVector = topRight;

		if (topRight.y > highestVector.y) {
			highestVector = topRight;
			highestCorner = QuadCorner.TOP_RIGHT;
		}
		if (botLeft.y > highestVector.y) {
			highestVector = botLeft;
			highestCorner = QuadCorner.BOT_LEFT;
		}
		if (botRight.y > highestVector.y) {
			highestVector = botRight;
			highestCorner = QuadCorner.BOT_RIGHT;
		}

		switch (highestCorner) {
		case QuadCorner.TOP_LEFT:
			leftMostVector = botLeft;
			rightMostVector = topRight;
			lowestVector = botRight;
			break;
		case QuadCorner.TOP_RIGHT:
			leftMostVector = topLeft;
			rightMostVector = botRight;
			lowestVector = botLeft;
			break;
		case QuadCorner.BOT_LEFT:
			leftMostVector = botRight;
			rightMostVector = topLeft;
			lowestVector = topRight;
			break;
		case QuadCorner.BOT_RIGHT:
			leftMostVector = topRight;
			rightMostVector = botLeft;
			lowestVector = topLeft;
			break;
		}

		highestVector = highestVector + position;
		lowestVector = lowestVector + position;
		leftMostVector = leftMostVector + position;
		rightMostVector = rightMostVector + position;

		upperHull = new List<Vector2> ();
		lowerHull = new List<Vector2> ();

		upperHull.Add (leftMostVector);
		upperHull.Add (highestVector);
		upperHull.Add (rightMostVector);

		lowerHull.Add (leftMostVector);
		lowerHull.Add (lowestVector);
		lowerHull.Add (rightMostVector);
	}

	private Vector2 rotateVector(float x, float y, float angle){
		return new Vector2(x*Mathf.Cos(angle) - y *Mathf.Sin(angle), x*Mathf.Sin(angle) + y * Mathf.Cos(angle));
	}

	public List<Vector2> GetUpperHull(){
		return upperHull;
	}

	public List<Vector2> GetLowerHull(){
		return lowerHull;
	}
}

public enum QuadCorner{
	TOP_LEFT, TOP_RIGHT, BOT_LEFT, BOT_RIGHT
}