using UnityEngine;
using System.Collections.Generic;

public class FSMTransition{
    private string state;
    private List<FSMExpression> expressions;


    public FSMTransition(string state){
        this.state = state;
        expressions = new List<FSMExpression>();
    }

    public void AddExpression(FSMExpression expression){
        expressions.Add(expression);
    }

    //Add a new expression by supplying the constructor parameters for it
    public void AddExpression(Condition ncondition, float nparam, bool ntruth){
        FSMExpression newExp = new FSMExpression();
        newExp.condition = ncondition;
        newExp.param = nparam;
        newExp.trueIfConditionTrue = ntruth;
        expressions.Add(newExp);
    }

    public List<FSMExpression> GetExpressions(){
        return expressions;
    }

    public string GetState(){
        return state;
    }
}