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
        player.sr.flipX = player.spriteRenderer.flipX;
        if (!player.isClimb)
        {
            if (player.inputX < 0)
            {
                player.spriteRenderer.flipX = true;
            }
            else if (player.inputX > 0)
            {
                player.spriteRenderer.flipX = false;
            }
        }
        
        if (player.GetIsJumped() && player.isGrounded)
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Jump]);

        if (player.isDash && player.isGrounded)
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Dash]);
        
        if (player.isClimb && player.stateMachine.CurState != player.stateMachine.StateDic[Estate.Climb])
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Climb]);

        if (player.isHang && !player.isClimb && !player.isGrounded && player.stateMachine.CurState != player.stateMachine.StateDic[Estate.CeilingClimb])
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.CeilingClimb]);
        
        if(Input.GetKeyDown(KeyCode.Mouse1) && player.isAttack)
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Attack1]);
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
    }

    public override void FixedUpdate()
    {
        player.rigid.velocity = new Vector2(player.inputX * player.MoveSpeed, player.rigid.velocity.y);
    }

    public override void Exit() {}
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
        base.Update();
        if(player.isGrounded)
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
    }

    public override void FixedUpdate()
    {
        player.rigid.velocity = new Vector2(player.inputX * player.MoveSpeed, player.rigid.velocity.y);
    }
}

public class Player_Dash : PlayerState
{
    public Player_Dash(Player _player) : base(_player) { HasPhysics = true; }
    
    public override void Enter()
    {
        player.animator.Play(player.Dash_Hash);
        player.currentSpeed = player.DashSpeed;
        player.dashTime = player.DefaultTime;
    }

    public override void Update()
    {
        player.dashTime -= Time.deltaTime;

        if (player.dashTime <= 0)
        {
            player.currentSpeed = player.MoveSpeed;
            player.SetDash(false,Player.DashType.Normal);
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
            player.SetLock(false);
        }
        
        if (player._playerLay.isdash && player.targetMonster != null)
        {
            float MonsterDistance = Vector2.Distance(player.transform.position, player.targetMonster.transform.position);
            Vector2 direction = (player.targetMonster.position - player.transform.position).normalized;
            player.transform.Translate(direction * player.MonsterDashSpeed * Time.deltaTime);

            if (MonsterDistance < 1f)
            {
                player.transform.position = player.targetMonster.transform.position;
                player.SetLock(true);
                player.rigid.velocity = Vector2.zero;
                player.rigid.gravityScale = 0f;
                player.SetDash(false,Player.DashType.Monster);
                player._playerLay.isdash = false;
                player.targetMonster = null;
                Debug.Log("슬래시 진입");
                player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Slash]);
            }
        }
    }

    public override void FixedUpdate()
    {
        player.rigid.velocity = new Vector2(player.inputX * player.currentSpeed, player.rigid.velocity.y);
    }
}

public class Player_Climb : PlayerState
{
    public Player_Climb(Player _player) : base(_player) { HasPhysics = true; }
    
    public override void Enter()
    {
        player.animator.Play(player.Climb_Hash);
    }

    public override void Update()
    {
        float inputY = Input.GetAxis("Vertical");
        if(Mathf.Abs(inputY) < 0.1f)
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.ClimbIdle]);
        if(player.isGrounded)
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
    }

    public override void FixedUpdate()
    {
        float ver = Input.GetAxis("Vertical");
        player.rigid.velocity = new Vector2(player.rigid.velocity.x, ver * player.MoveSpeed);
    }

    public override void Exit(){}
}

public class Player_ClimbIdle : PlayerState
{
    public Player_ClimbIdle(Player _player) : base(_player) { HasPhysics = false; }

    public override void Enter()
    {
        player.animator.Play(player.ClimbIdle_Hash);
        player.rigid.velocity = Vector2.zero;
        player.rigid.gravityScale = 0f;
    }

    public override void Update()
    {
        float inputY = Input.GetAxis("Vertical");
        if (Mathf.Abs(inputY) > 0.1f)
        {
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Climb]);
        }
    }

    public override void Exit()
    {
        player.rigid.gravityScale = 3f;
    }
}

public class Player_CeilingClimb : PlayerState
{
    public Player_CeilingClimb(Player _player) : base(_player) { HasPhysics = true; }
    
    public override void Enter()
    {
        player.animator.Play(player.CeilingClimb_Hash);
        player.rigid.gravityScale = 0f;
    }

    public override void Update()
    {
        base.Update();
        if (!player.isHang)
        {
            player.rigid.gravityScale = 3f;
            Debug.Log("중력3");
            
            if(player.isGrounded)
                player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
        }
        
        Debug.Log($"Ceiling Climb : {player.inputX}");

        if (Mathf.Abs(player.inputX) < 0.1f)
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.CeilingIdle]);
        
    }

    public override void FixedUpdate()
    {
        player.rigid.velocity = new Vector2(player.inputX * player.MoveSpeed, player.rigid.velocity.y);
    }
    
    public override void Exit() {}
}

