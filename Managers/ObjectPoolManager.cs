using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    public int defaultCapacity = 100;
    public int maxPoolSize = 200;
    [SerializeField] private GameObject bulletPrefab;

    public IObjectPool<GameObject> bulletPool { get; private set; }

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        Init();
    }
    private void Init()
    {
        bulletPool = new ObjectPool<GameObject>(CreateBullet, GetItem, ReleaseItem, DeleteItem, true, defaultCapacity, maxPoolSize);

        for (int i = 0; i < defaultCapacity; i++)
        {
            BulletController bullet = CreateBullet().GetComponent<BulletController>();
            bullet.Pool.Release(bullet.gameObject);
        }
    }
    private GameObject CreateBullet()
    {
        GameObject go = Instantiate(bulletPrefab, gameObject.transform);
        go.GetComponent<BulletController>().Pool = bulletPool;
        return go;
    }
    private void GetItem(GameObject _poolItem)
    {
        _poolItem.SetActive(true);
    }
    private void ReleaseItem(GameObject _poolItem)
    {
        _poolItem.SetActive(false);
    }
    private void DeleteItem(GameObject _poolItem)
    {
        Destroy(_poolItem);
    }
}