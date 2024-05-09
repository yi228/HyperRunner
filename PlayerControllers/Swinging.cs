using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float horizontalBoost;
    [SerializeField] private float forwardBoost;
    [SerializeField] private float cableExtendSpeed;
    [Header("References")]
    [SerializeField] private Transform gunHole;
    [SerializeField] private Transform playerCam;
    [SerializeField] private LayerMask canGrap;
    private PlayerController player;
    private LineRenderer line;
    private SpringJoint joint;
    private Transform orientation;
    private Rigidbody rigid;
    private AudioSource audioSource;
    //SwingValues
    private Vector3 curGrapPos;
    private float maxSwingDist = 25f;
    private Vector3 swingPoint;

    void Start()
    {
        Init();
    }
    void Update()
    {
        if (joint != null)
            OnAirMove();
    }
    private void Init()
    {
        player = GetComponent<PlayerController>();
        rigid = GetComponent<Rigidbody>();
        line = GetComponent<LineRenderer>();
        orientation = player.orientation;
        audioSource = GetComponent<AudioSource>();
    }
    public void StartSwing()
    {
        if(player != null)
            player.swinging = true;
        SoundManager.instance.PlayClip(audioSource, "GRAP");
        RaycastHit _hit;
        if(Physics.Raycast(playerCam.position, playerCam.forward, out _hit, maxSwingDist, canGrap))
        {
            swingPoint = _hit.point;
            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float _pointDist = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = _pointDist * 0.8f;
            joint.minDistance = _pointDist * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            line.positionCount = 2;
            curGrapPos = gunHole.position;
        }
    }
    public void StopSwing()
    {
        player.swinging = false;

        line.positionCount = 0;
        Destroy(joint);
    }
    public void DrawRope()
    {
        if (!joint)
            return;
        line.enabled = true;

        curGrapPos = Vector3.Lerp(curGrapPos, swingPoint, Time.deltaTime * 8f);

        line.SetPosition(0, gunHole.position);
        line.SetPosition(1, swingPoint);
    }
    private void OnAirMove()
    {
        if(Input.GetKey(KeyCode.D))
             rigid.AddForce(orientation.right * horizontalBoost * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            rigid.AddForce(-orientation.right * horizontalBoost * Time.deltaTime);
        if (Input.GetKey(KeyCode.W))
            rigid.AddForce(orientation.forward * forwardBoost * Time.deltaTime);
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 _pointDir = swingPoint - transform.position;
            rigid.AddForce(_pointDir.normalized * forwardBoost * Time.deltaTime);

            float _pointDist = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = _pointDist * 0.8f;
            joint.minDistance = _pointDist * 0.25f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            float _extendedPointdist = Vector3.Distance(transform.position, swingPoint) + cableExtendSpeed;

            joint.maxDistance = _extendedPointdist * 0.8f;
            joint.minDistance = _extendedPointdist * 0.25f;
        }
    }
}