public class Player_CeilingIdle : PlayerState
{
    public Player_CeilingIdle(Player _player) : base(_player) { HasPhysics = false; }

    public override void Enter()
    {
        player.animator.Play(player.CeilingIdle_Hash);
        player.rigid.velocity = Vector2.zero;
        player.rigid.gravityScale = 0f;
    }

    public override void Update()
    {
        if (!player.isHang)
        {
            player.rigid.gravityScale = 3f;
            Debug.Log("중력3");
            
            if(player.isGrounded)
                player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
        }
        
        Debug.Log($"Ceiling Idle : {player.inputX}");

        if (Mathf.Abs(player.inputX) > 0.1f)
        {
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.CeilingClimb]);
        }
    }

    public override void Exit(){}
}

public class Player_Attack1 : PlayerState
{
    public Player_Attack1(Player _player) : base(_player) { HasPhysics = true; }

    public override void Enter()
    {
        player.animator.Play(player.Attack1_Hash);
        player.AttackStep = 1;
        Debug.Log("공격1에 진입");
        player.AttackTime = 0f;
        player.currentSpeed = player.AttackDashSpeed;
        player.attackDashTime = player.AttackDefaultTime;
        float direction = player.spriteRenderer.flipX ? -1f : 1f;
        player.rigid.velocity = new Vector2(direction * player.currentSpeed, player.rigid.velocity.y);
    }

    public override void Update()
    {
        player.AttackTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse1) && player.AttackTime < player.AttackCoolTime)
        {
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Attack2]);
        }
        else if (player.AttackTime >= player.AttackCoolTime)
        {
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
        }
        
        player.attackDashTime -= Time.deltaTime;

        if (player.attackDashTime <= 0)
        {
            player.currentSpeed = player.MoveSpeed;
        }
    }
    public override void Exit(){}
}

public class Player_Attack2 : PlayerState
{
    public Player_Attack2(Player _player) : base(_player) { HasPhysics = true; }

    public override void Enter()
    {
        player.animator.Play(player.Attack2_Hash);
        player.AttackStep = 2;
        Debug.Log("공격2에 진입");
        player.AttackTime = 0f;
        player.currentSpeed = player.AttackDashSpeed;
        player.attackDashTime = player.AttackDefaultTime;
        float direction = player.spriteRenderer.flipX ? -1f : 1f;
        player.rigid.velocity = new Vector2(direction * player.currentSpeed, player.rigid.velocity.y);
    }

    public override void Update()
    {
        player.AttackTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse1) && player.AttackTime < player.AttackCoolTime)
        {
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Attack3]);
        }
        
        if (player.AttackTime >= player.AttackCoolTime)
        {
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
        }
        
        player.attackDashTime -= Time.deltaTime;

        if (player.attackDashTime <= 0)
        {
            player.currentSpeed = player.MoveSpeed;
        }
    }
    public override void Exit(){}
}

public class Player_Attack3 : PlayerState
{
    public Player_Attack3(Player _player) : base(_player) { HasPhysics = true; }

    public override void Enter()
    {
        player.animator.Play(player.Attack3_Hash);
        player.AttackStep = 3;
        Debug.Log("공격3에 진입");
        player.AttackTime = 0f;
        player.currentSpeed = player.AttackDashSpeed;
        player.attackDashTime = player.AttackDefaultTime;
        float direction = player.spriteRenderer.flipX ? -1f : 1f;
        player.rigid.velocity = new Vector2(direction * player.currentSpeed, player.rigid.velocity.y);
    }

    public override void Update()
    {
        player.AttackTime += Time.deltaTime;
        
        if (player.AttackTime >= player.AttackCoolTime)
        {
            player.stateMachine.ChangeState(player.stateMachine.StateDic[Estate.Idle]);
        }
        
        player.attackDashTime -= Time.deltaTime;

        if (player.attackDashTime <= 0)
        {
            player.currentSpeed = player.MoveSpeed;
        }
        
    }

    public override void Exit(){}
}

public class Player_Slash : PlayerState
{
    public Player_Slash(Player _player) : base(_player) { HasPhysics = true; }

    public override void Enter()
    {
        player.animator.Play(player.Slash_Hash);
        Debug.Log("Slash에 진입");
        player.SetSlash(true);
        float direction = player.spriteRenderer.flipX ? -1f : 1f;
        
    }

    public override void Update()
    {
        if (!player.isSlash)
        {
            player.SetSlash(true);
        }
    }

    public override void Exit()
    {
        player.SetSlash(false);
        player.rigid.gravityScale = 3f;
    }
}