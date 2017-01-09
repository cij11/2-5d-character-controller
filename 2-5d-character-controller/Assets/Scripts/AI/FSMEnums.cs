public enum MotorAction{
    IDLE, 
    MOVELEFT, MOVERIGHT, 
    MOVEHORIZONTALTOWARDSTARGET, MOVETOWARDSTARGET, MOVEAWAYFROMTARGET, MOVEBEHINDTARGET,
    JUMP, FALLONTARGET,
    AIMTARGET, RELEASEFIRETARGET 
};

public enum Condition{
    TIMER,
    ONEFRAME
};