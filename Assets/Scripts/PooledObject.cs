using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool returnPool;
    [SerializeField] private float returnTime;
    private float timer;
    [SerializeField] private Rigidbody2D rigid;

    private void OnEnable()
    {
        timer = returnTime;
        Debug.Log($"{gameObject.name} OnEnable 호출, 타이머 초기화");
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ReturnPool();  
        }
        if (rigid.velocity.magnitude > 2) //총알 방향 설정
        {
            transform.forward = rigid.velocity;
        }
    }
    
    public void ReturnPool()
    {
        if (returnPool ==null)
        {
            Debug.LogWarning($"{gameObject.name} 은 returnPool이 null이라 Destroy됩니다");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{gameObject.name} 이(가) ObjectPool로 반환됨");
            returnPool.ReturnPool(this);
        }
    }
}
