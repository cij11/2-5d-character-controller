using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderChunk : MonoBehaviour {

	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private Mesh faceMesh;

	private List<Vector3> faceVerts;
	private List<int> faceTris;
	private List<Color32> faceColors;

	private int chunkSize = 10;
	private Coord chunkCoord;
	private int numVerts = 0;
	private int numTris = 0;

	float borderWidth = 0.2f;
	// Use this for initialization
	void Start () {
	
	}

	public void Initialise(int chunkSize){
		this.chunkSize = chunkSize;

		meshFilter = GetComponent<MeshFilter> () as MeshFilter;
		meshRenderer = GetComponent<MeshRenderer> () as MeshRenderer;
		faceMesh = GetComponent<MeshFilter> ().mesh;

		faceVerts = new List<Vector3> ();
		faceTris = new List<int> ();
		faceColors = new List<Color32> ();
	}

	public void SetChunkCoord(Coord newCoord){
		this.chunkCoord = newCoord;
	}

	public void UpdateRenderChunk(MarchingSquaresGrid marchingGrid, OreGrid oreGrid){
		BuildMesh (marchingGrid, oreGrid);
	}

	public void BuildMesh(MarchingSquaresGrid marchingGrid, OreGrid oreGrid){
		//Loop through marchingGrid starting from your own coordinate
		for (int i = chunkCoord.X*chunkSize; i < chunkCoord.X*chunkSize + chunkSize; i++) {
			for (int j = chunkCoord.Y*chunkSize; j < chunkCoord.Y*chunkSize + chunkSize; j++) {
				BuildSquare (i, j, marchingGrid, oreGrid);
			}
		}
		UpdateMesh ();
	}
		
	private void BuildSquare (int x, int y, MarchingSquaresGrid marchingGrid, OreGrid oreGrid){
		float xOffset = (float)x;
		float yOffset = (float)y;
		Vector3 vecOffset = new Vector3 (xOffset, yOffset, 0f);

		Color32 borderColor = new Color32 (230, 230, 230, 255);
		Color32 mainColor = new Color32 (200, 200, 200, 255);

		//Build an individual square by going clockwise around the nodes and interpolations in that tile space
		int numCornerNodes = 0;
		int numAllTypeNodes = 0;
		int i = 0;
		int anchorVert = numVerts;

		int numShapes = 0;	//One shape per unique OreType.

		for (i = 0; i < 8; i++) {
			if (i == 0 || i == 2 || i == 4 || i == 6) {
				if (GetNode (i, x, y, marchingGrid) >= 0.5f) {
					numCornerNodes++;
				}
			} 
		}

		if (numCornerNodes > 0) {
			//Parse up to 4 shapes.

			//Look at each corner.
			//If the Corner is rendered, set its bit to true.
			//If the the adjacent corners are different or not rendered, set the adjacent edge bits to true.
			byte botLeft = 0;
			byte topRight = 0;
			byte topLeft = 0;
			byte botRight = 0;

			OreTypes ore0 = (OreTypes)oreGrid.GetTile (x, y);
			OreTypes ore1 = (OreTypes)oreGrid.GetTile (x, y+1);
			OreTypes ore2 = (OreTypes)oreGrid.GetTile (x+1, y+1);
			OreTypes ore3 = (OreTypes)oreGrid.GetTile (x+1, y);

			bool unique0 = true;
			bool unique1 = true;
			bool unique2 = true;
			bool unique3 = true;

			int shape0Nodes = 0;
			int shape1Nodes = 0;
			int shape2Nodes = 0;
			int shape3Nodes = 0;

			//Identify the ore types that appear. 
			if (ore0 == ore1)
				unique1 = false;
			if (ore0 == ore2)
				unique2 = false;
			if (ore0 == ore3)
				unique3 = false;

			if (ore1 == ore2)
				unique2 = false;
			if (ore1 == ore3)
				unique3 = false;

			if (ore2 == ore3)
				unique3 = false;

			//Render the first ore across the entire tile, to prevent gaps
			bool baseColorRendered = false;
			//For each unique ore type, loop around the perimeter, flagging raised corners of that type, and edge nodes where the ore is different or the elevation changes.
			//If a corner is the first instance of an ore type, parse its shape. Redundant ore type corners will be flagged in the first instance as part of this loop.
			if (unique0) {
				botLeft = ParseBaseShape(ore0, marchingGrid, oreGrid, x, y);
				baseColorRendered = true;
			//	botLeft = ParseShape (ore0, marchingGrid, oreGrid, x, y);
			}
			if (unique1) {
				if (baseColorRendered) {
					topLeft = ParseShape (ore1, marchingGrid, oreGrid, x, y);
				} else {
					baseColorRendered = true;
					topLeft = ParseBaseShape (ore1, marchingGrid, oreGrid, x, y);
				}
			}
			if (unique2) {
				if (baseColorRendered) {
					topRight = ParseShape (ore2, marchingGrid, oreGrid, x, y);
				} else {
					baseColorRendered = true;
					topRight = ParseBaseShape (ore2, marchingGrid, oreGrid, x, y);
				}
			}
			if (unique3) {
				if (baseColorRendered) {
					botRight = ParseShape (ore3, marchingGrid, oreGrid, x, y);
				} else {
					botRight = ParseBaseShape (ore3, marchingGrid, oreGrid, x, y);
				}
			}

			//If a corner is the origin of a shape, render it.
			if (botLeft > 0) { //if bot left needs to be rendered
				mainColor = oreGrid.GetOreColor(ore0);
				for (i = 0; i < 8; i++) {
					if (  (botLeft & ((byte)1 << i)) > 0) {
						AddFaceVert (GetTileVertPosition (i, x, y, marchingGrid) + vecOffset + new Vector3(0f, 0f, -0.1f), mainColor);
						shape0Nodes++;
					}
				}

				for (i = 1; i < shape0Nodes-1; i++) {
					faceTris.Add (anchorVert);
					faceTris.Add (anchorVert + i);
					faceTris.Add (anchorVert + i + 1);
					numTris++;
				}
				anchorVert = numVerts;
			}

			if (topLeft > 0) { 
				mainColor = oreGrid.GetOreColor(ore1);
				for (i = 0; i < 8; i++) {
					if (  (topLeft & ((byte)1 << i)) > 0) {
						AddFaceVert (GetTileVertPosition (i, x, y, marchingGrid) + vecOffset+ new Vector3(0f, 0f, -0.2f), mainColor);
						shape1Nodes++;
					}
				}

				for (i = 1; i < shape1Nodes-1; i++) {
					faceTris.Add (anchorVert);
					faceTris.Add (anchorVert + i);
					faceTris.Add (anchorVert + i + 1);
					numTris++;
				}
				anchorVert = numVerts;
			}

			if (topRight > 0) {
				mainColor = oreGrid.GetOreColor(ore2);
				for (i = 0; i < 8; i++) {
					if (  (topRight & ((byte)1 << i)) > 0) {
						AddFaceVert (GetTileVertPosition (i, x, y, marchingGrid) + vecOffset+ new Vector3(0f, 0f, -0.3f), mainColor);
						shape2Nodes++;
					}
				}

				for (i = 1; i < shape2Nodes-1; i++) {
					faceTris.Add (anchorVert);
					faceTris.Add (anchorVert + i);
					faceTris.Add (anchorVert + i + 1);
					numTris++;
				}
				anchorVert = numVerts;
			}

			if (botRight > 0) {
				mainColor = oreGrid.GetOreColor(ore3);
				for (i = 0; i < 8; i++) {
					if (  (botRight & ((byte)1 << i)) > 0) {
						AddFaceVert (GetTileVertPosition (i, x, y, marchingGrid) + vecOffset+ new Vector3(0f, 0f, -0.4f), mainColor);
						shape3Nodes++;
					}
				}

				for (i = 1; i < shape3Nodes-1; i++) {
					faceTris.Add (anchorVert);
					faceTris.Add (anchorVert + i);
					faceTris.Add (anchorVert + i + 1);
					numTris++;
				}
				anchorVert = numVerts;
			}

			//Add a lighter colour to the edge of the tile
			BuildOutline (x, y, marchingGrid, borderColor);
		}
	}

	//Base shape only cares about elevation, not ore type.
	private byte ParseBaseShape(OreTypes ore, MarchingSquaresGrid marchingGrid, OreGrid oreGrid, int x, int y){
		byte shape = 0;
		for (int i = 0; i < 7; i += 2) {
			if (  GetNode(i, x, y, marchingGrid)   > 0.5f) {
				shape |= (byte)(1 << i);
				int nextIndex = Mod (i + 2, 8);
				int prevIndex = Mod (i - 2, 8);
				int nextIntermediate = Mod (i + 1, 8);
				int prevIntermediate = Mod (i - 1, 8);

				//If the next node is a different type, or below the rendering threshold, add the intermediate node
				if (GetNode (nextIndex, x, y, marchingGrid) < 0.5f) {
					shape |= (byte)(1 << (nextIntermediate));
				}
				if (GetNode (prevIndex, x, y, marchingGrid) < 0.5f) {
					shape |= (byte)(1 << (prevIntermediate));
				}
			}
		}
		return shape;
		//	return (byte) 1 +( 1<< 1) + (1 << 7);
	}

	private byte ParseShape(OreTypes ore, MarchingSquaresGrid marchingGrid, OreGrid oreGrid, int x, int y){
		byte shape = 0;
		for (int i = 0; i < 7; i += 2) {
			if (  GetNode(i, x, y, marchingGrid)   > 0.5f && oreGrid.GetTileByIndex(i, x, y) == (int)ore) {
				shape |= (byte)(1 << i);
				int nextIndex = Mod (i + 2, 8);
				int prevIndex = Mod (i - 2, 8);
				int nextIntermediate = Mod (i + 1, 8);
				int prevIntermediate = Mod (i - 1, 8);

				//If the next node is a different type, or below the rendering threshold, add the intermediate node
				if (GetNode (nextIndex, x, y, marchingGrid) < 0.5f || oreGrid.GetTileByIndex (nextIndex, x, y) != (int)ore) {
					shape |= (byte)(1 << (nextIntermediate));
				}
				if (GetNode (prevIndex, x, y, marchingGrid) < 0.5f || oreGrid.GetTileByIndex (prevIndex, x, y) != (int)ore) {
					shape |= (byte)(1 << (prevIntermediate));
				}
			}
		}
		return shape;
	//	return (byte) 1 +( 1<< 1) + (1 << 7);
	}

	private int Mod(int num, int divisor){
		int raw = num % divisor;
		if (raw < 0)
			raw = raw + divisor;
		return raw;
	}
		
	//Largely copied form collisionChunk. That makes these calculations redundant. Might be better to preprocess
	//some of this info to give to both collision and render chunks.
	private void BuildOutline(int x, int y, MarchingSquaresGrid marchingGrid, Color32 borderColor){
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
				cornerPattern |= (ushort)((ushort)0x0001 << (i * 4));
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

/*		if (numPanesToRender == 1) {
			BuildBorderEdge (start1.x, start1.y, end1.x, end1.y, borderColor);
		} else if (numPanesToRender == 2) {
			BuildBorderEdge (start1.x, start1.y, end1.x, end1.y, borderColor);
			BuildBorderEdge (start2.x, start2.y, end2.x, end2.y, borderColor);
		}*/

		if (numPanesToRender == 1) {
			BuildBorderEdge (end1.x, end1.y, start1.x, start1.y, borderColor);
		} else if (numPanesToRender == 2) {
			BuildBorderEdge (end1.x, end1.y, start1.x, start1.y, borderColor);
			BuildBorderEdge (end2.x, end2.y, start2.x, start2.y, borderColor);
		}
	}

	//Build a thin quad going from the start point to the end point
	private void BuildBorderEdge(float startx, float starty, float endx, float endy, Color32 borderColor){
		int anchorVert = numVerts;
		//Quad goes from start, to start+normal, to end+normal, to end
		Vector3 normalVec = new Vector3(-(endy - starty), (endx - startx));
		normalVec.Normalize ();
		normalVec = normalVec * borderWidth;

		//Rendering on the 0 level
/*		AddFaceVert(new Vector3(startx, starty, 0f), borderColor);
		AddFaceVert(new Vector3(startx, starty, 0f) + normalVec, borderColor);
		AddFaceVert(new Vector3(endx, endy, 0f) + normalVec, borderColor);

		AddFaceVert(new Vector3(startx, starty, 0f), borderColor);
		AddFaceVert(new Vector3(endx, endy, 0f) + normalVec, borderColor);
		AddFaceVert(new Vector3(endx, endy, 0f), borderColor);*/

		//Rendering projected into the screen
		AddFaceVert(new Vector3(startx, starty, -0.5f), borderColor);
		AddFaceVert(new Vector3(startx, starty, -0.5f) + normalVec, borderColor);
		AddFaceVert(new Vector3(endx, endy, -0.5f) + normalVec, borderColor);

		AddFaceVert(new Vector3(startx, starty, -0.5f), borderColor);
		AddFaceVert(new Vector3(endx, endy, -0.5f) + normalVec, borderColor);
		AddFaceVert(new Vector3(endx, endy, -0.5f), borderColor);

		faceTris.Add (anchorVert);
		faceTris.Add (anchorVert + 1);
		faceTris.Add (anchorVert + 2);

		faceTris.Add (anchorVert+3);
		faceTris.Add (anchorVert + 4);
		faceTris.Add (anchorVert + 5);
		numTris+=2;
	}


	private void AddFaceVert(Vector3 newVert, Color32 vertColor){
		faceVerts.Add (newVert);
		faceColors.Add (vertColor);
		numVerts++;
	}

	private void AddFaceVert(float x, float y, Color32 vertColor){
		faceVerts.Add (new Vector3 (x, y, 0f));
		faceColors.Add (vertColor);
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
				;
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

	private float GetIntermediateNodePosition(int index, int anchorX, int anchorY, MarchingSquaresGrid marchingGrid){
		switch (index) {
		case 0:
			{
				return marchingGrid.GetNode (anchorX, anchorY);
			}
		case 1:
			{
				return Mathf.Abs( marchingGrid.GetVert (anchorX, anchorY));
				;
			}
		case 2:
			{
				return marchingGrid.GetNode (anchorX, anchorY + 1);

			}
		case 3:
			{
				return Mathf.Abs(marchingGrid.GetHoriz (anchorX, anchorY + 1));

			}
		case 4:
			{
				return marchingGrid.GetNode (anchorX + 1, anchorY + 1);

			}
		case 5:
			{
						return Mathf.Abs(marchingGrid.GetVert (anchorX + 1, anchorY));

			}
		case 6:
			{
				return marchingGrid.GetNode (anchorX + 1, anchorY);

			}
		case 7:
			{
								return Mathf.Abs(marchingGrid.GetHoriz (anchorX, anchorY));

			}

		}
		return -1f;
	}

	private Color32 GetNodeColor(int index, int anchorX, int anchorY, OreGrid oreGrid){
		switch (index) {
		case 0:
			{
				return oreGrid.GetColorOfTile (anchorX, anchorY);
			}
		case 1:
			{
				return Color32.Lerp( oreGrid.GetColorOfTile (anchorX, anchorY),oreGrid.GetColorOfTile (anchorX, anchorY + 1) , 0.5f);
				;
			}
		case 2:
			{
				return  oreGrid.GetColorOfTile (anchorX, anchorY + 1);

			}
		case 3:
			{
				return Color32.Lerp( oreGrid.GetColorOfTile (anchorX+1, anchorY+1),oreGrid.GetColorOfTile (anchorX, anchorY + 1) , 0.5f);

			}
		case 4:
			{
				return oreGrid.GetColorOfTile (anchorX + 1, anchorY + 1);

			}
		case 5:
			{
				return Color32.Lerp( oreGrid.GetColorOfTile (anchorX+1, anchorY+1),oreGrid.GetColorOfTile (anchorX+1, anchorY) , 0.5f);

			}
		case 6:
			{
				return oreGrid.GetColorOfTile (anchorX + 1, anchorY);

			}
		case 7:
			{
				return Color32.Lerp( oreGrid.GetColorOfTile (anchorX, anchorY),oreGrid.GetColorOfTile (anchorX+1, anchorY) , 0.5f);

			}
		default:
			return new Color32 (0, 255, 255, 255);
		}
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
				return new Vector3 (0f, Mathf.Abs( GetNode (index, anchorX, anchorY, marchingGrid)), 0f);	//Abs to convert to -ve values (indicating they don't cross the 0.5 mark) into postive values 
																											//(which will be midway between two points that are both above or below 0.5)
				
			}
		case 2:
			{
				return new Vector3 (0f, 1f, 0f);
				
			}
		case 3:
			{
				return new Vector3 (Mathf.Abs(GetNode (index, anchorX, anchorY, marchingGrid)), 1f, 0f);
				
			}
		case 4:
			{
				return new Vector3 (1f, 1f, 0f);
				
			}
		case 5:
			{
				return new Vector3 (1f, Mathf.Abs(GetNode (index, anchorX, anchorY, marchingGrid)), 0f);
				
			}
		case 6:
			{
				return new Vector3 (1f, 0f, 0f);
				
			}
		case 7:
			{
				return new Vector3 (Mathf.Abs(GetNode (index, anchorX, anchorY, marchingGrid)), 0f, 0f);
				
			}
		}
		return new Vector3 (0f, 0f, 0f);
	}

	private void UpdateMesh(){
		faceMesh.Clear ();
		faceMesh.vertices = faceVerts.ToArray();
		faceMesh.triangles = faceTris.ToArray();
		faceMesh.colors32 = faceColors.ToArray ();
		;
		faceMesh.RecalculateNormals ();

		numTris = 0;
		numVerts = 0;
		faceVerts.Clear ();
		faceTris.Clear ();
		faceColors.Clear ();
	}
}