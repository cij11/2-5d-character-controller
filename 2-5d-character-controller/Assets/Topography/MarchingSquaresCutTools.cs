using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingSquaresCutTools {

	int tileXSize;
	int tileYSize;
	float[,] nodeArray;

	public float perlinResolution = 0.02f;
	public float perlinThreshold = 0.4f;
	public float perlinTunnelThreshold = 6f;
	public float perimeterBuffer = 0.1f;

	public MarchingSquaresCutTools(float[,] marchingSquaresNodeArray){
		nodeArray = marchingSquaresNodeArray;
		tileXSize = nodeArray.GetLength (0);
		tileYSize = nodeArray.GetLength (1);
	}
	//Step along an upper and lower hull of a convex shape. Set all the nodes between these two hulls
	//equal to the given elevation.
	public void DigConvexHullFromWorld(List<Vector2> topHull, List<Vector2> botHull, bool isSolid){
		List<Vector2> topHullInGridSpace = ConvertHullToGridSpace (topHull);
		List<Vector2> botHullInGridSpace = ConvertHullToGridSpace (botHull);

		BulkAndFineHorizontalCutsPass (topHullInGridSpace, botHullInGridSpace, isSolid);
		FineVerticalCutsPass (topHullInGridSpace, botHullInGridSpace, isSolid);
	}

	public void DigConvexHullFromLocal(List<Vector2> topHull, List<Vector2> botHull, bool isSolid){
		BulkAndFineHorizontalCutsPass (topHull, botHull, isSolid);
		FineVerticalCutsPass (topHull, botHull, isSolid);
	}

	private List<Vector2> ConvertHullToGridSpace(List<Vector2> hull){
		List<Vector2> hullInGridSpace = new List<Vector2> ();
		Vector2 gridOffset = new Vector2 ((float)tileXSize / 2f, (float)tileYSize / 2f);
		foreach (Vector2 vertice in hull) {
			hullInGridSpace.Add(vertice + gridOffset);
		}
		return hullInGridSpace;
	}

	private void BulkAndFineHorizontalCutsPass(List<Vector2> topHull, List<Vector2> botHull, bool isSolid){
		float elevation = 0f;
		if (isSolid)
			elevation = 1f;
		
		float startX = topHull [0].x;
		startX = Mathf.Max (startX, 0);
		float endX = topHull [topHull.Count - 1].x;
		endX = Mathf.Min (endX, tileXSize-1);

		int topHullIndex = 0;
		int botHullIndex = 0;
		float topGradient = HullSegmentGradient (topHull, topHullIndex);
		float botGradient = HullSegmentGradient (botHull, botHullIndex);

		for (int i = Mathf.CeilToInt (startX)+1; i < Mathf.FloorToInt (endX); i++) {
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

			SetColumnNodeToVertOverlap (i, topIntersection, elevation, 1);
			SetColumnNodeToVertOverlap (i, botIntersection, elevation, -1);
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

		if (topNode > tileYSize)
			topNode = tileYSize;
		if (botNode < 0)
			botNode = 0;
		
		for (int j = botNode + 1; j < topNode; j++) {
			nodeArray [column, j] = elevation;
		}
	}

	void SetColumnNodeToVertOverlap (int column, float intersection, float elevation, int direction){
		if (direction == 1) { //If the cut overshoots above
			int row = Mathf.FloorToInt (intersection);
			if (row > 0 && row < tileYSize-1) {
				float overlap = intersection - row;
				SetNodesToAchieveTopography(column, row, column, row+1, elevation, overlap);
			}
		} else {
			int row = Mathf.CeilToInt (intersection);
			if (row > 1 && row < tileYSize) {
				float overlap = (float)row - intersection;
				SetNodesToAchieveTopography(column, row, column, row-1, elevation, overlap);
			}
		}
	}

	void SetRowNodeToHorizontalOverlap(int row, float intersection, float elevation, int direction){
		if (direction == 1) {  //If the cut overshoots to the right
			int column = Mathf.FloorToInt (intersection);
			if (column > 0 && column < tileXSize-1) {
				float overlap = intersection - column;
				SetNodesToAchieveTopography(column, row, column + 1, row, elevation, overlap);
			}
		} else {
			int column = Mathf.CeilToInt (intersection);
			if (column > 1 && column < tileXSize) {
				float overlap = (float)column - intersection;
				SetNodesToAchieveTopography(column, row, column - 1, row, elevation, overlap);
			}
		}
	}

	void SetNodesToAchieveTopography(int acol, int arow, int bcol, int brow, float elevation, float overlap){

		//If adjacent node already on the same side of elevation, set current node to elevation.
		if (elevation >= 0.5) {
			if (nodeArray [bcol, brow] > 0.5) {
				nodeArray [acol, arow] = elevation;
				return;
			}
		}
		if (elevation < 0.5) {
			if (nodeArray [bcol, brow] < 0.5) {
				nodeArray [acol, arow] = elevation;
				return;
			}
		}

		//If this is a solid stamp
		if (elevation >= 0.5) {
			if (overlap > 0.5f) { //If the boundary needs to be pulled as well as pushed.
				nodeArray [acol, arow] = 1f;
				nodeArray [bcol, brow] = InverseInterpolationOneToB (overlap);
			} else {
				nodeArray [acol, arow] = InverseInterpolationAToZero (overlap);
				nodeArray [bcol, brow] = 0f;
			}
			return;
		}

		//If this is an empty stamp, invert the overlap and treat node b as the one pushing the boundry.
		if (elevation < 0.5) {
			overlap = 1f - overlap;  //Just treat the other node as the tall one.
			if (overlap > 0.5f) { //If the boundary needs to be pulled as well as pushed.
				nodeArray [bcol, brow] = 1f;
				nodeArray [acol, arow] = InverseInterpolationOneToB (overlap);
			} else {
				nodeArray [bcol, brow] = InverseInterpolationAToZero (overlap);
				nodeArray [acol, arow] = 0f;
			}
			return;
		}
	}

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

	private void FineVerticalCutsPass(List<Vector2> topHull, List<Vector2> botHull, bool isSolid){
		float elevation = 0f;
		if (isSolid)
			elevation = 1f;

		//Top hull
		for (int topIndex = 0;  topIndex < topHull.Count - 1; topIndex++) {
			float topGradient = HullSegmentGradient (topHull, topIndex);
			int startY = 1;
			int endY = 1;

			if (topGradient > 1){
				startY = Mathf.CeilToInt(topHull[topIndex].y);
				endY = Mathf.FloorToInt (topHull [topIndex + 1].y);
			}
			if (topGradient < -1){
				startY = Mathf.FloorToInt(topHull[topIndex].y);
				endY = Mathf.CeilToInt (topHull [topIndex + 1].y);
			}

			if (startY > tileYSize)
				startY = tileYSize;
			if (startY < 0)
				startY = 0;
			if (endY > tileYSize)
				endY = tileYSize;
			if (endY < 0)
				endY = 0;

			if (topGradient > 1) { //if this is steep
				for (int j = startY; j < endY  + 1; j++) { //Advance from bottom to top
					float xIntersect = topHull [topIndex].x + ((float)j - topHull [topIndex].y) / topGradient;
					SetRowNodeToHorizontalOverlap (j, xIntersect, elevation, -1);
				}
			}

			if (topGradient < -1) { //if this is steeply descending
				for (int j = startY; j > endY - 1; j--) { //Advance from top to bottom
					float xIntersect = topHull [topIndex].x + ((float)j - topHull [topIndex].y) / topGradient;
					SetRowNodeToHorizontalOverlap (j, xIntersect, elevation, 1);
				}
			}
		}

		//Bot hull
		for (int botIndex = 0; botIndex < botHull.Count - 1; botIndex++) {
			float botGradient = HullSegmentGradient (botHull, botIndex);
			int startY = 1;
			int endY = 1;

			if(botGradient > 1){
			 startY = Mathf.CeilToInt(botHull[botIndex].y);
			 endY = Mathf.FloorToInt (botHull [botIndex + 1].y);
			}
			if (botGradient < -1){
				startY = Mathf.FloorToInt(botHull[botIndex].y);
				endY = Mathf.CeilToInt (botHull [botIndex + 1].y);
			}

			if (startY > tileYSize)
				startY = tileYSize;
			if (startY < 0)
				startY = 0;
			if (endY > tileYSize)
				endY = tileYSize;
			if (endY < 0)
				endY = 0;
			
			if (botGradient > 1) { //if this is steep
				for (int j = startY; j < endY + 1; j++) {
					float xIntersect = botHull [botIndex].x + ((float)j - botHull [botIndex].y) / botGradient;
					SetRowNodeToHorizontalOverlap (j, xIntersect, elevation, 1);
				}
			}
			if (botGradient < -1) { //if this is steeply descending
				for (int j = startY; j > endY - 1; j--) { //Advance from top to bottom
					float xIntersect = topHull [botIndex].x + ((float)j - topHull [botIndex].y) / botGradient;
					SetRowNodeToHorizontalOverlap (j, xIntersect, elevation, -1);
				}
			}
		}
	}

	public void DigCircleFromWorld(Vector2 position, float radius, bool isSolid){
		DigCircle (position.x + tileXSize / 2f - 0.5f, position.y + tileYSize / 2f - 0.5f, radius, isSolid);
	}

	//Treat the circle as a hemisphere, then normalise elevation to the radius of the hemisphere.
	public void DigCircle(float cx, float cy, float inputRadius, bool solid){

		//making the cut at a spherical cap 0.525r high in the sphere, so increase the radius
		//to make sure that the circle formed by the plane passing through the cap is of radius
		//'input radius'.
		float radius = (inputRadius / 0.525f) * 0.6f;

		int xmin = (int)Mathf.Max ((int) cx - (int) radius - 2, 0);
		int ymin = (int)Mathf.Max ((int) cy - (int) radius - 2, 0);
		int xmax = (int)Mathf.Min ((int) cx + (int) radius + 2, tileXSize-1);
		int ymax = (int)Mathf.Min ((int) cy + (int) radius + 2, tileYSize-1);

		float radiusSquared = radius * radius;
		for (int i = xmin; i < xmax; i++) {
			for (int j = ymin; j < ymax; j++) {
				float xDistFromCenter = cx - i;
				float yDistFromCenter = cy - j;
				//height of hemisphere at this point
				float baseSquared = xDistFromCenter*xDistFromCenter + yDistFromCenter * yDistFromCenter;
				if (baseSquared > radiusSquared){ //point is outside the hemisphere

				}
				else{
					float hemisphereHeight = Mathf.Sqrt(radiusSquared - baseSquared);
					float hemisphereHeightNormalised = hemisphereHeight - 0.525f*radius;
					//hemisphereHeightNormalised = hemisphereHeightNormalised / radius;

					//if (hemisphereHeight > 1) hemisphereHeight = 1f;
					if (solid) {
						if (nodeArray [i, j] <= 0.5f) { //Don't change nodes that are already at the correct elevation
							nodeArray [i, j] = hemisphereHeightNormalised;
						}
					}
					else {
						if(nodeArray[i, j] >= 0.5f){
						nodeArray [i, j] = 1f - hemisphereHeightNormalised;
						}
					}
				}
			}
		}
	}

	public void DigPerlinCaves(float res){
		for(int i = 0; i < tileXSize; i++){
			for (int j = 0; j < tileYSize; j++){
				float perl = 1f-Mathf.PerlinNoise (i * res, j * res);
				if ((perl < nodeArray [i, j] - 0.2f) && (perl < perlinThreshold))
					//		if (perl < perlinThreshold)
					nodeArray [i, j] = perl;
			}
		}
	}

	public void DigPerlinTunnels(float res){
		for(int i = 0; i < tileXSize; i++){
			for (int j = 0; j < tileYSize; j++){
				float perl = Mathf.PerlinNoise (i * res, j * res);

				//0.5 stays as 0.5
				//0.4 and 0.6 become 0.
				perl = Mathf.Abs(perl-0.5f);
				perl = perl * perlinTunnelThreshold;


				if (perl < nodeArray [i, j] - 0.1f)	//If perl is less than the current node, but the current node is not on the border of a circle or otherwise at a gradient.
					nodeArray [i, j] = perl;
			}
		}
	}
}