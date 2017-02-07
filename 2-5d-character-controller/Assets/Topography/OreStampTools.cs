using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreStampTools : CutTools {
	int[,] terrainMap;
	public OreStampTools(OreGrid oreGrid){
		terrainMap = (oreGrid).GetTerrainMap ();
		tileXSize = terrainMap.GetLength (0);
		tileYSize = terrainMap.GetLength (1);
	}

	//Step along an upper and lower hull of a convex shape. Set all the nodes between these two hulls
	//equal to the given elevation.
	public void StampOreConvexHullFromWorld(List<Vector2> topHull, List<Vector2> botHull, int oreType){
		List<Vector2> topHullInGridSpace = ConvertHullToGridSpace (topHull);
		List<Vector2> botHullInGridSpace = ConvertHullToGridSpace (botHull);

		BulkAndFineHorizontalCutsPass (topHullInGridSpace, botHullInGridSpace, oreType);
	}
	
	private void BulkAndFineHorizontalCutsPass(List<Vector2> topHull, List<Vector2> botHull, int oreType){

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

			BulkFillOreColumnSpan(i, botIntersection, topIntersection, oreType);
		}
	}

	void BulkFillOreColumnSpan(int column, float botOfSpan, float topOfSpan, int oreType){
		int topNode = Mathf.FloorToInt (topOfSpan);
		int botNode = Mathf.CeilToInt (botOfSpan);

		if (topNode > tileYSize)
			topNode = tileYSize;
		if (botNode < 0)
			botNode = 0;

		for (int j = botNode + 1; j < topNode; j++) {
			terrainMap [column, j] = oreType;
		}
	}
}
