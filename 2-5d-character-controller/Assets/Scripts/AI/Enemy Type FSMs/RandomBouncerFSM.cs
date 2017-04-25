using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBouncerFSM : FSMLoader {


	protected override void SeriallyLoadStates(){
		AddState ("float_randomly", Action.RUN_SUB_FSM, "pick_random_rose");
		AddTransistion ("attack", Condition.TARGET_IN_RADIUS, 5, true);

		AddState ("attack", Action.RUN_SUB_FSM, "aim_target");
		AddTransistion ("float_randomly", Condition.TARGET_IN_RADIUS, 5, false);

		AddState ("pick_random_rose", Action.PICK_RANDOM_ROSE_DIRECTION);
		AddTransistion ("move_rose_direction", Condition.FRAMES, 2, true);

		AddState ("move_rose_direction", Action.MOVE_ROSE_DIRECTION);
		AddTransistion ("pick_random_rose", Condition.TIMER, 3, true);

		AddState ("aim_target", Action.AIM_TARGET);
		AddTransistion ("shoot_target", Condition.TIMER, 2, true);

		AddState ("shoot_target", Action.RELEASE_FIRE_TARGET);
		AddTransistion ("aim_target", Condition.TIMER, 0.1f, true);
	}

	protected override void ChooseStartingState(){
		startingStateName = "float_randomly";
	}
}
	