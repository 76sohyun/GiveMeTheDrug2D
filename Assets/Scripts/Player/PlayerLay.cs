using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLay : MonoBehaviour
{
    [SerializeField] private float MaxDistance;
    [SerializeField]private LayerMask _layerMask;
    [SerializeField] private Texture2D MonsterCursor;
    private Vector3 MousePosition;
    private Player _player;
    public bool isdash{get; set;}
    private bool isMonster = false;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        isdash = false;
    }
    
    private void Update()
    {
        _player.AttackCollider();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        
        Debug.DrawRay(transform.position, direction * MaxDistance, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, MaxDistance, _layerMask);

        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            if (!isMonster)
            {
                isMonster = true;
                Cursor.SetCursor(MonsterCursor, Vector2.zero, CursorMode.Auto);
            }
        }
        else
        {
            if (isMonster)
            {
                isMonster = false;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    isdash = true;
                    _player.targetMonster = hit.collider.transform;
                    _player.SetDash(true,Player.DashType.Monster);
                }
            }
        }
    }

}
