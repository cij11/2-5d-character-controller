using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBank : MonoBehaviour {
	public int maxHearts = 3;
	int currentHearts;
	HealthTank healthTank;

	bool isOutOfHearts = false;

	// Use this for initialization
	void Start () {
		currentHearts = maxHearts;
		healthTank = new HealthTank ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isOutOfHearts) {
			healthTank.UpdateHealth ();
		}
	}

	public void TakeDamage(int damageAmount){
		healthTank.TakeDamage (damageAmount);
		if (healthTank.GetIsTankEmpty ()) {
			DestroyTank ();
		}
	}

	private void DestroyTank(){
		currentHearts--;
		if (currentHearts < 0) {
			isOutOfHearts = true;
		} else {
			CreateNextTank ();
		}
	}

	public void RestoreHeart(){
		if (currentHearts < maxHearts) {
			currentHearts++;
		}
		CreateNextTank ();
	}

	private void CreateNextTank(){
		healthTank = new HealthTank ();
	}

	public bool GetIsOutOfHearts(){
		return isOutOfHearts;
	}

	public int GetMaxHearts(){
		return maxHearts;
	}

	public int GetCurrentHearts(){
		return currentHearts;
	}

	public float GetMaxHealth(){
		return healthTank.GetMaxHealth ();
	}

	public float GetCurrentHealth(){
		return healthTank.GetCurrentHealth ();
	}
}