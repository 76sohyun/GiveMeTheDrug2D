using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneShoot : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] ObjectPool bulletPool;
    [Range(10,30)]
    [SerializeField] float bulletSpeed;
    [SerializeField] int bulletCount;
    [SerializeField] float timeBetweenBullets;
    
    private Coroutine bulletCoroutine;
    private Vector3 bulletPosition;
    private bool isShoot = false;

    public void Startshoot(Vector2 targetPos)
    {
        bulletPosition = targetPos;
        if (!isShoot)
        {
            bulletCoroutine = StartCoroutine(Shooting(targetPos));
            isShoot = true;
        }
    }

    public void Stopshoot()
    {
        if (isShoot && bulletCoroutine != null)
        {
            StopCoroutine(bulletCoroutine);
            bulletCoroutine = null;
            isShoot = false;
        }
    }

    private void Updatepos(Vector2 targetPos)
    {
        bulletPosition = targetPos;
    }

    private IEnumerator Shooting(Vector2 targetPos)
    {
        while (true)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                Fire(bulletPosition);
                yield return new WaitForSeconds(timeBetweenBullets);
            }
            yield return new WaitForSeconds(3f);
        }
    }
    
    public void Fire(Vector2 targetPos)
    {
        PooledObject instance = bulletPool.GetPool((Vector2)muzzlePoint.position + Vector2.up * 0.5f ,Quaternion.LookRotation(targetPos));
        Rigidbody2D bulletRigidBody = instance.GetComponent<Rigidbody2D>();
        
        Vector2 dir = (targetPos - (Vector2)muzzlePoint.position).normalized;
        bulletRigidBody.velocity = dir * bulletSpeed;
    }
}
