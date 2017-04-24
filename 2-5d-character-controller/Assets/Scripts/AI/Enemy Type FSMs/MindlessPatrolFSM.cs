using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindlessPatrolFSM : FSMLoader {

	protected override void SeriallyLoadStates(){

		AddState ("patrol", Action.IDLE);
		AddTransistion ("move_forward", Condition.TIMER, 1, true);

		AddState ("move_forward", Action.MOVE_FORWARD);
		AddTransistion ("change_direction", Condition.CLIFF_FORWARD, 1, true);

		AddState ("change_direction", Action.CHANGE_DIRECTION);
		AddTransistion ("move_forward", Condition.FRAMES, 1, true);
	}
}
