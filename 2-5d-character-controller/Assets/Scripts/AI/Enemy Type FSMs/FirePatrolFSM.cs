﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePatrolFSM : FSMLoader {

	protected override void SeriallyLoadStates(){

		AddState ("patrol", Action.RUN_SUB_FSM, "move_forward");
		AddTransistion ("fire", Condition.TIMER, 1, true);

		AddState("fire", Action.RUN_SUB_FSM, "tap_fire");
		AddTransistion ("patrol", Condition.FRAMES, 8, true);

		AddState("tap_fire", Action.PRESS_FIRE);
		AddTransistion ("release_fire", Condition.FRAMES, 3, true);

		AddState("release_fire", Action.RELEASE_FIRE);
		AddTransistion ("release_fire", Condition.ALWAYS,1 , false);


		AddState ("move_forward", Action.MOVE_FORWARD);
		AddTransistion ("change_direction", Condition.CLIFF_FORWARD, 1, true);

		AddState ("change_direction", Action.CHANGE_DIRECTION);
		AddTransistion ("move_forward", Condition.FRAMES, 1, true);
	}
}
