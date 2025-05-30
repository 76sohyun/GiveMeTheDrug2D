using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLay : MonoBehaviour
{
    [SerializeField] private float MaxDistance;
    private Vector3 MousePosition;
    [SerializeField]private LayerMask _layerMask;
    private Player _player;
    public bool isdash{get; set;}

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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        Vector2 direction = (mousePos - transform.position).normalized;
        Debug.DrawRay(transform.position, direction * MaxDistance, Color.green);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, MaxDistance, _layerMask);
        
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
