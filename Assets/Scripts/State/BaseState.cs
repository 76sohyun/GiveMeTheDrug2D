using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public bool HasPhysics;
    public abstract void Enter();
    public abstract void Update();
    public virtual void FixedUpdate(){}
    public abstract void Exit();
}

public enum Estate
{
    Idle, Run, Jump, Dash, Climb, ClimbIdle, CeilingClimb, CeilingIdle, Attack1, Attack2, Attack3, DroneIdle
}
