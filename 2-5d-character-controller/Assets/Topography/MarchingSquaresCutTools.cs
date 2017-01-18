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
	public void DigConvexHull(List<Vector2> topHull, List<Vector2> botHull, float elevation){
		BulkAndFineHorizontalCutsPass (topHull, botHull, elevation);
		FineVerticalCutsPass (topHull, botHull, elevation);
	}

	private void BulkAndFineHorizontalCutsPass(List<Vector2> topHull, List<Vector2> botHull, float elevation){
		float startX = topHull [0].x;
		startX = Mathf.Max (startX, 0);
		float endX = topHull [topHull.Count - 1].x;
		endX = Mathf.Min (endX, tileXSize);

		int topHullIndex = 0;
		int botHullIndex = 0;
		float topGradient = HullSegmentGradient (topHull, topHullIndex);
		float botGradient = HullSegmentGradient (botHull, botHullIndex);

		for (int i = Mathf.CeilToInt (startX); i < Mathf.FloorToInt (endX); i++) {
			if (CheckLineEnd (topHull, topHullIndex, i)) {
				topHullIndex = topHullIndex + 1;
				topGradient = HullSegmentGradient (topHull, topHullIndex);
			}
			if (CheckLineEnd (botHull, botHullIndex, i)) {
				botHullIndex = botHullIndex + 1;
				botGradient = HullSegmentGradient (botHull, botHullIndex);
			}

			float topIntersection = topGradient * ((float)i - topHull [topHullIndex].x) + topHull [topHullIndex].y; //mx + c
			float botIntersection = botGradient * ((float)i - botHull [botHullIndex].x) + botHull [botHullIndex].y;

			SetColumnSpanToElevation (i, botIntersection, topIntersection, elevation);
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

	void SetColumnSpanToElevation(int column, float botOfSpan, float topOfSpan, float elevation){
		int topNode = Mathf.FloorToInt (topOfSpan);
		float topOverlap = topOfSpan - topNode;

		int botNode = Mathf.CeilToInt (botOfSpan);
		float botOverlap = Mathf.Abs(botOfSpan - botNode);

		//Nodes on the boundry transition between elevation and the surrounding terrain, 
		//so are set to get the correct topographic line, not the correct elevation.

		//If need to push the topography further than 0.5, set current node to max, and lift adjacent node
		if (topOverlap > 0.5f) {
			nodeArray [column, topNode] = 1f;
			nodeArray [column, topNode + 1] = InverseInterpolationOneToB (topOverlap);
		} else {
			nodeArray [column, topNode] = InverseInterpolationAToZero (topOverlap);
		}
		nodeArray [column, botNode] = InverseInterpolationAToZero (botOverlap);

		if (botOverlap > 0.5f) {
			nodeArray [column, botNode] = 1f;
			nodeArray [column, botNode - 1] = InverseInterpolationOneToB (botOverlap);
		} else {
			nodeArray [column, botNode] = InverseInterpolationAToZero (botOverlap);
		}

		for (int j = botNode + 1; j < topNode; j++) {
			nodeArray [column, j] = elevation;
		}
	}

	//Push out from one node, assuming other node 0
	float InverseInterpolationAToZero(float targetTopographic){
		return (0.5f) / (1f - targetTopographic);
	}

	//Pull in from another node, assuming pulling from 1 node.
	float InverseInterpolationOneToB(float targetTopograpohic){
		return (1f - (0.5f / targetTopograpohic));
	}

	void SetColumnToElevation(int column, float elevation){
		for (int j = 0; j < tileYSize; j++) {
			nodeArray [column, j] = elevation;
		}
	}

	private void FineVerticalCutsPass(List<Vector2> topHull, List<Vector2> botHull, float elevation){
		//Ascending top hull
		for (int topIndex = 0;  topIndex < topHull.Count - 1; topIndex++) {
			float topGradient = HullSegmentGradient (topHull, topIndex);
			if (topGradient > 1) { //if this is steep
				int startY = Mathf.CeilToInt(topHull[topIndex].y);
				int endY = Mathf.FloorToInt (topHull [topIndex + 1].y);
				for (int j = startY; j < endY + 1; j++) {
					float xIntersect = topHull [topIndex].x + ((float)j - topHull [topIndex].y) / topGradient;
					int nearestIncludedNode = Mathf.CeilToInt (xIntersect);
					float overlap = nearestIncludedNode - xIntersect;
					nodeArray [nearestIncludedNode, j] = overlap;
				}
			}
		}

		//Ascending bot hull
		for (int botIndex = 0; botIndex < botHull.Count - 1; botIndex++) {
			float botGradient = HullSegmentGradient (botHull, botIndex);
			if (botGradient > 1) { //if this is steep
				int startY = Mathf.CeilToInt(botHull[botIndex].y);
				int endY = Mathf.FloorToInt (botHull [botIndex + 1].y);
				for (int j = startY; j < endY + 1; j++) {
					float xIntersect = botHull [botIndex].x + ((float)j - botHull [botIndex].y) / botGradient;
					int nearestIncludedNode = Mathf.FloorToInt (xIntersect);
					float overlap = xIntersect - nearestIncludedNode;
					nodeArray [nearestIncludedNode, j] = overlap;
				}
			}
		}
	}
}