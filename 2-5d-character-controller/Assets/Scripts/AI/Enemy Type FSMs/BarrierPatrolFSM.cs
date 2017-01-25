using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPatrolFSM : FSMLoader {

	protected override void SeriallyLoadStates(){
		AddState ("patrol", Action.RUN_SUB_FSM, "start_patrol");
		AddTransistion ("attack", Condition.TARGET_IN_RADIUS, 3, true);

		AddState ("attack", Action.RUN_SUB_FSM, "aim_target");
		AddTransistion ("patrol", Condition.TARGET_IN_RADIUS, 3, false);

		AddState ("start_patrol", Action.IDLE);
		AddTransistion ("move_forward", Condition.TIMER, 1, true);

		AddState ("move_forward", Action.MOVE_FORWARD);
		AddTransistion ("change_direction", Condition.CLIFF_FORWARD, 1, true);

		AddState ("change_direction", Action.CHANGE_DIRECTION);
		AddTransistion ("move_forward", Condition.FRAMES, 1, true);

		AddState ("aim_target", Action.AIM_TARGET);
		AddTransistion ("shoot_target", Condition.TIMER, 2, true);

		AddState ("shoot_target", Action.RELEASE_FIRE_TARGET);
		AddTransistion ("aim_target", Condition.TIMER, 0.1f, true);
	}
}
