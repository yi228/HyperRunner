using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletController : MonoBehaviour
{
    //References
    private Rigidbody rigid;
    public IObjectPool<GameObject> Pool;
    public Transform gunHole;
    //values
    [SerializeField] private float speed;
    public Vector3 dir;
    public int damage;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(dir != null)
            rigid.MovePosition(rigid.position + dir * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, rigid.position) >= 100)
            Pool.Release(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.GetComponent<PlayerController>().OnDamaged(damage);
        Pool.Release(gameObject);
    }
}
