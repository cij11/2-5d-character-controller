using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloaterBouncerFSM : FSMLoader {


	protected override void SeriallyLoadStates(){
		AddState ("float_towards", Action.RUN_SUB_FSM, "aim_towards_target");
		AddTransistion ("attack", Condition.TARGET_IN_RADIUS, 5, true);

		AddState ("attack", Action.RUN_SUB_FSM, "aim_target");
		AddTransistion ("float_towards", Condition.TARGET_IN_RADIUS, 5, false);

		AddState ("aim_towards_target", Action.AIM_TARGET);
		AddTransistion ("float_current_direction", Condition.FRAMES, 2, true);

		AddState ("float_current_direction", Action.PRESS_FIRE);
		AddTransistion ("aim_towards_target", Condition.TIMER, 3, true);

		AddState ("aim_target", Action.AIM_TARGET);
		AddTransistion ("shoot_target", Condition.TIMER, 2, true);

		AddState ("shoot_target", Action.RELEASE_FIRE_TARGET);
		AddTransistion ("aim_target", Condition.TIMER, 0.1f, true);
	}

	protected override void ChooseStartingState(){
		startingStateName = "float_towards";
	}
}
	