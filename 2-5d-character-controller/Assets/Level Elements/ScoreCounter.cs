using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour {
	int numKills = 0;
	float timeSurvived = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timeSurvived += Time.deltaTime;
	}

	public void IncrementKills(){
		numKills++;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = h * 4 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		string text = "Score = " + numKills.ToString () + "\n" + "Time = " + ((int)timeSurvived).ToString();
		GUI.Label(rect, text, style);
	}
}
