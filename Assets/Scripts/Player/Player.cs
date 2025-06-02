using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Player : MonoBehaviour
{
     [Header("Player Settings")]
     [Tooltip("최대 체력")]
     [SerializeField] private int MaxHealth;
     public float maxHealth => MaxHealth;
     [Tooltip("이동속도")]
     [SerializeField] private float moveSpeed;
     public float MoveSpeed => moveSpeed;
     [Tooltip("점프할 때 작용하는 힘")]
     [SerializeField] private float jumpForce;
     public float JumpForce => jumpForce;
     [Tooltip("대시 속도")]
     [SerializeField]private float dashSpeed;
     public float DashSpeed => dashSpeed;
     [Tooltip("대시 지속 시간")]
     [SerializeField]private float defaultTime;
     public float DefaultTime => defaultTime;
     [Tooltip("몬스터 대시 스피드")]
     [SerializeField] private float monsterDashSpeed;
     public float MonsterDashSpeed => monsterDashSpeed;
     [Header("Animation Settings")]
     [Tooltip("FX")]
     [SerializeField]private GameObject fx;
     public GameObject FxObj => fx;
     [Header("Attack Settings")]
     [Tooltip("공격 시간")] 
     [SerializeField] private float attackCoolTime;
     public float AttackCoolTime
     {
          get => attackCoolTime;
          set => attackCoolTime = value;
     }
     [Tooltip("공격 대시 속도")]
     [SerializeField] private float attackDashSpeed;
     public float AttackDashSpeed => attackDashSpeed;
     [Tooltip("공격 대시 지속 시간")]
     [SerializeField] private float attackdefaultTime;
     public float AttackDefaultTime => attackdefaultTime;
     [Tooltip("공격 범위 위치")]
     [SerializeField] private Transform attackPos;
     [Tooltip("공격 범위 크기")]
     [SerializeField] private Vector2 boxSize;
     [SerializeField] private Vector2 offset;
     [SerializeField] private LayerMask wallMask;
     [SerializeField] private float healTime;
     
     public enum DashType
     {
          Normal,
          Monster
     }
     public DashType curDashType { get; private set; }
     public Transform targetMonster { get; set; }
     public PlayerLay _playerLay{get; private set;}
     public int CurHealth { get; set; }
     private float attackTime;
     public float AttackTime
     {
          get => attackTime;
          set => attackTime = value;
     }
     public SpriteRenderer sr { get; private set; }
     public StateMachine stateMachine { get; private set; }
     public Animator animator { get; private set; }
     public SpriteRenderer spriteRenderer {get; private set;}
     public Rigidbody2D rigid { get; private set; }
     private Vector2 inputVec;
     public float inputX { get; private set; }
     public float dashTime { get;  set; }
     public float currentSpeed {get; set;}
     public float attackDashTime { get;  set; }
     public int AttackStep { get; set; }
     public bool isRight {get; set;}
     
     
     private bool isJumped;
     public bool isGrounded { get; private set; }
     public bool isDash {get; private set;}
     public bool isClimb{get; private set;}
     public bool isHang {get; private set;}
     public bool isAttack {get; private set;}
     public bool isSlash {get; private set;}
     public bool isLocked {get; private set;}
     public bool isWallJump {get; private set;}
   
     public readonly int Idle_Hash = Animator.StringToHash("Idle");
     public readonly int Run_Hash = Animator.StringToHash("Run");
     public readonly int Jump_Hash = Animator.StringToHash("Jump");
     public readonly int Dash_Hash = Animator.StringToHash("Dash");
     public readonly int Climb_Hash = Animator.StringToHash("Climb");
     public readonly int ClimbIdle_Hash = Animator.StringToHash("ClimbIdle");
     public readonly int CeilingClimb_Hash = Animator.StringToHash("CeilingClimb");
     public readonly int CeilingIdle_Hash = Animator.StringToHash("CeilingIdle");
     public readonly int Attack1_Hash = Animator.StringToHash("Attack1");
     public readonly int Attack2_Hash = Animator.StringToHash("Attack2");
     public readonly int Attack3_Hash = Animator.StringToHash("Attack3");
     public readonly int Slash_Hash = Animator.StringToHash("Slash");

     private void Awake()
     {
          StateMachineInit();
          CurHealth = MaxHealth;
     }
     private void Start()
     {
          animator = GetComponent<Animator>();
          spriteRenderer = GetComponent<SpriteRenderer>();
          rigid = GetComponent<Rigidbody2D>();
          sr = FxObj.GetComponent<SpriteRenderer>();
          _playerLay = GetComponent<PlayerLay>();
          currentSpeed = moveSpeed;
     }

     private void StateMachineInit()
     {
          stateMachine = new StateMachine();
          stateMachine.StateDic.Add(Estate.Idle, new Player_Idle(this));
          stateMachine.StateDic.Add(Estate.Run, new Player_Run(this));
          stateMachine.StateDic.Add(Estate.Jump, new Player_Jump(this));
          stateMachine.StateDic.Add(Estate.Dash, new Player_Dash(this));
          stateMachine.StateDic.Add(Estate.Climb, new Player_Climb(this));
          stateMachine.StateDic.Add(Estate.ClimbIdle, new Player_ClimbIdle(this));
          stateMachine.StateDic.Add(Estate.CeilingClimb, new Player_CeilingClimb(this));
          stateMachine.StateDic.Add(Estate.CeilingIdle, new Player_CeilingIdle(this));
          stateMachine.StateDic.Add(Estate.Attack1, new Player_Attack1(this));
          stateMachine.StateDic.Add(Estate.Attack2, new Player_Attack2(this));
          stateMachine.StateDic.Add(Estate.Attack3, new Player_Attack3(this));
          stateMachine.StateDic.Add(Estate.Slash, new Player_Slash(this));
          stateMachine.CurState = stateMachine.StateDic[Estate.Idle];
     }

     private void Update()
     {
          inputX = Input.GetAxisRaw("Horizontal");
          if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
          {
               Debug.Log("점프!");
               isJumped = true;
          }
          if (Input.GetKeyDown(KeyCode.LeftShift) && !isDash)
          {
               Debug.Log("대쉬!");
               SetDash(true, DashType.Normal);
          }

          if (Input.GetKeyDown(KeyCode.Mouse1) && !isAttack)
          {
               Debug.Log("공격1!");
               AttackCollider();
               isAttack = true;
          }

          HangableRay();
          Heal();
          
          stateMachine.Update();
     }

     private void FixedUpdate()
     {
          stateMachine.FixedUpdate();
     }
     
     private void Heal()
     {
          if (CurHealth < MaxHealth)
          {
               healTime += Time.deltaTime;
               if (healTime >= 10f)
               {
                    CurHealth++;
                    healTime = 0f;
                    Debug.Log("체력회복");
               }
          }
          else
          {
               healTime = 0f;
          }
     }
     

     public void OnDamage(int damage)
     {
          CurHealth -= damage;

          if (CurHealth <= 0)
          {
               Die();
          }
     }
     
     private void Die()
     {
          
     }

     public void AttackCollider()
     {
          Vector2 attackVec = attackPos.position;

          if (spriteRenderer.flipX)
          {
               attackVec.x = attackPos.position.x - (attackPos.localScale.x * 2);
          }

          Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackVec, boxSize, 0);
          foreach (Collider2D collider2D in collider2Ds)
          {
              // Debug.Log(collider2D.tag);
          }
     }

     private void HangableRay()
     {
          int layerMask = LayerMask.GetMask("HangableWall", "Car");
          RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 2.5f, layerMask);
          Debug.DrawLine(transform.position, transform.position + Vector3.up * 2.5f, Color.red);
          if (hit.collider != null)
          {
               Debug.Log("Ray Hit! " + hit.collider.name);
               SetHang(true);
               if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Car")
               {
                    transform.SetParent(hit.collider.transform, true);
               }
          }
          else
          {
               Debug.Log("Ray Miss!");
               SetHang(false);
               transform.SetParent(null);
          }
     }
     
     public void WallRay()
     {
          int layerMask = LayerMask.GetMask("Ground");
          Vector2 rayDir = (Vector2.down + (isRight ? Vector2.left : Vector2.right)).normalized;
          RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, 2.5f, layerMask);
          Debug.DrawLine(transform.position, transform.position + (Vector3)rayDir * 2.5f, Color.red);
          if (hit.collider != null)
          {
               Debug.Log("Ray Hit! " + hit.collider.name);
               SetGrounded(true);
          }
          else
          {
               Debug.Log("Ray Miss!");
               SetGrounded(false);
          }
     }

     public bool GetIsJumped()
     {
          if (isJumped)
          {
               isJumped = false;
               return true;
          }
          else
          {
               return false;
          }
     }

     public void SetGrounded(bool grounded)
     {
          isGrounded = grounded;
     }

     public void SetDash(bool dash, DashType dashType)
     {
          isDash = dash;
          curDashType = dashType;
     }

     public void SetClimb(bool climb)
     {
          isClimb = climb;
     }

     public void SetHang(bool hang)
     {
          isHang = hang;
     }

     public void SetSlash(bool slash)
     {
          isSlash = slash;
     }

     public void SetLock(bool locked)
     {
          isLocked = locked;
     }

     public void SetWallJump(bool wallJump)
     {
          isWallJump = wallJump;
     }

     public void SlashEnd()
     {
          stateMachine.ChangeState(stateMachine.StateDic[Estate.Idle]);
          isSlash = false;
          Debug.Log("SlashEnd");
     }

     private void OnDrawGizmos()
     {
          if (attackPos == null || spriteRenderer == null) return;

          Vector2 attackVec = attackPos.position;

          if (spriteRenderer.flipX)
          {
               attackVec.x = attackPos.position.x - (attackPos.localScale.x * 2);
          }
          Gizmos.color = Color.blue;
          Gizmos.DrawWireCube(attackVec, boxSize);
     }

     private void OnCollisionEnter2D(Collision2D collision)
     {
          if (collision.gameObject.CompareTag("Ground"))
          {
               isGrounded = true;
               Debug.Log("충돌");
          }

          if (collision.gameObject.CompareTag("Wall"))
          {
               isClimb = true;
               Debug.Log("벽이에요.");
          }

          if (collision.gameObject.CompareTag("HangableWall"))
          {
               isHang = true;
          }

          if (collision.gameObject.CompareTag("Monster") && isSlash)
          {
               isSlash = true;
               Debug.Log("슬래시공격!!!!!!!!");
          }
     }

     private void OnCollisionExit2D(Collision2D collision)
     {
          if (collision.gameObject.CompareTag("Ground"))
          {
               isGrounded = false; 
          }

          if (collision.gameObject.CompareTag("Wall"))
          {
               isClimb = false;
               Debug.Log("벽에서 벗어남");
          }

          if (collision.gameObject.CompareTag("HangableWall"))
          {
               isHang = false;
               Debug.Log("벗어남");
          }

          if (collision.gameObject.CompareTag("Monster") && isSlash)
          {
               isSlash = false;
          }
     }
     
     
}
