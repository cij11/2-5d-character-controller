using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarchingSquaresGrid : MonoBehaviour {
	public int tileXSize = 10;
	public int tileYSize = 10;
	public float perlinResolution = 0.02f;
	public float perlinThreshold = 0.4f;
	public float perlinTunnelThreshold = 6f;
	public float perimeterBuffer = 0.1f;

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

	//	DigCircle ((float)worldSizeX/2f, (float)worldSizeY/2f, vesselRadius - perimeterBuffer, false);
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

	//Treat the circle as a hemisphere, then normalise elevation to the radius of the hemisphere.
	public void DigCircle(float cx, float cy, float radius, bool solid){
		int xmin = (int)Mathf.Max ((int) cx - (int) radius - 2, 0);
		int ymin = (int)Mathf.Max ((int) cy - (int) radius - 2, 0);
		int xmax = (int)Mathf.Min ((int) cx + (int) radius + 2, nodeXSize-1);
		int ymax = (int)Mathf.Min ((int) cy + (int) radius + 2, nodeYSize-1);

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
					float hemisphereHeightNormalised = hemisphereHeight/ radius;
					//if (hemisphereHeight > 1) hemisphereHeight = 1f;
					if (solid) {
						nodeArray [i, j] = hemisphereHeightNormalised;
					}
					else {
						nodeArray [i, j] = 1f - hemisphereHeightNormalised;
					}
				}
			}
		}
	}

	public void DigPerlinCaves(float res){
		for(int i = 0; i < nodeXSize; i++){
			for (int j = 0; j < nodeYSize; j++){
				float perl = 1f-Mathf.PerlinNoise (i * res, j * res);
				if ((perl < nodeArray [i, j] - 0.2f) && (perl < perlinThreshold))
		//		if (perl < perlinThreshold)
					nodeArray [i, j] = perl;
			}
		}
	}

	public void DigPerlinTunnels(float res){
		for(int i = 0; i < nodeXSize; i++){
			for (int j = 0; j < nodeYSize; j++){
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

		topHull.Add (new Vector2 (100, 100));
		topHull.Add (new Vector2 (125, 190));
		topHull.Add (new Vector2 (150, 100));

		botHull.Add (new Vector2 (100, 100));
		botHull.Add (new Vector2 (125, -50));
		botHull.Add (new Vector2 (150, 100));

	/*	topHull.Add (new Vector2 (10, 60));
		topHull.Add (new Vector2 (60, 110));
		topHull.Add (new Vector2 (110, 60));

		botHull.Add (new Vector2 (10, 60));
		botHull.Add (new Vector2 (60, 10));
		botHull.Add (new Vector2 (110, 60));*/

		new MarchingSquaresCutTools(nodeArray).DigConvexHull (topHull, botHull, isSolid);
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

/*	private float InterpolateEdge(float low, float high){

		float ratio = (low + high) / 2f;

		if (high > low)
			ratio = 1f - ratio;

		return ratio;
	}*/

	//Finds the point that a line from one node to another would intersect height 0.5
	private float InterpolateEdge(float a, float b){
		return (0.5f - a) / (b - a);
	}

	//Returns a weighted point between the two nodes. -ve, to distinguish from 0.5 intersections.
	private float MidPoint(float a, float b){
	//	return -(a + b) / 2f;
		return -0.5f;
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
