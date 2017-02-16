using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWheel : MonoBehaviour {
	public ItemIcon itemIconPrefab;

	private ItemIcon[] itemIcons;

	// Use this for initialization
	void Start () {
		itemIcons = new ItemIcon[9];
		CreateIcons ();
		HighlightIcon (0);
		HideItemWheel ();
	}

	private void CreateIcons(){
		for(int i = 0; i < 9; i++){
			itemIcons[i] = Instantiate (itemIconPrefab, this.transform);
			itemIcons [i].transform.localPosition = GetIconLocalPosition (i); 
			itemIcons [i].InitialiseItemIcon ();
		}
	}

	public void HighlightIcon(int iconNumber){
		foreach (ItemIcon icon in itemIcons) {
			icon.DeHighlightIcon ();
		}
		itemIcons [iconNumber].HighlightIcon ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadSpritesIntoIcons(List<Item> itemList){
		for (int i = 0; i < itemList.Count; i++) {
			Item item = itemList [i];
			SpriteRenderer spriteRend = item.GetComponentInChildren<SpriteRenderer> ();
			itemIcons [i].SetCopyIconSpriteAndColor (spriteRend);
		}
		for (int i = itemList.Count; i < 9; i++) {
			itemIcons [i].SetIconToBlank ();
		}
	}

	public void ShowItemWheel(){
		SetIconVisibility (1f);
	}

	public void HideItemWheel(){
		SetIconVisibility (0f);
	}

	private void SetIconVisibility(float alpha){
		foreach (ItemIcon icon in itemIcons) {
			icon.SetAlpha (alpha);
		}
	}

	private Vector3 GetIconLocalPosition(int iconNumber){
		float cardinalDist = 1f;
		float diagDist = 0.707f;

		switch (iconNumber) {
		case 0:
			return new Vector3 (0f, 0f, 0f);
			break;
		case 1:
			return new Vector3 (0f, cardinalDist, 0f);
			break;
		case 2:
			return new Vector3 (diagDist, diagDist, 0f);
			break;
		case 3:
			return new Vector3 (cardinalDist, 0f, 0f);
			break;
		case 4:
			return new Vector3 (diagDist, -diagDist, 0f);
			break;
		case 5:
			return new Vector3 (0f, -cardinalDist, 0f);
			break;
		case 6:
			return new Vector3 (-diagDist, -diagDist, 0f);
			break;
		case 7:
			return new Vector3 (-cardinalDist, 0f, 0f);
			break;
		case 8:
			return new Vector3 (-diagDist, diagDist, 0f);
			break;

		default:
			return new Vector3 (0f, 0f, 0f);
		}
	}
}
