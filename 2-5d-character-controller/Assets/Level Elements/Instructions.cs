using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour {
	float fadeAlpha = 1.0f;
	public float persistTime = 8;
	public string text = "Placeholder";

	public bool bottomOfScreen = false;

	// Use this for initialization
	void Start () {
		Invoke ("FadeOut", persistTime);
	}
		
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		if(!bottomOfScreen)
			style.alignment = TextAnchor.UpperCenter;
		else
			style.alignment = TextAnchor.UpperCenter;
		
		style.fontSize = h * 8 / 100;
		style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, fadeAlpha);

		if (!bottomOfScreen) {
			GUI.Label (rect, "\n\n\n" + text, style);
		} else {
			GUI.Label (rect, "\n\n\n\n\n\n\n WASD to move. \n Space to jump and wall-jump  \n J to aim and fire \n K to pickup and cycle weapons", style);
		}
	}

	void FadeOut(){
		fadeAlpha -= Time.deltaTime;
		if (fadeAlpha < 0) {
			Destroy (this.gameObject);
		}
		Invoke ("FadeOut", Time.deltaTime);
	}
}
