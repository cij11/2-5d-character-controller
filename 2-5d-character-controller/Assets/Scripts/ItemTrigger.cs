using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{

	Item item;
	Transform itemTransform;
	SphereCollider sphereCollider;
	// Use this for initialization
	void Start ()
	{
		item = GetComponentInParent<Item> () as Item;
		itemTransform = item.transform;
		sphereCollider = GetComponent<SphereCollider>() as SphereCollider;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public Transform GetWeaponTransform ()
	{
		return itemTransform;
	
	}

	public void EnableSphereCollider(){
		EnsureSphereColliderSet ();
		sphereCollider.enabled = true;
	}
	public void DisableSphereCollider(){
		EnsureSphereColliderSet();
		sphereCollider.enabled = false;
	}

	void EnsureSphereColliderSet(){
		if (sphereCollider == null) {
			sphereCollider = GetComponent<SphereCollider>() as SphereCollider;
		}
	}
}
