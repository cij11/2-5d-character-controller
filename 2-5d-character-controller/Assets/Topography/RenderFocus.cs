using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderFocus : MonoBehaviour {

	Topography topography;
	bool isAddedToTopography = false;

	// Use this for initialization
	void Start () {
		topography = (Topography)FindObjectOfType (typeof(Topography));
	}
	
	// Update is called once per frame
	void Update () {
		if (!isAddedToTopography) {
			isAddedToTopography = topography.AddRenderFocus (this.gameObject);
		}
	}
}
