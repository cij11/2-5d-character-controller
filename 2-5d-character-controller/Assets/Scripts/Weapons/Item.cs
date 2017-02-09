using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public Rigidbody body;
	public ItemTrigger itemTrigger;
	public Vector3 gripOffset;
	protected SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = this.transform.GetComponentInChildren<SpriteRenderer> () as SpriteRenderer;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ThrowItem(Vector3 direction, float speed){
		this.transform.parent = null;
		body.isKinematic = false;
		body.velocity = direction.normalized * speed;
		itemTrigger.enabled = true;
		itemTrigger.EnableSphereCollider();
	}

	public void DrawItem(){
		body.isKinematic = true;
		itemTrigger.DisableSphereCollider();
		itemTrigger.enabled = false;
	}

	public SpriteRenderer GetSpriteRenderer(){
		if (spriteRenderer != null) {
			return spriteRenderer;
		} else {
			return null;
		}
	}

	public void SetVisibility(bool visible){
		spriteRenderer.enabled = visible;
	}
}
