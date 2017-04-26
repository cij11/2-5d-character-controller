using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttackFSM : FSMLoader {

	protected override void SeriallyLoadStates(){
		//SuperFSM states
		AddState ("patrol", Action.RUN_SUB_FSM, "start_patrol");
		AddTransistion ("attack", Condition.TARGET_IN_FRONT_HORIZONTAL, 1, true);
		AddExpression (Condition.TARGET_IN_RADIUS, 5, true); //By adding an expression to an existing transition, these two condtions are 'ANDed' together.

		AddState ("attack", Action.RUN_SUB_FSM, "jump");
	//	AddTransistion ("patrol", Condition.TARGET_IN_FRONT_HORIZONTAL, 1, false); //By adding two seperate transitions, these two conditions are 'ORed' together.
		AddTransistion ("patrol", Condition.TARGET_IN_RADIUS, 5f, false);

		//Patrol states
		AddState ("start_patrol", Action.IDLE);
		AddTransistion ("move_forward", Condition.TIMER, 0.1f, true);

		AddState ("move_forward", Action.MOVE_FORWARD);
		AddTransistion ("change_direction", Condition.CLIFF_FORWARD, 1, true);

		AddState ("change_direction", Action.CHANGE_DIRECTION);
		AddTransistion ("move_forward", Condition.FRAMES, 1, true);

		//Jump attack states
		AddState ("jump", Action.PRESS_JUMP);
		AddTransistion ("stop_jump", Condition.FRAMES, 2, true);

		AddState ("stop_jump", Action.RELEASE_JUMP);
		AddTransistion ("aim_at_target", Condition.FRAMES, 2, true);

		AddState ("aim_at_target", Action.AIM_TARGET);
		AddTransistion ("fire_at_target", Condition.TIMER, 1f, true);

		AddState ("fire_at_target", Action.RELEASE_FIRE_TARGET);
		AddTransistion ("finish_jump", Condition.TIMER, 0.1f, true);

		AddState ("finish_jump", Action.IDLE);
		AddTransistion ("change_direction_on_landing", Condition.TIMER, 1f, true);

		AddState ("change_direction_on_landing", Action.CHANGE_DIRECTION);
		AddTransistion ("jump", Condition.FRAMES, 1, true);
	}
}
