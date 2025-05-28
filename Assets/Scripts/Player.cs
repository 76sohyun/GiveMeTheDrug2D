using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
     [Header("Movement Settings")]
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
     
     public StateMachine stateMachine { get; private set; }
     public Animator animator { get; private set; }
     public SpriteRenderer spriteRenderer {get; private set;}
     public Rigidbody2D rigid { get; private set; }
     private Vector2 inputVec;
     public float inputX { get; private set; }
     public float dashTime { get;  set; }
     public float currentSpeed {get; set;}
     private bool isJumped;
     public bool isGrounded { get; private set; }
     public bool isDash {get; private set;}
   
     public readonly int Idle_Hash = Animator.StringToHash("Idle");
     public readonly int Run_Hash = Animator.StringToHash("Run");
     public readonly int Jump_Hash = Animator.StringToHash("Jump");
     public readonly int Dash_Hash = Animator.StringToHash("Dash");

     private void Awake()
     {
          StateMachineInit();
     }
     private void Start()
     {
          animator = GetComponent<Animator>();
          spriteRenderer = GetComponent<SpriteRenderer>();
          rigid = GetComponent<Rigidbody2D>();
          currentSpeed = moveSpeed;
     }

     private void StateMachineInit()
     {
          stateMachine = new StateMachine();
          stateMachine.StateDic.Add(Estate.Idle, new Player_Idle(this));
          stateMachine.StateDic.Add(Estate.Run, new Player_Run(this));
          stateMachine.StateDic.Add(Estate.Jump, new Player_Jump(this));
          stateMachine.StateDic.Add(Estate.Dash, new Player_Dash(this));
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
          stateMachine.Update();
     }

     private void FixedUpdate()
     {
          stateMachine.FixedUpdate();
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

     public void SetDash(bool dash)
     {
          isDash = dash;
     }

     private void OnCollisionEnter2D(Collision2D collision)
     {
          if (collision.gameObject.CompareTag("Ground"))
          {
               isGrounded = true;
               Debug.Log("충돌");
          }
     }

     private void OnCollisionExit2D(Collision2D collision)
     {
          if (collision.gameObject.CompareTag("Ground"))
          {
               isGrounded = false; 
               Debug.Log("벗어남");
          }
     }
}
