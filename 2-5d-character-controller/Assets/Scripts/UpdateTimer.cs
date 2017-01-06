using UnityEngine;
using System.Collections;

public class UpdateTimer {
	int updatePeriod;
	int timer;
	public UpdateTimer(int period){
		if (period > 0){
			updatePeriod = period;
		}
		else{
			updatePeriod = 1;
		}
		timer = Random.Range(1, updatePeriod + 1);
	}

	public bool TryUpdateThisTick(){
		timer--;
		if (timer == 0){
			timer = updatePeriod;
			return true;
		}
		return false;
	}
}
