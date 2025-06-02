using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform startpos;
    [SerializeField] private Transform endpos;
    [SerializeField] private float speed;
    [SerializeField] private Transform desPos;
    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        transform.position = startpos.position;
        desPos = endpos;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, desPos.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, desPos.position) <= 0.5f)
        {
            if (desPos == endpos)
            {
                desPos = startpos;
                _spriteRenderer.flipX = true;
            }
            else
            {
                desPos = endpos;
                _spriteRenderer.flipX = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
    
}
