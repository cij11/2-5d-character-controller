using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour {
	float fadeAlpha = 1.0f;
	public string text = "Placeholder";

	// Use this for initialization
	void Start () {
		Invoke ("FadeOut", 8);
	}
		
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = h * 8 / 100;
		style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, fadeAlpha);
		GUI.Label(rect, "\n\n\n" + text, style);
	}

	void FadeOut(){
		fadeAlpha -= Time.deltaTime;
		if (fadeAlpha < 0) {
			Destroy (this.gameObject);
		}
		Invoke ("FadeOut", Time.deltaTime);
	}
}
