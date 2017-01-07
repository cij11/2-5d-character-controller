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
				virtualController.PushJump();
			break;

			case 1:
				virtualController.ReleaseJump();
			break;
			case 2:
				virtualController.PushJump();
			break;

			case 3:
				virtualController.ReleaseJump();
			break;
			case 4:
				virtualController.PushFire();
			break;

			case 5:
				virtualController.ReleaseFire();
			break;
			default:
				virtualController.PushHorizAxis(-1f);
				break;
		}
	}
}
