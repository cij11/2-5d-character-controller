using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {
	protected Projectile projectile;
	// Use this for initialization
	void Start () {
		projectile = GetComponent<Projectile> () as Projectile;
	}
	
	public virtual void PerformBehaviour(CharacterComponentData compData){

	}
}
