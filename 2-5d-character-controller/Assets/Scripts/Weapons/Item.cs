﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public Rigidbody body;
	public ItemTrigger itemTrigger;
	public Vector3 gripOffset;
	protected SpriteRenderer spriteRenderer;

	protected Transform barrelTransform;
	protected Vector3 localBarrelPosition;

	private ItemState itemState;

	private float gravityForce = 500f;

	int throwDamage = 50;

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
		itemState = ItemState.SPAWN_HOVER;
	}
	
	// Update is called once per frame
	void Update () {
		if (itemState == ItemState.DISCARDED || itemState == ItemState.THROWN) {
			ApplyGravity ();
		}
	}

	private void ApplyGravity(){
		body.AddForce (GravityManager.instance.GetDownVector (body.transform.position) * gravityForce * Time.deltaTime);
	}

	public void ThrowItem(Vector3 direction, float speed){
		itemState = ItemState.THROWN;
		this.gameObject.layer = 13; //Add to projectile layer
		this.transform.parent = null;
		body.isKinematic = false;
		body.velocity = direction.normalized * speed;
		itemTrigger.enabled = true;
		itemTrigger.EnableSphereCollider();
	}

	public void DrawItem(){
		itemState = ItemState.HELD;
		this.gameObject.layer = 9; //Add to only collide with obstacles layer.
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

	void OnCollisionEnter(Collision other) {
		if (itemState == ItemState.THROWN) {
			CharacterCorpus corpus = other.collider.GetComponent<CharacterCorpus> () as CharacterCorpus;
			if (corpus != null) {
				corpus.TakeDamage (throwDamage);
				itemState = ItemState.DISCARDED;
				this.gameObject.layer = 9; // Add to phasing layer
			}
			if (other.gameObject.layer == 8) {
				itemState = ItemState.DISCARDED;
				this.gameObject.layer = 9; //Add to phasing layer
			}
		}
	}
}
