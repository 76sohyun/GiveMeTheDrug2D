using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    [SerializeField] private GameObject pivot;
    [SerializeField] private LayerMask droneLayer;
    public LayerMask DroneLayer => droneLayer;
    [SerializeField] private float detectRange;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private DroneShoot droneShoot;

    private StateMachine stateMachine;
    public Rigidbody2D rigid{get; private set;}
    public Animator animator{get; private set;}
    public Vector2 patrolVec{get; private set;}
    private SpriteRenderer spriteRenderer;
    public bool isWaited{get; private set;}
    public bool isStoped{get; private set;}
    
    public readonly int DronIdle_Hash = Animator.StringToHash("DroneIdle");
    
    private void Awake()
    {
        StateMachineInit();
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        droneShoot = GetComponent<DroneShoot>();
        patrolVec = Vector2.left;
    }

    private void StateMachineInit()
    {
        stateMachine = new StateMachine();
        stateMachine.StateDic.Add(Estate.DroneIdle, new Drone_Idle(this));
        stateMachine.CurState = stateMachine.StateDic[Estate.DroneIdle];
    }

    private void Update()
    {
        stateMachine.Update();
        DetectTarget();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void Back()
    {
        StartCoroutine(CoTurnBack());
    }

    private IEnumerator CoTurnBack()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        if (spriteRenderer.flipX)
        {
            patrolVec = Vector2.right;
        }
        else
        {
            patrolVec = Vector2.left;
        }
        isWaited = true;
        rigid.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        isWaited = false;
    }

    private void DetectTarget()
    {
        Collider2D target = Physics2D.OverlapCircle(transform.position, detectRange, targetLayer);

        if (target != null)
        {
            Vector2 targetPos = target.transform.position;
            droneShoot.Startshoot(targetPos);
        }
        else
        {
            droneShoot.Stopshoot();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("부딫힘");
            isStoped = true;
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isStoped = false;
        }
    }
}
