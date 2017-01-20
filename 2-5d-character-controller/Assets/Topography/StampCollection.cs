using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampCollection : MonoBehaviour, IStampable {

	public void ApplyStamp(MarchingSquaresGrid marchingGrid){
		print ("Apply stamp invoked in stamp collection");
		IStampable[] stampables =  this.transform.GetComponentsInChildren<IStampable> ();
		foreach (IStampable stampable in stampables) {
			if (!stampable.Equals (this)) {
				stampable.ApplyStamp (marchingGrid);
			}
		}
	}
}
