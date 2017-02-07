using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutTools{
	protected int tileXSize;
	protected int tileYSize;

	protected List<Vector2> ConvertHullToGridSpace(List<Vector2> hull){
		List<Vector2> hullInGridSpace = new List<Vector2> ();
		Vector2 gridOffset = new Vector2 ((float)tileXSize / 2f, (float)tileYSize / 2f);
		foreach (Vector2 vertice in hull) {
			hullInGridSpace.Add(vertice + gridOffset);
		}
		return hullInGridSpace;
	}

	protected float HullSegmentGradient(List<Vector2> hull, int index){
		return (hull [index + 1].y - hull [index].y) / (hull [index + 1].x - hull [index].x);
	}

	//Advance to the next index in the hull if the column index is past the end of the current line segment
	protected bool CheckLineEnd(List<Vector2> hull, int hullIndex, int columnIndex){
		if (columnIndex > hull [hullIndex + 1].x) {
			return true;
		} else {
			return false;
		}
	}
}
