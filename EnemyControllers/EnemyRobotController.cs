using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemyRobotController : CreatureController
{
    [Header("References")]
    [SerializeField] private Transform gunHole;
    [SerializeField] private GameObject bulletPrefab;
    public IObjectPool<GameObject> Pool;
    private PlayerController player;
    private Animator anim;
    private new AudioSource audio;
    [Header("Values")]
    [SerializeField] private float turnSpeed;
    private bool lateInitComplete = false;
    private float dist;

    void Start()
    {
        Init();
    }
    void Update()
    {
        if (!lateInitComplete)
        {
            lateInitComplete = true;
            Pool = ObjectPoolManager.instance.bulletPool;
        }
        if (hp > 0)
            LookPlayer();
        dist = Vector3.Distance(transform.position, player.transform.position);
        anim.SetFloat("Dist", dist);
    }
    private void Init()
    {
        player = FindAnyObjectByType<PlayerController>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }
    private void LookPlayer()
    {
        Vector3 _playerDir = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - gunHole.position.y, player.transform.position.z - transform.position.z);
        var _targetRot = Quaternion.LookRotation(_playerDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, turnSpeed * Time.deltaTime);
    }
    public void SpawnBullet()
    {
        if (Pool != null)
        {
            GameObject _go = Pool.Get();
            _go.GetComponent<BulletController>().gunHole = gunHole;
            _go.GetComponent<BulletController>().damage = attack;
            _go.transform.position = gunHole.position;
            _go.transform.rotation = gunHole.rotation;
            _go.GetComponent<BulletController>().dir = gunHole.up.normalized;
            SoundManager.instance.PlayClip(audio, "GUN", false, 20);
        }
    }
}
