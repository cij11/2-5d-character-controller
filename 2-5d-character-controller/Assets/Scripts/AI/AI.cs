using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {
	UpdateTimer updateTimer;
	int action = 0;
	AIVirtualController virtualController;
	// Use this for initialization
	void Start () {
		virtualController = GetComponent<AIVirtualController>() as AIVirtualController;
		updateTimer = new UpdateTimer(60);
	}
	
	// Update is called once per frame
	void Update () {
		if(updateTimer.TryUpdateThisTick()){
			action++;
		}
		switch (action){
			case 0:
				virtualController.TapFire();
			break;

			case 1:
				virtualController.PushSwap();
			break;
			case 2:
				virtualController.ReleaseSwap();
			break;

			case 3:
				virtualController.TapFire();
			break;
			case 4:
				virtualController.TapJump();
			break;

			case 5:
				virtualController.TapFire();
			break;
			default:
				virtualController.PushHorizAxis(-1f);
				break;
		}
	}
}
