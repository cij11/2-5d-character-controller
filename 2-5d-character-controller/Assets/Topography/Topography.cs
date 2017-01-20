using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Topography : MonoBehaviour {
	public GameObject marchingGridPrefab;
	public GameObject renderChunkPrefab;
	public GameObject collisionChunkPrefab;
	public GameObject facePrefab;
	public GameObject interiorPrefab;

	public int chunkSize = 10;
	public int numChunks = 10;
	public int chunkRenderSpawnRadius = 12;
	public int chunkCollisionSpawnRadius = 12;
	int instantiatedChunkCount = 0;
	int instantiatedCollisionChunkCount = 0;

	MarchingSquaresGrid marchingGrid;

	private int worldSizeX;
	private int worldSizeY;
	private float vesselRadius = 10f;
	private Vector3 vesselCenter = new Vector3 (20f, 20f, 0f);

	private Stack<RenderChunk> renderChunkPool;
	private Queue<RenderChunk> renderChunkUpdateQueue;
	private Queue<RenderChunk> renderChunkPriorityUpdateQueue;
	private RenderChunk[,] renderChunkArray;

	private Stack<CollisionChunk> collisionChunkPool;
	private Queue<CollisionChunk> collisionChunkUpdateQueue;
	private Queue<CollisionChunk> collisionChunkPriorityUpdateQueue;
	private CollisionChunk[,] collisionChunkArray;

	int digCooldown = 4;
	int digTimer = 0;

	private GameObject faceGO;
	private GameObject interiorGO;

	private float interiorZLayer = 0f;

	private GameObject renderFocus = null;
	private GameObject playerUnitGO = null;

	private OreGrid oreGrid = null;

	private StampCollection stampCollection;

	// Use this for initialization
	void Start () {

		Initialise (numChunks * chunkSize - chunkSize);
	//	renderChunkGO.transform.SetParent (hullGO.transform);
	
	}

	public void Initialise(float vesselSize){
		this.vesselRadius = vesselSize/2f;

		//Ensure enough chunks to contain vessel. Could add a buffer to build above this level.
		numChunks = (int)Mathf.Ceil (vesselSize / chunkSize);
		worldSizeX = numChunks * chunkSize;
		worldSizeY = numChunks * chunkSize;
		vesselCenter = new Vector3((float)worldSizeX / 2f, (float)worldSizeY / 2f, 0f);

		GameObject marchingGridGO = Instantiate (marchingGridPrefab, this.transform.position, Quaternion.identity) as GameObject;
		marchingGrid = marchingGridGO.GetComponent<MarchingSquaresGrid> () as MarchingSquaresGrid;
		marchingGrid.Initialise (worldSizeX, worldSizeY, vesselRadius);

		stampCollection = this.transform.GetComponentInChildren<StampCollection> () as StampCollection;
		if (stampCollection != null) { //May not have a stamp collection to apply to this topography
			stampCollection.ApplyStamp (marchingGrid);
		}

		oreGrid = new OreGrid ();
		oreGrid.GenerateMap (worldSizeX, worldSizeY);

		renderChunkPool = new Stack<RenderChunk> ();
		renderChunkUpdateQueue = new Queue<RenderChunk> ();
		renderChunkPriorityUpdateQueue = new Queue<RenderChunk> ();
		renderChunkArray = new RenderChunk[numChunks,numChunks];

		collisionChunkPool = new Stack<CollisionChunk> ();
		collisionChunkUpdateQueue = new Queue<CollisionChunk> ();
		collisionChunkPriorityUpdateQueue = new Queue<CollisionChunk> ();
		collisionChunkArray = new CollisionChunk[numChunks,numChunks];

		this.faceGO = Instantiate(facePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;

		this.interiorGO = Instantiate (interiorPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;

	}

	//Parent the renderChunkGO to the hull, and set its 0 poistion to -ve radius, -ve radius
	private void BindRenderToFace(GameObject renderChunkGO){
		Vector3 gridOffset = new Vector3 (-worldSizeX/2f, -worldSizeY/2f, 0f);

	//	renderChunkGO.transform.position = gridOffset + face.transform.position;
	//	renderChunkGO.transform.position = new Vector3(0f, 0f, 0f);
	//	renderChunkGO.transform.rotation = Quaternion.identity;
		renderChunkGO.transform.SetParent (faceGO.transform);
		renderChunkGO.transform.localRotation = Quaternion.identity; //Fix for some chunks having non-zero relative rotations.
		renderChunkGO.transform.localPosition = gridOffset;
	}

	private void BindCollisionToInterior(GameObject collisionChunkGO){
		//	renderChunkGO.transform.position = new Vector3 (-vesselRadius, -vesselRadius, 0f);
		collisionChunkGO.transform.position = new Vector3(-worldSizeX/2f, -worldSizeY/2f, interiorZLayer);
		collisionChunkGO.transform.rotation = Quaternion.identity;
		collisionChunkGO.transform.SetParent (interiorGO.transform);
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (renderFocus == null) {
			renderFocus = GameObject.FindGameObjectWithTag ("RenderFocus");
		}
	
			SpawnDespawnRenderChunksWithinRange ();
			UpdatePriorityRenderChunkQueue ();
			UpdateRenderChunkQueue ();

			SpawnDespawnCollisionChunksWithinRange ();
			UpdatePriorityCollisionChunkQueue ();
			UpdateCollisionChunkQueue ();
	}

	private void RefreshChunksInRange(int xTileMin, int yTileMin, int xTileMax, int yTileMax){
		Coord minTileCoord = new Coord ((int)Mathf.Max (0, xTileMin), (int)Mathf.Max (0, yTileMin));
		Coord maxTileCoord = new Coord ((int)Mathf.Min (worldSizeX, xTileMax), (int)Mathf.Min (worldSizeY, yTileMax));

		minTileCoord = minTileCoord.ConvertTileCoordToChunkCoord (chunkSize);
		maxTileCoord = maxTileCoord.ConvertTileCoordToChunkCoord (chunkSize);

		minTileCoord.X = Mathf.Max (0, minTileCoord.X-1);
		minTileCoord.Y = Mathf.Max (0, minTileCoord.Y-1);
		maxTileCoord.X = Mathf.Min (numChunks, maxTileCoord.X+ 1);
		maxTileCoord.Y = Mathf.Min (numChunks, maxTileCoord.Y + 1);

		for (int i = minTileCoord.X; i < maxTileCoord.X; i++) {
			for (int j = minTileCoord.Y; j < maxTileCoord.Y; j++) {
				FlagRenderChunkForPriorityUpdate(renderChunkArray[i, j]);
				FlagCollisionChunkForPriorityUpdate(collisionChunkArray[i, j]);
			}
		}
	}

	//Cull the renderered chunks to those near the player.
	private void SpawnDespawnRenderChunksWithinRange(){
	//	Vector3 playerPosition = renderFocus.transform.position + new Vector3 (worldSizeX / 2f, worldSizeY / 2f, 0f) + renderFocus.transform.position - hull.transform.position;
		Vector3 playerPosition = renderFocus.transform.position + new Vector3 (worldSizeX / 2f, worldSizeY / 2f, 0f);
		Coord playerCoord = new Coord ((int)playerPosition.x, (int)playerPosition.y);
		Coord playerChunkCoord = playerCoord.ConvertTileCoordToChunkCoord (chunkSize);

		//only check for tiles within a reasonable distance of the player.
		int minX = (int)Mathf.Max(0, playerChunkCoord.X - chunkRenderSpawnRadius - 2);
		int minY = (int)Mathf.Max(0, playerChunkCoord.Y - chunkRenderSpawnRadius - 2);
		int maxX = (int)Mathf.Min (numChunks, playerChunkCoord.X + chunkRenderSpawnRadius + 2);
		int maxY = (int)Mathf.Min (numChunks, playerChunkCoord.Y + chunkRenderSpawnRadius + 2);

		for (int i = minX; i < maxX; i++) {
			for (int j = minY; j < maxY; j++) {
				Coord worldChunkCoord = new Coord (i, j);
				float distance = worldChunkCoord.Dist (playerChunkCoord);

				//If the chunk is within the render radius
				if (distance < chunkRenderSpawnRadius) {
					//If the chunk doesn't exist
					if (renderChunkArray [i, j] == null) {
						InstantiateRenderChunk (i, j);
					}
				} else {
					if (renderChunkArray [i, j] != null) {
						RemoveRenderChunk (i, j);
					}
				}
			}
		}
	}

	private void PushRenderChunkToPool(RenderChunk renderChunkToDeactivate){
		renderChunkToDeactivate.gameObject.SetActive (false);
		renderChunkPool.Push (renderChunkToDeactivate);
	}

	//return the next available chunk. If no chunks available, instantiate a new chunk.
	private RenderChunk PullRenderChunkFromPool(){
		if (renderChunkPool.Count > 0) {
			RenderChunk renderChunkToActivate = renderChunkPool.Pop ();
			renderChunkToActivate.gameObject.SetActive (true);
			return renderChunkToActivate;
		} else {
			GameObject newRenderChunkGO = Instantiate (renderChunkPrefab, this.transform.position, Quaternion.identity) as GameObject;
	//		newRenderChunkGO.transform.SetParent (this.transform);
			RenderChunk newRenderChunk = newRenderChunkGO.GetComponent<RenderChunk> () as RenderChunk;
			newRenderChunk.Initialise (chunkSize);
			BindRenderToFace(newRenderChunkGO);
			instantiatedChunkCount++;
			return newRenderChunk;
		}
	}

	private void InstantiateRenderChunk(int x, int y){
		if (renderChunkArray [x, y] == null) {
			RenderChunk newRenderChunk = PullRenderChunkFromPool ();
			newRenderChunk.SetChunkCoord (new Coord (x, y));

			//Don't immediately update chunk, as many chunks will be added at once as the player crosses grid lines in
			//chunk coords. Enque, then update one per frame.
			renderChunkUpdateQueue.Enqueue (newRenderChunk);
			renderChunkArray [x, y] = newRenderChunk.GetComponent<RenderChunk> () as RenderChunk;
		}
	}

	private void RemoveRenderChunk(int x, int y){
		if (renderChunkArray [x, y] != null) {
			PushRenderChunkToPool (renderChunkArray [x, y]);
			renderChunkArray [x, y] = null;
		}
	}

	private void UpdateRenderChunkQueue(){
		for (int i = 0; i < 8; i++) {
			if (renderChunkUpdateQueue.Count > 0) {
				renderChunkUpdateQueue.Dequeue ().UpdateRenderChunk (marchingGrid, oreGrid);
			}
		}
	}

	//Chunks in this queue have been altered by the player, and are currently within the render radius.
	//Only que if not already in queue
	private void FlagRenderChunkForPriorityUpdate(RenderChunk renderChunkToUpdate){
		if(!renderChunkPriorityUpdateQueue.Contains(renderChunkToUpdate))
			renderChunkPriorityUpdateQueue.Enqueue (renderChunkToUpdate);
	}

	private void UpdatePriorityRenderChunkQueue(){
		for (int i = 0; i < 64; i++) {
			if (renderChunkPriorityUpdateQueue.Count > 0)
				renderChunkPriorityUpdateQueue.Dequeue ().UpdateRenderChunk (marchingGrid, oreGrid);
		}
	}

	private void RefreshRenderChunksInRange(int xTileMin, int yTileMin, int xTileMax, int yTileMax){
		Coord minTileCoord = new Coord ((int)Mathf.Max (0, xTileMin), (int)Mathf.Max (0, yTileMin));
		Coord maxTileCoord = new Coord ((int)Mathf.Min (worldSizeX, xTileMax), (int)Mathf.Min (worldSizeY, yTileMax));

		minTileCoord = minTileCoord.ConvertTileCoordToChunkCoord (chunkSize);
		maxTileCoord = maxTileCoord.ConvertTileCoordToChunkCoord (chunkSize);

		minTileCoord.X = Mathf.Max (0, minTileCoord.X-1);
		minTileCoord.Y = Mathf.Max (0, minTileCoord.Y-1);
		maxTileCoord.X = Mathf.Min (numChunks, maxTileCoord.X+ 1);
		maxTileCoord.Y = Mathf.Min (numChunks, maxTileCoord.Y + 1);

		for (int i = minTileCoord.X; i < maxTileCoord.X; i++) {
			for (int j = minTileCoord.Y; j < maxTileCoord.Y; j++) {
				//RemoveChunk (i, j);
				//InstantiateChunk (i, j);
				//	chunkArray[i,j].UpdateChunk (tileGrid);
				FlagRenderChunkForPriorityUpdate(renderChunkArray[i, j]);
			}
		}

	}




	//Cull the renderered chunks to those near the render focus.
	private void SpawnDespawnCollisionChunksWithinRange(){
		//Vector3 renderPosition = renderFocus.transform.position;
		Vector3 renderPosition = renderFocus.transform.position + new Vector3 (worldSizeX / 2f, worldSizeY / 2f, 0f);;
		Coord renderCoord = new Coord ((int)renderPosition.x, (int)renderPosition.y);
		Coord renderChunkCoord = renderCoord.ConvertTileCoordToChunkCoord (chunkSize);

		//only check for tiles within a reasonable distance of the render.
		int minX = (int)Mathf.Max(0, renderChunkCoord.X - chunkCollisionSpawnRadius - 2);
		int minY = (int)Mathf.Max(0, renderChunkCoord.Y - chunkCollisionSpawnRadius - 2);
		int maxX = (int)Mathf.Min (numChunks, renderChunkCoord.X + chunkCollisionSpawnRadius + 2);
		int maxY = (int)Mathf.Min (numChunks, renderChunkCoord.Y + chunkCollisionSpawnRadius + 2);

		for (int i = minX; i < maxX; i++) {
			for (int j = minY; j < maxY; j++) {
				Coord worldChunkCoord = new Coord (i, j);
				float distance = worldChunkCoord.Dist (renderChunkCoord);

				//If the chunk is within the render radius
				if (distance < chunkCollisionSpawnRadius) {
					//If the chunk doesn't exist
					if (collisionChunkArray [i, j] == null) {
						InstantiateCollisionChunk (i, j);
					}
				} else {
					if (collisionChunkArray [i, j] != null) {
						RemoveCollisionChunk (i, j);
					}
				}
			}
		}
	}

	private void PushCollisionChunkToPool(CollisionChunk collisionChunkToDeactivate){
		collisionChunkToDeactivate.gameObject.SetActive (false);
		collisionChunkPool.Push (collisionChunkToDeactivate);
	}

	//return the next available chunk. If no chunks available, instantiate a new chunk.
	private CollisionChunk PullCollisionChunkFromPool(){
		if (collisionChunkPool.Count > 0) {
			CollisionChunk collisionChunkToActivate = collisionChunkPool.Pop ();
		//	collisionChunkToActivate.gameObject.SetActive (true);
			return collisionChunkToActivate;
		} else {
			GameObject newCollisionChunkGO = Instantiate (collisionChunkPrefab, this.transform.position, Quaternion.identity) as GameObject;
			//		newCollisionChunkGO.transform.SetParent (this.transform);
			CollisionChunk newCollisionChunk = newCollisionChunkGO.GetComponent<CollisionChunk> () as CollisionChunk;
			newCollisionChunk.Initialise (chunkSize);
			newCollisionChunkGO.transform.SetParent (this.interiorGO.transform);
			BindCollisionToInterior (newCollisionChunkGO);
			instantiatedCollisionChunkCount++;
			return newCollisionChunk;
		}
	}

	private void InstantiateCollisionChunk(int x, int y){
		if (collisionChunkArray [x, y] == null) {
			CollisionChunk newCollisionChunk = PullCollisionChunkFromPool ();
			newCollisionChunk.SetChunkCoord (new Coord (x, y));

			//Don't immediately update chunk, as many chunks will be added at once as the player crosses grid lines in
			//chunk coords. Enque, then update one per frame.
			collisionChunkUpdateQueue.Enqueue (newCollisionChunk);
			collisionChunkArray [x, y] = newCollisionChunk.GetComponent<CollisionChunk> () as CollisionChunk;
		}
	}

	private void RemoveCollisionChunk(int x, int y){
		if (collisionChunkArray [x, y] != null) {
			PushCollisionChunkToPool (collisionChunkArray [x, y]);
			collisionChunkArray [x, y] = null;
		}
	}

	private void UpdateCollisionChunkQueue(){
		for (int i = 0; i < 8; i++) {
			if (collisionChunkUpdateQueue.Count > 0) {
				CollisionChunk chunkToActivate = collisionChunkUpdateQueue.Dequeue ();
				chunkToActivate.UpdateCollisionChunk (marchingGrid);
				chunkToActivate.gameObject.SetActive (true);	//Update the mesh before waking the object to prevent baking collision data twice.
			}
		}
	}

	//Chunks in this queue have been altered by the player, and are currently within the collision radius.
	//Only que if not already in queue
	private void FlagCollisionChunkForPriorityUpdate(CollisionChunk collisionChunkToUpdate){
		if(!collisionChunkPriorityUpdateQueue.Contains(collisionChunkToUpdate))
			collisionChunkPriorityUpdateQueue.Enqueue (collisionChunkToUpdate);
	}

	private void UpdatePriorityCollisionChunkQueue(){
		for (int i = 0; i < 8; i++) {
			if (collisionChunkPriorityUpdateQueue.Count > 0) {
				CollisionChunk chunkToActivate = collisionChunkPriorityUpdateQueue.Dequeue ();
				chunkToActivate.UpdateCollisionChunk (marchingGrid);
				chunkToActivate.gameObject.SetActive (true);	//Update the mesh before waking the object to prevent baking collision data twice.
			}
		}
	}

	private void RefreshCollisionChunksInRange(int xTileMin, int yTileMin, int xTileMax, int yTileMax){
		Coord minTileCoord = new Coord ((int)Mathf.Max (0, xTileMin), (int)Mathf.Max (0, yTileMin));
		Coord maxTileCoord = new Coord ((int)Mathf.Min (worldSizeX, xTileMax), (int)Mathf.Min (worldSizeY, yTileMax));

		minTileCoord = minTileCoord.ConvertTileCoordToChunkCoord (chunkSize);
		maxTileCoord = maxTileCoord.ConvertTileCoordToChunkCoord (chunkSize);

		minTileCoord.X = Mathf.Max (0, minTileCoord.X-1);
		minTileCoord.Y = Mathf.Max (0, minTileCoord.Y-1);
		maxTileCoord.X = Mathf.Min (numChunks, maxTileCoord.X+ 1);
		maxTileCoord.Y = Mathf.Min (numChunks, maxTileCoord.Y + 1);

		for (int i = minTileCoord.X; i < maxTileCoord.X; i++) {
			for (int j = minTileCoord.Y; j < maxTileCoord.Y; j++) {
				FlagCollisionChunkForPriorityUpdate(collisionChunkArray[i, j]);
			}
		}
	}
}
