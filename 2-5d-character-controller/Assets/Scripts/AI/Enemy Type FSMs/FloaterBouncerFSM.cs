using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloaterBouncerFSM : FSMLoader {


	protected override void SeriallyLoadStates(){
		AddState ("float_towards", Action.RUN_SUB_FSM, "float_towards_target");
		AddTransistion ("attack", Condition.TARGET_IN_RADIUS, 5, true);

		AddState ("attack", Action.RUN_SUB_FSM, "aim_target");
		AddTransistion ("float_towards", Condition.TARGET_IN_RADIUS, 5, false);

		AddState ("float_towards_target", Action.AIM_TARGET);
		AddTransistion ("move_random_rose_direction", Condition.ALWAYS, 1, false);

		AddState ("aim_target", Action.AIM_TARGET);
		AddTransistion ("shoot_target", Condition.TIMER, 2, true);

		AddState ("shoot_target", Action.RELEASE_FIRE_TARGET);
		AddTransistion ("aim_target", Condition.TIMER, 0.1f, true);
	}

	protected override void ChooseStartingState(){
		startingStateName = "float_towards";
	}
}
	