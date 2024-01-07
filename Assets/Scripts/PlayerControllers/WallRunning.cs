using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Force & Speed")]
    [SerializeField] private float wallRunForce;
    [SerializeField] private float wallJumpUpForce;
    [SerializeField] private float wallJumpSideForce;
    [SerializeField] private float climbSpeed;
    [Header("Layer")]
    [SerializeField] private LayerMask isWall;
    [SerializeField] private LayerMask isGround;
    [Header("Exit")]
    [SerializeField] private float wallExitTime;
    private float wallExitTimer;
    [Header("Gravity")]
    [SerializeField] private float counterGravity;
    [Header("Check")]
    [SerializeField] private float wallCheckDist;
    [SerializeField] private float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    //MoveValues
    private float inputHor;
    private float inputVer;
    //References
    private Transform orientation;
    private PlayerController player;
    private Rigidbody rigid;
    private PlayerCamera playerCam;
    private KeyBindings key;
    private AudioSource audioSource;
    //Flags
    private bool isToUp;
    private bool isToDown;
    private bool wallExit;
    private bool useGravity;
    private bool onLeftWall;
    private bool onRightWall;

    void Start()
    {
        Init();
    }
    void Update()
    {
        CheckForWall();
        ManageState();
    }
    void FixedUpdate()
    {
        if (player.wallRunning)
            WallRunningMovement();
    }
    private void Init()
    {
        rigid = GetComponent<Rigidbody>();
        player = GetComponent<PlayerController>();
        playerCam = Camera.main.GetComponent<PlayerCamera>();
        audioSource = GetComponent<AudioSource>();
        key = GetComponent<KeyBindings>();
        orientation = player.orientation;
        useGravity = true;
    }
    private void CheckForWall()
    {
        onRightWall = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDist, isWall);
        onLeftWall = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDist, isWall);
    }
    private bool CheckEnoughHeight()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, isWall);
    }
    private void ManageState()
    {
        inputHor = Input.GetAxisRaw("Horizontal");
        inputVer = Input.GetAxisRaw("Vertical");

        isToUp = Input.GetKey(key.upRunning);
        isToDown = Input.GetKey(key.downRunning);
        
        if ((onLeftWall || onRightWall) && inputVer > 0 && CheckEnoughHeight() && !wallExit)
        {
            if (!player.wallRunning)
                StartWallRun();
            if (Input.GetKeyDown(key.wallJump))
                WallJump();
        }
        else if (wallExit)
        {
            if (player.wallRunning)
                StopWallRun();
            if (wallExitTimer > 0)
                wallExitTimer -= Time.deltaTime;
            if (wallExitTimer <= 0)
                wallExit = false;
        }
        else
        {
            if (player.wallRunning)
                StopWallRun();
        }
    }
    private void StartWallRun()
    {
        player.wallRunning = true;
        rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
        SoundManager.instance.PlayClip(audioSource, "WALL", true);
        playerCam.DoFov(90);
        if (onLeftWall)
            playerCam.DoTilt(-5f);
        if(onRightWall)
            playerCam.DoTilt(5f);
    }
    private void WallRunningMovement()
    {
        rigid.useGravity = useGravity;

        Vector3 _wallNormal = onRightWall ? rightWallHit.normal : leftWallHit.normal;
        Vector3 _wallForward = Vector3.Cross(_wallNormal, transform.up);

        if ((orientation.forward - _wallForward).magnitude > (orientation.forward - -_wallForward).magnitude)
            _wallForward = -_wallForward;

        rigid.AddForce(_wallForward * wallRunForce, ForceMode.Force);

        if (isToUp)
            rigid.velocity = new Vector3(rigid.velocity.x, climbSpeed, rigid.velocity.z);
        if (isToDown)
            rigid.velocity = new Vector3(rigid.velocity.x, -climbSpeed, rigid.velocity.z);

        if (!(onLeftWall && inputHor > 0) && !(onRightWall && inputHor < 0))
            rigid.AddForce(-_wallNormal * 100, ForceMode.Force);

        if(useGravity)
            rigid.AddForce(transform.up * counterGravity, ForceMode.Force);
    }
    private void StopWallRun()
    {
        player.wallRunning = false;
        playerCam.DoFov(80);
        playerCam.DoTilt(0);
        audioSource.Stop();
    }
    private void WallJump()
    {
        wallExit = true;
        wallExitTimer = wallExitTime;
        Vector3 _wallNormal = onRightWall ? rightWallHit.normal : leftWallHit.normal;
        Vector3 _applyForce = transform.up * wallJumpUpForce + _wallNormal * wallJumpSideForce;

        rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
        rigid.AddForce(_applyForce, ForceMode.Impulse);
    }
}
