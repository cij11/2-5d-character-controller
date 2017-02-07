using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StampCollection : MonoBehaviour, IStampable {

	public void ApplyStamp(MarchingSquaresGrid marchingGrid, OreGrid oreGrid){
		IStampable[] stampables =  this.transform.GetComponentsInChildren<IStampable> ();
		foreach (IStampable stampable in stampables) {
			if (!stampable.Equals (this)) {
				stampable.ApplyStamp (marchingGrid, oreGrid);
			}
		}
	}

	//Function to aid editing. Stamps are applied sequentially, with later (lower down the child list) stamps overriding earlier
	//ones. Later stamps are pushed forward along the z axis, so that they are visible on top of the stamps they will override.
	void Update(){
		for (int i = 0; i < transform.childCount; i++) {
			Transform childTransform = transform.GetChild (i);
			childTransform.position = new Vector3 (childTransform.position.x, childTransform.position.y, this.transform.position.z -i);
		}
	}
}
