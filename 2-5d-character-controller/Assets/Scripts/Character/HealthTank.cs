using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTank {
	float maxHealth;
	float currentHealth;

	float rechangeRate;
	float rechangeStunPeriod;
	float rechargeStunTimer;

	public HealthTank(){
		maxHealth = 100;
		currentHealth = maxHealth;

		rechangeRate = 40f;
		rechangeStunPeriod = 8f;
		rechargeStunTimer = 0f;
	}

	public void TakeDamage(int damageAmount){
		DecreaseHealth (damageAmount);
		StunRecharge ();
	}

	void DecreaseHealth(float damageAmount){
		currentHealth -= damageAmount;
		if (currentHealth < 0) {
			currentHealth = 0;
		}
	}

	void StunRecharge(){
		rechargeStunTimer = rechangeStunPeriod;
	}

	public void UpdateHealth(){
		if (rechargeStunTimer > 0) {
			rechargeStunTimer -= Time.deltaTime;
		} else {
			RechargeHealth ();
		}
	}

	void RechargeHealth(){
		if (currentHealth < maxHealth) {
			currentHealth += rechangeRate * Time.deltaTime;
		}
	}

	public float GetMaxHealth(){
		return (int)maxHealth;
	}

	public float GetCurrentHealth(){
		return (int)currentHealth;
	}

	public bool GetIsTankEmpty(){
		if (currentHealth <= 0) {
			return true;
		} else {
			return false;
		}
	}
}