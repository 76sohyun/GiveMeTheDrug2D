using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundManager1 : MonoBehaviour
{
    [Header("Drag&Drop")] 
    [SerializeField] private Transform[] _backGrounds;
    [SerializeField] private Transform _playerPos;

    private float _spriteWidth;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Relocate();
    }

    private void Init()
    {
        _spriteWidth = _backGrounds[0].GetComponent<SpriteRenderer>().bounds.size.x - 0.2f;
        Debug.Log(_spriteWidth);
    }

    private void Relocate()
    {
        for (int i = 0; i < _backGrounds.Length; i++)
        {
            float distance = (_playerPos.position.x - _backGrounds[i].position.x) / 2f; 
            if (Mathf.Abs(distance) > _spriteWidth)
            {
                float offset = _spriteWidth * _backGrounds.Length;
                Vector3 pos = _backGrounds[i].position;
                _backGrounds[i].position = new Vector3(pos.x + Mathf.Sign(distance) * offset, pos.y, pos.z);
            }
        }
    }
}
