using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretFSM : FSMLoader {


	protected override void SeriallyLoadStates(){
		AddState ("idle", Action.RUN_SUB_FSM, "float_in_place");
		AddTransistion ("attack", Condition.TARGET_IN_RADIUS, 16, true);

		AddState ("attack", Action.RUN_SUB_FSM, "aim_target");
		AddTransistion ("idle", Condition.TARGET_IN_RADIUS, 16, false);

		AddState ("float_in_place", Action.IDLE);

		AddState ("aim_target", Action.AIM_TARGET);
		AddTransistion ("shoot_target", Condition.TIMER, 2, true);

		AddState ("shoot_target", Action.RELEASE_FIRE_TARGET);
		AddTransistion ("aim_target", Condition.TIMER, 0.1f, true);
	}

	protected override void ChooseStartingState(){
		startingStateName = "idle";
	}
}
	