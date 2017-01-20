using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarchingSquaresGrid : MonoBehaviour {
	public int tileXSize = 10;
	public int tileYSize = 10;

	private int nodeXSize;
	private int nodeYSize;

	private float[,] nodeArray;
	private float[,] horizInterpArray;
	private float[,] vertInterpArray;

	// Use this for initialization
	void Start () {

	}
	public void Initialise(int worldSizeX, int worldSizeY, float vesselRadius){
			tileXSize = worldSizeX;
			tileYSize = worldSizeY;
			nodeXSize = tileXSize+1;
			nodeYSize = tileYSize + 1;

			nodeArray = new float[nodeXSize, nodeYSize];
			horizInterpArray = new float[tileXSize, nodeYSize];
			vertInterpArray = new float[nodeXSize, tileYSize];

		float defaultElevation = 1f;

			int i = 0;
			int j = 0;
			for (i = 0; i < nodeXSize-1; i++){
				for (j = 0; j < nodeYSize-1; j++){
				//	nodeArray [i, j] = Mathf.PerlinNoise (i * perlinResolution, j * perlinResolution) * 0.9f;
				nodeArray[i,j] = defaultElevation;
				}
			}

	//	DigCircle ((float)worldSizeX/2f, (float)worldSizeY/2f, vesselRadius - perimeterBuffer, true);
	//	DigPerlinTunnels (perlinResolution);
	//	StampAxisAlignedRect ((int)worldSizeX/2, (int)worldSizeY/2, 35, 35, 0f);
	//	DigPerlinCaves (perlinResolution);
		DigTestConvexHull(false);


			for (i = 0; i < tileXSize; i++){
				for (j = 0; j < nodeYSize; j++){
					horizInterpArray [i, j] = 0.5f;
				}
			}
			for (i = 0; i < nodeXSize; i++){
				for (j = 0; j < tileYSize; j++){
					vertInterpArray [i, j] = 0.5f;
				}
			}

			InterpolateAllHorizontal ();
			InterpolateAllVertical ();
	}
		
	public void StampAxisAlignedRect(int startX, int startY, int width, int height, float elevation){
		RecieveStamp (BuildRectStamp (width, height, elevation), startX, startX);
	}

	public float [,] BuildRectStamp(int width, int height, float elevation){
		float[,] stampNodeArray = new float[width, height];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				stampNodeArray [i, j] = elevation;
			}
		}
		return stampNodeArray;
	}

	//Apply the source marching grid stamp to the destination marching grid
	public void RecieveStamp(float[,] stampNodeArray, int startLeftX, int startBotY){
		//Loop through the stamp from the bottom left (0, 0) to the top right (stampsizeX, stampsizeY)
		//Loop through the marching squares grid from startinb point (startLeftX, startBottomY) to the
		//top right (startLeftX + stampsizeX, startBottomY + stampSizeY)
		int destStartX = startLeftX;
		int destStartY = startBotY;
		int destEndX = startLeftX + stampNodeArray.GetLength (0);
		int destEndY = startBotY + stampNodeArray.GetLength (1);

		//Limit bounds of stamping to the bounds of the marching squares grid
		if (destEndX > nodeXSize) destEndX = nodeXSize;
		if (destEndY > nodeYSize)
			destEndY = nodeYSize;

		for (int i = destStartX; i < destEndX; i++) {
			for (int j = destStartY; j < destEndY; j++) {
				int stampX = i - destStartX;
				int stampY = j - destStartY;

				nodeArray [i, j] = stampNodeArray [stampX, stampY];
			}
		}
	}

	//Test function. Construct a simple upper and lower hull. Call DigConvexHull.
	public void DigTestConvexHull(bool isSolid){
		List<Vector2> topHull = new List<Vector2> ();
		List<Vector2> botHull = new List<Vector2> ();

/*	//	new MarchingSquaresCutTools(nodeArray).DigConvexHull (topHull, botHull, isSolid);
		for (int i = 0; i < 170; i += 10) {
			QuadToHulls quadHulls = new QuadToHulls (new Vector2 (-20.01f + i*3, 100.5f), 0, 10f, 20f);
			new MarchingSquaresCutTools (nodeArray).DigConvexHull (quadHulls.GetUpperHull (), quadHulls.GetLowerHull (), isSolid);
		}*/

		for (int i = 0; i < 170; i += 10) {
			QuadToHulls quadHulls = new QuadToHulls (new Vector2 (-99.01f + i*3, 50.5f), i * 3, 10f, 20f);
			new MarchingSquaresCutTools (nodeArray).DigConvexHullFromLocal (quadHulls.GetUpperHull (), quadHulls.GetLowerHull (), isSolid);
		}
	}

	public float GetNode(int x, int y){
		return nodeArray [x, y];
	}
	public float GetHoriz(int x, int y){
		return horizInterpArray [x, y];
	}
	public float GetVert(int x, int y){
		return vertInterpArray [x, y];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void InterpolateAllHorizontal(){
		int i = 0;
		int j = 0;

		for (i = 0; i < tileXSize; i++){
			for (j = 0; j < nodeYSize; j++){
				if (nodeArray [i, j] < 0.5f && nodeArray [i + 1, j] < 0.5f) {
					horizInterpArray [i, j] = MidPoint (nodeArray [i, j], nodeArray [i + 1, j]);
				} else if (nodeArray [i, j] > 0.5f && nodeArray [i + 1, j] > 0.5f) {
					horizInterpArray [i, j] =  MidPoint (nodeArray [i, j], nodeArray [i + 1, j]);
				}
				else
					horizInterpArray [i, j] = InterpolateEdge (nodeArray [i, j], nodeArray [i + 1, j]);
				//	horizInterpArray[i,j] = 0.5f;
			}
		}
	}

	private void InterpolateAllVertical(){
		int i = 0;
		int j = 0;

		for (i = 0; i < nodeXSize; i++){
			for (j = 0; j < tileYSize; j++){
				if (nodeArray [i, j] < 0.5f && nodeArray [i, j + 1] < 0.5f) {
					vertInterpArray [i, j] =  MidPoint (nodeArray [i, j], nodeArray [i, j+1]);
				} else if (nodeArray [i, j] > 0.5f && nodeArray [i, j + 1] > 0.5f) {
					vertInterpArray [i, j] =  MidPoint (nodeArray [i, j], nodeArray [i, j+1]);
				}
				else
					vertInterpArray [i, j] = InterpolateEdge (nodeArray [i, j], nodeArray [i, j+1]);
				//	vertInterpArray[i,j] = 0.5f;
			}
		}
	}

	public void InterpolateAll(){
		InterpolateAllHorizontal ();
		InterpolateAllVertical ();
	}

	public void InterpolateAllInRange(int xmin, int ymin, int xmax, int ymax){
		xmin = (int)Mathf.Max (0, xmin);
		ymin = (int)Mathf.Max (0, ymin);
		xmax = (int)Mathf.Min (nodeXSize, xmax);
		ymax = (int)Mathf.Min (nodeYSize, ymax);

		int i = 0;
		int j = 0;

		//Horizontal edges
		for (i = xmin; i < xmax - 1; i++){
			for (j = ymin; j < ymax; j++){
				if (nodeArray [i, j] < 0.5f && nodeArray [i + 1, j] < 0.5f) {
					horizInterpArray [i, j] = MidPoint (nodeArray [i, j], nodeArray [i + 1, j]);
				} else if (nodeArray [i, j] > 0.5f && nodeArray [i + 1, j] > 0.5f) {
					horizInterpArray [i, j] =  MidPoint (nodeArray [i, j], nodeArray [i + 1, j]);
				}
				else
					horizInterpArray [i, j] = InterpolateEdge (nodeArray [i, j], nodeArray [i + 1, j]);
			}
		}

		//Vertical edges
		for (i = xmin; i < xmax; i++){
			for (j = ymin; j < ymax-1; j++){
				if (nodeArray [i, j] < 0.5f && nodeArray [i, j + 1] < 0.5f) {
					vertInterpArray [i, j] =  MidPoint (nodeArray [i, j], nodeArray [i, j+1]);
				} else if (nodeArray [i, j] > 0.5f && nodeArray [i, j + 1] > 0.5f) {
					vertInterpArray [i, j] =  MidPoint (nodeArray [i, j], nodeArray [i, j+1]);
				}
				else
					vertInterpArray [i, j] = InterpolateEdge (nodeArray [i, j], nodeArray [i, j+1]);
			}
		}
	}

	//Finds the point that a line from one node to another would intersect height 0.5
	private float InterpolateEdge(float a, float b){
		return (0.5f - a) / (b - a);
	}

	//Returns a weighted point between the two nodes. -ve, to distinguish from 0.5 intersections.
	private float MidPoint(float a, float b){
	//	return -(a + b) / 2f;
		return -0.5f;
	}

	public float[,] GetNodeArray(){
		return nodeArray;
	}

/*	void OnDrawGizmos() {
		int i = 0;
		int j = 0;
		for (i = 0; i < nodeXSize; i++) {
			for (j = 0; j < nodeYSize; j++) {
				if (nodeArray [i, j] < 0.5f)
					Gizmos.color = Color.yellow;
				else
					Gizmos.color = Color.green;
				Gizmos.DrawSphere (new Vector3 (i, j, 0f), 0.2f * nodeArray [i, j]);
			}
		}
		//Render horizontal interpolations at the correct point
		for (i = 0; i < tileXSize; i++) {
			for (j = 0; j < nodeYSize; j++) {
				Gizmos.color = Color.red;
				if (horizInterpArray[i,j] > 0)
					Gizmos.DrawSphere (new Vector3 (i + horizInterpArray [i, j], j, 0f), 0.1f);
			}
		}
		//Render vertical interpolations at the correct point.
		for (i = 0; i < nodeXSize; i++) {
			for (j = 0; j < tileYSize; j++) {
				Gizmos.color = Color.blue;
				if (vertInterpArray[i,j] > 0)
					Gizmos.DrawSphere (new Vector3 (i, j + vertInterpArray [i, j], 0f), 0.1f);
			}
		}
	}*/

}
