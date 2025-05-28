using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState : BaseState
{
    protected Player player;

    public PlayerState(Player _player)
    {
        player = _player;
    }
    public override void Enter(){}

    public override void Update()
    {
        if (player.GetIsJumped() && player.isGrounded)
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Jump]);
    }

    public override void Exit(){}
}

public class Player_Idle : PlayerState
{
    public Player_Idle(Player _player) : base(_player) { HasPhysics = false; }

    public override void Enter()
    {
        player.animator.Play(player.Idle_Hash);
        player.rigid.velocity = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        if (Mathf.Abs(player.inputX) > 0.1f)
        {
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Run]);
        }
    }
    public override void Exit(){}
}

public class Player_Run : PlayerState
{
    public Player_Run(Player _player) : base(_player){ HasPhysics = true; }
    
    public override void Enter()
    {
        player.animator.Play(player.Run_Hash);
    }

    public override void Update()
    {
        base.Update();
        if (Mathf.Abs(player.inputX) < 0.1f)
        {
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
        }

        if (player.inputX < 0)
        {
            player.spriteRenderer.flipX = true;
        }
        else
        {
            player.spriteRenderer.flipX = false;
        }
    }

    public override void FixedUpdate()
    {
        player.rigid.velocity = new Vector2(player.inputX * player.MoveSpeed, player.rigid.velocity.y);
    }
    
    public override void Exit(){}
}

public class Player_Jump : PlayerState
{
    public Player_Jump(Player _player) : base(_player) { HasPhysics = true; }

    public override void Enter()
    {
        player.animator.Play(player.Jump_Hash);
        player.rigid.AddForce(Vector2.up * player.JumpForce, ForceMode2D.Impulse);

        player.SetGrounded(false);
    }

    public override void Update()
    {
        if (player.inputX < 0)
        {
            player.spriteRenderer.flipX = true;
        }
        else
        {
            player.spriteRenderer.flipX = false;
        }
        
        if(player.isGrounded)
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
    }

    public override void FixedUpdate()
    {
        player.rigid.velocity = new Vector2(player.inputX * player.MoveSpeed, player.rigid.velocity.y);
    }
}