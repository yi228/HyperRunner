using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunHole;
    [SerializeField] private LineRenderer line;
    private PlayerController player;
    private PlayerCamera playerCam;
    private AudioSource audioSource;
    [Header("Layer")]
    [SerializeField] private LayerMask canGrap;
    [Header("Grappling")]
    [SerializeField] private float maxGrapDist;
    [SerializeField] private float grapDelayTime;
    [SerializeField] private float overY;
    private Vector3 grapPoint;
    [Header("Cooltime")]
    [SerializeField] private float grapCooltime;
    private float grapCoolTimer;
    //Flag
    private bool isGrappling;
    
    void Start()
    {
        Init();
    }
    void Update()
    {
        if(grapCoolTimer > 0)
            grapCoolTimer -= Time.deltaTime;
    }
    void LateUpdate()
    {
        if (isGrappling)
            line.SetPosition(0, gunHole.position);
    }
    private void Init()
    {
        player = GetComponent<PlayerController>();
        playerCam = Camera.main.GetComponent<PlayerCamera>();
        audioSource = GetComponent<AudioSource>();
    }
    public void StartGrap()
    {
        if (grapCoolTimer > 0)
            return;
        isGrappling = true;
        player.freeze = true;
        SoundManager.instance.PlayClip(audioSource, "GRAP");
        RaycastHit _hit;
        if(Physics.Raycast(cam.position, cam.forward, out _hit, maxGrapDist, canGrap))
        {
            grapPoint = _hit.point;
            Invoke("DoGrap", grapDelayTime);
        }
        else
        {
            grapPoint = cam.position + cam.forward * maxGrapDist;
            Invoke("StopGrap", grapDelayTime);
        }

        line.enabled = true;
        line.SetPosition(1, grapPoint);
    }
    private void DoGrap()
    {
        player.freeze = false;

        Vector3 _lowestPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        float _grapPosGapY = grapPoint.y - _lowestPos.y;
        float _highestPosY = _grapPosGapY + overY;

        if (_grapPosGapY < 0) 
            _highestPosY = overY;

        player.JumpToPos(grapPoint, _highestPosY);
        playerCam.DoFov(100);
        Invoke("StopGrap", 1f);
    }
    public void StopGrap()
    {
        player.freeze = false;
        isGrappling = false;
        grapCoolTimer = grapCooltime;
        playerCam.DoFov(80);
        line.enabled = false;
    }
}
