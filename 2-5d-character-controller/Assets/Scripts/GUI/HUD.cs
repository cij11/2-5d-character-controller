using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
	LineRenderer lineRenderer;
	HeartBank heartBank;

	float healthBarMaxLength = 1f;
	float healthbarElevation = 1f;
	// Use this for initialization
	void Start () {
		lineRenderer = GetComponent<LineRenderer> () as LineRenderer;
		heartBank = GetComponentInParent<HeartBank> () as HeartBank;
	}
	
	// Update is called once per frame
	void Update () {
		RenderHealth ();
	}

	void RenderHealth(){
		Color healthbarColor = InterpolateColorFromGreenToRedFromRemainingHearts ();
		float healthbarLength = InterpolateLengthFromRemainingHealth ();
		Vector3 startPoint = GetHealthbarStart ();
		Vector3 endPoint = GetHealthbarEnd (startPoint, healthbarLength);
		DrawLine (startPoint, endPoint, healthbarColor);
	}

	private Color InterpolateColorFromGreenToRedFromRemainingHearts (){
		float remainingHeartPercentage = ((float)heartBank.GetCurrentHearts () + 0.1f) / ((float)heartBank.GetMaxHearts () + 0.1f); //Add 0.1 avoid 0/0 division when character has no hearts.
		return new Color ((1f - remainingHeartPercentage), remainingHeartPercentage, 0f, 1f);
	}

	private float InterpolateLengthFromRemainingHealth (){
		float healthPercent = heartBank.GetCurrentHealth () / heartBank.GetMaxHealth ();
		return healthPercent * healthBarMaxLength;
	}

	private Vector3 GetHealthbarStart(){
		return this.transform.position - this.transform.right * (healthBarMaxLength / 2f) + this.transform.up * healthbarElevation;
	}

	private Vector3 GetHealthbarEnd(Vector3 startPosition, float healthbarLength){
		return startPosition + this.transform.right * healthbarLength;
	}

	void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		lineRenderer.SetColors(color, color);
		lineRenderer.SetWidth(0.1f, 0.1f);
		lineRenderer.SetPosition(0, start);
		lineRenderer.SetPosition(1, end);
	}
}
