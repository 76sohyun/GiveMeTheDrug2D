using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    [SerializeField] private int damage;
    [SerializeField] private int MaxHealth;
    [SerializeField] private LayerMask ZombieLayer;
    public LayerMask zombieLayer => ZombieLayer;
    [Tooltip("공격 범위 위치")]
    [SerializeField] private Transform attackPos;
    [Tooltip("공격 범위 크기")]
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private Vector2 offset;
    [SerializeField] private int AttackDamage;
    
    public Animator animator{get; private set;}
    private SpriteRenderer spriteRenderer;
    public Rigidbody2D rigidbody2D{get; private set;}
    private int CurHealth;
    public Vector2 patrolVec{get; private set;}
    public bool isWaited{get; set;}
    public bool isStopped{get; set;}
    private Transform target;
    private bool isAttacking = false;
    private LayerMask targetLayer;
    
    public readonly int ZomIdle_Hash = Animator.StringToHash("Zombie1");
    public readonly int ZomAttack_Hash = Animator.StringToHash("ZomAttack");
    public readonly int ZomWalk_Hash = Animator.StringToHash("ZomWalk");
    
    private void Awake()
    {
        CurHealth = MaxHealth;
    }
    
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolVec = Vector2.left;
    }

    private void Update()
    {
        Patrol();
        AttackCollider();
        TraceTarget();
    }
    
    private void FixedUpdate()
    {
        if(isWaited == false)
            rigidbody2D.velocity = patrolVec * moveSpeed;
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
            patrolVec = Vector2.left;
        }
        else
        {
            patrolVec = Vector2.right;
        }
        animator.Play(ZomIdle_Hash);
        isWaited = true;
        rigidbody2D.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        isWaited = false;
        animator.Play(ZomWalk_Hash);
    }
    
    private void Patrol()
    {
        Vector2 rayOrigin = transform.position + new Vector3(patrolVec.x, 0);
        Debug.DrawRay(rayOrigin, Vector2.down * 3f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 3f, ZombieLayer);
        if (hit.collider == null)
        {
            //돌아가는 로직
            StartCoroutine(CoTurnBack());
        }
    }

    private void AttackCollider()
    {
        if (isAttacking) return;
        Vector2 attackVec = attackPos.position;

        if (spriteRenderer.flipX)
        {
            attackVec.x = attackPos.position.x - (attackPos.localScale.x * 2);
        }

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackVec, boxSize, 0);
        bool hitPlayer = false;
        foreach (Collider2D collider2D in collider2Ds)
        {
            if (collider2D.CompareTag("Player"))
            {
                Player player = collider2D.GetComponent<Player>();
                if (player != null)
                {
                    isAttacking = true;
                    animator.Play(ZomAttack_Hash);
                    player.OnDamage(AttackDamage);
                    Debug.Log("플레이어 타격");
                    StartCoroutine(AttackCool());
                    hitPlayer = true;
                    break;
                }
            }
        }

        if (!hitPlayer)
        {
            animator.Play(ZomWalk_Hash);
        }
    }

    private void TraceTarget()
    {
        Vector2 lookDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDir, 3f, targetLayer);
        Debug.DrawLine(transform.position, transform.position + (Vector3)(lookDir * 3f), Color.green);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log(hit.collider.name);
            target = hit.collider.transform;
        }
        else
        {
            target = null;
        }

        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            transform.position += (Vector3)(dir * 3f * Time.deltaTime);
            animator.Play(ZomWalk_Hash);
        }
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

    public void OnDamage(int damage)
    {
        CurHealth -= damage;
        Debug.Log($"몬스터의 {CurHealth}체력");

        if (CurHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator AttackCool()
    {
        yield return new WaitForSeconds(1.0f); 
        isAttacking = false;
    }
}
