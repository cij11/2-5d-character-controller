using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampPerlinTunnel :  MonoBehaviour, IStampable {

	public void ApplyStamp(MarchingSquaresGrid marchingGrid, OreGrid oreGrid){

		new MarchingSquaresCutTools (marchingGrid).DigPerlinTunnels (0.02f);
		Destroy (this.gameObject);
	}
}
