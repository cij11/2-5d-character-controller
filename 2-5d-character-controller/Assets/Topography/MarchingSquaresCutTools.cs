using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingSquaresCutTools {

	int tileXSize;
	int tileYSize;
	float[,] nodeArray;

	public MarchingSquaresCutTools(float[,] marchingSquaresNodeArray){
		nodeArray = marchingSquaresNodeArray;
		tileXSize = nodeArray.GetLength (0);
		tileYSize = nodeArray.GetLength (1);
	}
	//Step along an upper and lower hull of a convex shape. Set all the nodes between these two hulls
	//equal to the given elevation.
	public void DigConvexHull(List<Vector2> topHull, List<Vector2> botHull, bool isSolid){
		BulkAndFineHorizontalCutsPass (topHull, botHull, isSolid);
		FineVerticalCutsPass (topHull, botHull, isSolid);
	}

	private void BulkAndFineHorizontalCutsPass(List<Vector2> topHull, List<Vector2> botHull, bool isSolid){
		float elevation = 0f;
		if (isSolid)
			elevation = 1f;
		
		float startX = topHull [0].x;
		startX = Mathf.Max (startX, 0);
		float endX = topHull [topHull.Count - 1].x;
		endX = Mathf.Min (endX, tileXSize);

		int topHullIndex = 0;
		int botHullIndex = 0;
		float topGradient = HullSegmentGradient (topHull, topHullIndex);
		float botGradient = HullSegmentGradient (botHull, botHullIndex);

		for (int i = Mathf.CeilToInt (startX); i < Mathf.FloorToInt (endX); i++) {
			while (CheckLineEnd (topHull, topHullIndex, i)) {
				topHullIndex = topHullIndex + 1;
				topGradient = HullSegmentGradient (topHull, topHullIndex);
			}
			while (CheckLineEnd (botHull, botHullIndex, i)) {
				botHullIndex = botHullIndex + 1;
				botGradient = HullSegmentGradient (botHull, botHullIndex);
			}

			float topIntersection = topGradient * ((float)i - topHull [topHullIndex].x) + topHull [topHullIndex].y; //mx + c
			float botIntersection = botGradient * ((float)i - botHull [botHullIndex].x) + botHull [botHullIndex].y;

			BulkFillColumnSpan(i, botIntersection, topIntersection, elevation);
			SetColumnNodeToVertOverlap(i, topIntersection, elevation, 1);
			SetColumnNodeToVertOverlap(i, botIntersection, elevation, -1);
		}
	}

	private float HullSegmentGradient(List<Vector2> hull, int index){
		return (hull [index + 1].y - hull [index].y) / (hull [index + 1].x - hull [index].x);
	}

	//Advance to the next index in the hull if the column index is past the end of the current line segment
	private bool CheckLineEnd(List<Vector2> hull, int hullIndex, int columnIndex){
		if (columnIndex > hull [hullIndex + 1].x) {
			return true;
		} else {
			return false;
		}
	}

	void BulkFillColumnSpan(int column, float botOfSpan, float topOfSpan, float elevation){
		int topNode = Mathf.FloorToInt (topOfSpan);
		int botNode = Mathf.CeilToInt (botOfSpan);

		for (int j = botNode + 1; j < topNode; j++) {
			nodeArray [column, j] = elevation;
		}
	}

	void SetColumnNodeToVertOverlap (int column, float intersection, float elevation, int direction){
		if (direction == 1) { //If the cut overshoots above
			int row = Mathf.FloorToInt (intersection);
			float overlap = intersection - row;
			nodeArray [column, row] = Mathf.Abs(overlap + elevation - 1);
		} else {
			int row = Mathf.CeilToInt (intersection);
			float overlap = (float)row - intersection;
			nodeArray [column, row] = Mathf.Abs(overlap + elevation - 1);
		}
	}

	void SetRowNodeToHorizontalOverlap(int row, float intersection, float elevation, int direction){
		if (direction == 1) {  //If the cut overshoots to the right
			int column = Mathf.FloorToInt (intersection);
			float overlap = intersection - column;
			nodeArray [column, row] = Mathf.Abs(overlap + elevation - 1);
		} else {
			int column = Mathf.CeilToInt (intersection);
			float overlap = (float)column - intersection;
			nodeArray [column, row] = Mathf.Abs(overlap + elevation - 1);
		}
	}

	void SetColumnToElevation(int column, float elevation){
		for (int j = 0; j < tileYSize; j++) {
			nodeArray [column, j] = elevation;
		}
	}

	private void FineVerticalCutsPass(List<Vector2> topHull, List<Vector2> botHull, bool isSolid){
		float elevation = 0f;
		if (isSolid)
			elevation = 1f;

		//Top hull
		for (int topIndex = 0;  topIndex < topHull.Count - 1; topIndex++) {
			float topGradient = HullSegmentGradient (topHull, topIndex);
			int startY = Mathf.CeilToInt(topHull[topIndex].y);
			int endY = Mathf.FloorToInt (topHull [topIndex + 1].y);
			if (topGradient > 1) { //if this is steep
				for (int j = startY; j < endY + 1; j++) { //Advance from bottom to top
					float xIntersect = topHull [topIndex].x + ((float)j - topHull [topIndex].y) / topGradient;
					SetRowNodeToHorizontalOverlap (j, xIntersect, elevation, -1);
				}
			}

			if (topGradient < -1) { //if this is steeply descending
				for (int j = startY; j > endY + 1; j--) { //Advance from top to bottom
					float xIntersect = topHull [topIndex].x + ((float)j - topHull [topIndex].y) / topGradient;
					SetRowNodeToHorizontalOverlap (j, xIntersect, elevation, 1);
				}
			}
		}

		//Bot hull
		for (int botIndex = 0; botIndex < botHull.Count - 1; botIndex++) {
			float botGradient = HullSegmentGradient (botHull, botIndex);
			int startY = Mathf.CeilToInt(botHull[botIndex].y);
			int endY = Mathf.FloorToInt (botHull [botIndex + 1].y);
			if (botGradient > 1) { //if this is steep
				for (int j = startY; j < endY + 1; j++) {
					float xIntersect = botHull [botIndex].x + ((float)j - botHull [botIndex].y) / botGradient;
					SetRowNodeToHorizontalOverlap (j, xIntersect, elevation, 1);
				}
			}
			if (botGradient < -1) { //if this is steeply descending
				for (int j = startY; j > endY + 1; j--) { //Advance from top to bottom
					float xIntersect = topHull [botIndex].x + ((float)j - topHull [botIndex].y) / botGradient;
					SetRowNodeToHorizontalOverlap (j, xIntersect, elevation, -1);
				}
			}
		}
	}
}