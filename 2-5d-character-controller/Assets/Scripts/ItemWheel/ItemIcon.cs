using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemIcon : MonoBehaviour {
	SpriteRenderer spriteRenderer;
	Color itemColor;
	// Use this for initialization
	void Start () {
		InitialiseItemIcon ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InitialiseItemIcon(){
		spriteRenderer = GetComponent<SpriteRenderer> () as SpriteRenderer;
	}

	public void HighlightIcon(){
		SetAlpha (0.9f);
	}

	public void DeHighlightIcon(){
		SetAlpha (0.3f);
	}

	public void SetCopyIconSpriteAndColor(SpriteRenderer sourceSpriteRenderer){
		SetSprite (sourceSpriteRenderer.sprite);
		SetColor (sourceSpriteRenderer.color);
	}

	public void SetSprite(Sprite sprite){
		spriteRenderer.sprite = sprite;
	}

	public void SetColor(Color newColor){
		spriteRenderer.color = newColor;
	}

	public void SetAlpha(float alpha){
		Color oldColor = spriteRenderer.color;
		spriteRenderer.color = GetColourWithAlphaOf (spriteRenderer.color, alpha);
	}

	private Color GetColourWithAlphaOf(Color oldColor, float newAlpha){
		return new Color (oldColor.r, oldColor.g, oldColor.b, newAlpha);
	}
}
