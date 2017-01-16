using UnityEngine;
using System.Collections;

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

			int i = 0;
			int j = 0;
			for (i = 0; i < nodeXSize; i++){
				for (j = 0; j < nodeYSize; j++){
				//	nodeArray [i, j] = Mathf.PerlinNoise (i * perlinResolution, j * perlinResolution) * 0.9f;
				nodeArray[i,j] = 0.0f;
				}
			}


		DigCircle ((float)worldSizeX/2f, (float)worldSizeY/2f, vesselRadius - perimeterBuffer, true);
		DigPerlinTunnels (perlinResolution);
	//	DigPerlinCaves (perlinResolution);


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

	public void DigCircle(float cx, float cy, float radius, bool solid){
		int xmin = (int)Mathf.Max ((int) cx - (int) radius - 1, 0);
		int ymin = (int)Mathf.Max ((int) cy - (int) radius - 1, 0);
		int xmax = (int)Mathf.Min ((int) cx + (int) radius + 1, nodeXSize-1);
		int ymax = (int)Mathf.Min ((int) cy + (int) radius + 1, nodeYSize-1);

		for (int i = xmin; i < xmax; i++) {
			for (int j = ymin; j < ymax; j++) {
				float dist = Vector3.Distance (new Vector3 (cx, cy, 0f), new Vector3 ((float)i, (float)j, 0f));
				float overlap = radius - dist;
				if (solid) {
					if ((overlap > 1)) {
						nodeArray [i, j] = 1f;
					} else if (overlap > 0) {
						nodeArray [i, j] = overlap;
					}
				} else {
					
					if ((overlap > 1)) {
						nodeArray [i, j] = 0f;
					} else if (overlap > 0){
						if (1 - overlap < nodeArray[i,j])
							nodeArray[i,j] = 1 - overlap;
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
