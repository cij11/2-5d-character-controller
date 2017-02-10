using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public Rigidbody body;
	public ItemTrigger itemTrigger;
	public Vector3 gripOffset;
	protected SpriteRenderer spriteRenderer;

	protected Transform barrelTransform;
	protected Vector3 localBarrelPosition;

	// Use this for initialization
	void Start () {
		spriteRenderer = this.transform.GetComponentInChildren<SpriteRenderer> () as SpriteRenderer;
		Barrel barrel = transform.GetComponentInChildren<Barrel> () as Barrel;
		if (barrel != null) {
			barrelTransform = barrel.transform;
			localBarrelPosition = barrelTransform.localPosition;
		} else {
			barrelTransform = this.transform;
			localBarrelPosition = new Vector3 (0f, 0f, 0f);
		}
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

	public Transform GetBarrelTransform(){
		if (barrelTransform != null) {
			return barrelTransform;
		} else {
			return this.transform;
		}
	}

	public void FlipBarrel(bool isFlipped){
		if (barrelTransform != null) {
			if (!isFlipped) {
				barrelTransform.localPosition = localBarrelPosition;
			} else {
				barrelTransform.localPosition = new Vector3 (-localBarrelPosition.x, localBarrelPosition.y, 0f);
			}
		}
	}
}
