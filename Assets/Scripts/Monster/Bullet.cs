using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (rigid.velocity.magnitude > 2f)
        {
            transform.right = rigid.velocity.normalized;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<PooledObject>().ReturnPool();
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
