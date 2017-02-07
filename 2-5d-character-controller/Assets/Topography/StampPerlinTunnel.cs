using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampPerlinTunnel :  MonoBehaviour, IStampable {

	public void ApplyStamp(MarchingSquaresGrid marchingGrid){

		new MarchingSquaresCutTools (marchingGrid.GetNodeArray ()).DigPerlinTunnels (0.02f);
		Destroy (this.gameObject);
	}
}
