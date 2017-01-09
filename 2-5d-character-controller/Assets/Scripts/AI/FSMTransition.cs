using UnityEngine;


public class FSMTransition{
    private Condition condition;
    private float param;
    private string state;

    public FSMTransition(Condition con, float cparam, string nstate){
        condition = con;
        param = cparam;
        state = nstate;
    }

    public Condition GetCondition(){
        return condition;
    }
    public float GetParam(){
        return param;
    }
    public string GetState(){
        return state;
    }
}