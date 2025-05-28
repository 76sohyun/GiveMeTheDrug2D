using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private Dictionary<Estate, BaseState> stateDic;
    public Dictionary<Estate, BaseState> StateDic => stateDic;
    public BaseState CurState;
    public StateMachine()
    {
        stateDic = new Dictionary<Estate, BaseState>();
    }

    public void ChangeState(BaseState changedState)
    {
        if (CurState == changedState)
            return;

        CurState.Exit(); 
        CurState = changedState;
        CurState.Enter(); 
    }
    public void Update() => CurState.Update();

    public void FixedUpdate()
    {
        if(CurState.HasPhysics)
        CurState.FixedUpdate();
    }
}
