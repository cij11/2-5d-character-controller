using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionChunk : MonoBehaviour {
	private MeshFilter meshFilter;
	private MeshCollider meshCollider;
	private Mesh colMesh;

//	private List<Vector3> colVerts;
//	private List<int> colTris;
	private Vector3[] colVerts;
	private int[] colTris;

	private int chunkSize = 10;
	private Coord chunkCoord;
	private int numTiles = 0;
	private int maxPanes = 0;
	private int maxVerts = 0;
	private int maxTris = 0;
	private int numPanes = 0;
	private int numVerts = 0;
	private int numTris = 0;

	public void Initialise(int chunkSize){
		this.chunkSize = chunkSize;

		meshFilter = GetComponent<MeshFilter> () as MeshFilter;
		meshCollider = GetComponent<MeshCollider> () as MeshCollider;
		colMesh = new Mesh ();

		numTiles = chunkSize * chunkSize;
		maxPanes = (int) numTiles;
		maxVerts = maxPanes * 4;
		maxTris = maxPanes * 2;

		colVerts = new Vector3[maxVerts];
		colTris = new int[maxTris * 3];

		int i = 0;
		for (i = 0; i < maxVerts; i++) {
			colVerts [i] = new Vector3 (0f, 0f, 0f);
		}
		for (i = 0; i < maxTris * 3; i++) {
			colTris [i] = 0;
		}
	}

	public void SetChunkCoord(Coord newCoord){
		this.chunkCoord = newCoord;
	}

	public void UpdateCollisionChunk(MarchingSquaresGrid marchingGrid){
		BuildMesh (marchingGrid);
	}

	public void BuildMesh(MarchingSquaresGrid marchingGrid){
		//Loop through marchingGrid starting from your own coordinate
		for (int i = chunkCoord.X*chunkSize; i < chunkCoord.X*chunkSize + chunkSize; i++) {
			for (int j = chunkCoord.Y*chunkSize; j < chunkCoord.Y*chunkSize + chunkSize; j++) {
				BuildSquare (i, j, marchingGrid);
			}
		}
		while (numPanes < maxPanes)
			StowPane ();
		
		UpdateMesh ();
	}

	private void StowPane(){
		BuildPane ((float)numPanes-3000f, -3000f - chunkCoord.Y - chunkCoord.X * 50, (float)(numPanes + 1)-3000f, -3000f- chunkCoord.Y - chunkCoord.X * 50, true);
	}

	private void BuildSquare (int x, int y, MarchingSquaresGrid marchingGrid){
		float xOffset = ((float)x);
		float yOffset = ((float)y);
		Vector3 vecOffset = new Vector3 (xOffset, yOffset, 0f);

		Vector3 start1 = new Vector3 (0f, 0f, 0f);
		Vector3 end1 = new Vector3 (0f, 0f, 0f);
		Vector3 start2 = new Vector3 (0f, 0f, 0f);
		Vector3 end2 = new Vector3 (0f, 0f, 0f);
		int numPanesToRender = 0;

		//A tile will have
		//0 collision panes if all corners true or no corners true
		//1 collision pane if 1 corner true, 1 corner false, or 2 adjacent corners true
		//2 collision panes corners alternate true and false

		//Turn the pattern of corners into a nybble
		ushort cornerPattern = 0x0000;

		for (int i = 0; i < 4; i++) {
			if (GetNode (i * 2, x, y, marchingGrid) >= 0.5f) {
				cornerPattern |= (ushort)((ushort)0x0001 << (i*4));
			}
		}

		switch (cornerPattern) {
		case 0x0000:	//All empty
			{
				break;
			}
		case 0x1111:	//All occupied
			{
				break;
			}
		case 0x0001:	//Bot left occupied
			{
				start1 = GetTileVertPosition (1, x, y, marchingGrid);
				end1 = GetTileVertPosition (7, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x0010:	//Top left occupied
			{
				start1 = GetTileVertPosition (3, x, y, marchingGrid);
				end1 = GetTileVertPosition (1, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x0100:	//Top right occupied
			{
				start1 = GetTileVertPosition (5, x, y, marchingGrid);
				end1 = GetTileVertPosition (3, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x1000:	//Bot right occupied
			{
				start1 = GetTileVertPosition (7, x, y, marchingGrid);
				end1 = GetTileVertPosition (5, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x1110:	//Bot left vacant
			{
				start1 = GetTileVertPosition (7, x, y, marchingGrid);
				end1 = GetTileVertPosition (1, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x1101:	//Top left vacant
			{
				start1 = GetTileVertPosition (1, x, y, marchingGrid);
				end1 = GetTileVertPosition (3, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x1011:	//Top right vacant
			{
				start1 = GetTileVertPosition (3, x, y, marchingGrid);
				end1 = GetTileVertPosition (5, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x0111:	//Bot right vacant
			{
				start1 = GetTileVertPosition (5, x, y, marchingGrid);
				end1 = GetTileVertPosition (7, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x0011:	//Right facing wall
			{
				start1 = GetTileVertPosition (3, x, y, marchingGrid);
				end1 = GetTileVertPosition (7, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x1100:	//left facing wall
			{
				start1 = GetTileVertPosition (7, x, y, marchingGrid);
				end1 = GetTileVertPosition (3, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x1001:	//Up facing panel
			{
				start1 = GetTileVertPosition (1, x, y, marchingGrid);
				end1 = GetTileVertPosition (5, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x0110:	//Down facing panel
			{
				start1 = GetTileVertPosition (5, x, y, marchingGrid);
				end1 = GetTileVertPosition (1, x, y, marchingGrid);
				numPanesToRender = 1;
				break;
			}
		case 0x0101:	//Solid from bot left to top right
			{
				start1 = GetTileVertPosition (1, x, y, marchingGrid);
				end1 = GetTileVertPosition (3, x, y, marchingGrid);
				start2 = GetTileVertPosition (5, x, y, marchingGrid);
				end2 = GetTileVertPosition (7, x, y, marchingGrid);
				numPanesToRender = 2;
				break;
			}
		case 0x1010:	//Solid from top left to bot right
			{
				start1 = GetTileVertPosition (3, x, y, marchingGrid);
				end1 = GetTileVertPosition (5, x, y, marchingGrid);
				start2 = GetTileVertPosition (7, x, y, marchingGrid);
				end2 = GetTileVertPosition (1, x, y, marchingGrid);
				numPanesToRender = 2;
				break;
			}
		}

		start1 = start1 + new Vector3 (xOffset, yOffset);
		end1 = end1 + new Vector3 (xOffset, yOffset);
		start2 = start2 + new Vector3 (xOffset, yOffset);
		end2 = end2 + new Vector3 (xOffset, yOffset);

		if (numPanesToRender == 1) {
			BuildPane (start1.x, start1.y, end1.x, end1.y);
		} else if (numPanesToRender == 2) {
			BuildPane (start1.x, start1.y, end1.x, end1.y);
			BuildPane (start2.x, start2.y, end2.x, end2.y);
		}
	}

	private void BuildPane(float startX, float startY, float endX, float endY){
		if (numPanes < maxPanes) {
			int anchorVert = numVerts; //This will be the index of the first vert added to the pane, and the start of the next two triangles
			//Add deep and shallow vertices for the start pane and the end pane, arranged start shallow, start deep,
			//end deep, end shallow
			AddColVert (startX, startY, 0f);
			AddColVert (startX, startY, 1f);
			AddColVert (endX, endY, 1f);
			AddColVert (endX, endY, 0f);

			colTris [numTris * 3] = anchorVert;
			colTris [numTris * 3 + 1] = anchorVert + 1;
			colTris [numTris * 3 + 2] = anchorVert + 2;

			colTris [numTris * 3 + 3] = anchorVert;
			colTris [numTris * 3 + 4] = anchorVert + 2;
			colTris [numTris * 3 + 5] = anchorVert + 3;

			numTris += 2;
			numPanes += 1;
		}
	}

	//Build a plane that doesn't interact with rendered objects.
	//Better system would generate a minimum number of planes from a budget. Work on this later.
	private void BuildPane(float startX, float startY, float endX, float endY, bool outOfPlane){
		int anchorVert = numVerts; //This will be the index of the first vert added to the pane, and the start of the next two triangles
		//Add deep and shallow vertices for the start pane and the end pane, arranged start shallow, start deep,
		//end deep, end shallow
		AddColVert (startX, startY, -261f);
		AddColVert (startX, startY, -260f);
		AddColVert (endX, endY, -260f);
		AddColVert (endX, endY, -261f);

		colTris [numTris * 3] = anchorVert;
		colTris [numTris * 3 + 1] = anchorVert + 1;
		colTris [numTris * 3 + 2] = anchorVert + 2;

		colTris [numTris * 3 + 3] = anchorVert;
		colTris [numTris * 3 + 4] = anchorVert +2;
		colTris [numTris * 3 + 5] = anchorVert + 3;

		numTris += 2;
		numPanes += 1;
	}

	private void AddColVert(Vector3 newVert){
		colVerts[numVerts].x = newVert.x;
		colVerts[numVerts].y = newVert.y;
		colVerts[numVerts].z = newVert.z;

		numVerts++;
	}

	private void AddColVert(float x, float y){
		colVerts[numVerts].x = x;
		colVerts[numVerts].y = y;
		colVerts[numVerts].z = 0f;
		numVerts++;
	}

	private void AddColVert(float x, float y, float z){
		colVerts[numVerts].x = x;
		colVerts[numVerts].y = y;
		colVerts[numVerts].z = z;
		numVerts++;
	}

	//Converts the 3 arrays of the marching square grid (corner nodes, horizontal edge nodes, vertical edge nodes) into
	//a series of 8 nodes arranged clockewise from the lower left boundary of a node at 0.
	private float GetNode(int index, int anchorX, int anchorY, MarchingSquaresGrid marchingGrid){
		switch (index) {
		case 0:
			{
				return marchingGrid.GetNode (anchorX, anchorY);
			}
		case 1:
			{
				return marchingGrid.GetVert (anchorX, anchorY);
			}
		case 2:
			{
				return marchingGrid.GetNode (anchorX, anchorY + 1);
			}
		case 3:
			{
				return marchingGrid.GetHoriz (anchorX, anchorY + 1);
			}
		case 4:
			{
				return marchingGrid.GetNode (anchorX + 1, anchorY + 1);
			}
		case 5:
			{
				return marchingGrid.GetVert (anchorX + 1, anchorY);
			}
		case 6:
			{
				return marchingGrid.GetNode (anchorX + 1, anchorY);
			}
		case 7:
			{
				return marchingGrid.GetHoriz (anchorX, anchorY);
			}

		}
		return -1f;
	}

	//Take a tile and an index, and return the vertext in 0,0 to 1,1 terms.
	private Vector3 GetTileVertPosition(int index, int anchorX, int anchorY, MarchingSquaresGrid marchingGrid){
		switch (index) {
		case 0:
			{
				return new Vector3 (0f, 0f, 0f);
			}
		case 1:
			{
				return new Vector3 (0f, GetNode (index, anchorX, anchorY, marchingGrid), 0f);
			}
		case 2:
			{
				return new Vector3 (0f, 1f, 0f);
			}
		case 3:
			{
				return new Vector3 (GetNode (index, anchorX, anchorY, marchingGrid), 1f, 0f);
			}
		case 4:
			{
				return new Vector3 (1f, 1f, 0f);
			}
		case 5:
			{
				return new Vector3 (1f, GetNode (index, anchorX, anchorY, marchingGrid), 0f);
			}
		case 6:
			{
				return new Vector3 (1f, 0f, 0f);
			}
		case 7:
			{
				return new Vector3 (GetNode (index, anchorX, anchorY, marchingGrid), 0f, 0f);
			}
		}
		return new Vector3 (0f, 0f, 0f);
	}

	private void UpdateMesh(){
		colMesh.Clear ();
		colMesh.vertices = colVerts;
		colMesh.triangles = colTris;
		meshCollider.sharedMesh = colMesh;

		numTris = 0;
		numVerts = 0;
		numPanes = 0;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
