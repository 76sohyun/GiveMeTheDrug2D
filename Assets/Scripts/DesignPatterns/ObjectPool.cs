using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private List<PooledObject> pool;
    [SerializeField] PooledObject prefab;
    [SerializeField] int size;

    private void Awake()
    {
        pool = new List<PooledObject>();
        for (int i = 0; i < size; i++)
        {
            PooledObject instance = Instantiate(prefab);
            instance.gameObject.SetActive(false);
            pool.Add(instance);
        }
    }

    public PooledObject GetPool(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0)
        {
            return Instantiate(prefab, position, rotation);
        }
        
        PooledObject instance = pool[pool.Count - 1];
        pool.RemoveAt(pool.Count - 1);

        instance.returnPool = this;
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.gameObject.SetActive(true);
        
        return instance;
    }

    public void ReturnPool(PooledObject instance) //다썼으면반납
    {
        Debug.Log($"ObjectPool에 {instance.gameObject.name} 반환");
        instance.gameObject.SetActive(false);
        pool.Add(instance);
    }

}
