public enum Action{
    IDLE, 
    MOVE_LEFT, MOVE_RIGHT, MOVE_FORWARD,
	CHANGE_DIRECTION,
    MOVEHORIZONTAL_TOWARDS_TARGET, MOVE_TOWARDS_TARGET, MOVE_AWAY_FROM_TARGET, MOVE_BEHIND_TARGET,
    JUMP, FALL_ON_TARGET,
	PRESS_JUMP, RELEASE_JUMP,
	AIM_TARGET, RELEASE_FIRE_TARGET, PRESS_FIRE, FIRE, RELEASE_FIRE,
	RUN_SUB_FSM,
	PICK_RANDOM_ROSE_DIRECTION, PICK_RANDOM_ROSE_DIRECTION_TOWARDS_TARGET,
	MOVE_ROSE_DIRECTION
};

public enum Condition{
    TIMER,
	FRAMES,
    TARGET_IN_RADIUS, TARGET_OUTSIDE_RADIUS, 
    TARGET_IN_LOS,
    TARGET_IN_OCTANT_BELOW, TARGET_IN_OCTANT_ABOVE,
    TARGET_IN_HORIZONTAL_OCTANTS,
	TARGET_IN_FRONT_OCTANT,
	TARGET_IN_FRONT_HORIZONTAL,
    CLIFF_LEFT, CLIFF_RIGHT, CLIFF_FORWARD,
	ALWAYS
};