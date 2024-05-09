using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : CreatureController
{
    [Header("Speed")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float swingSpeed;
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float groundDrag;
    private float moveSpeed;
    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCool;
    [SerializeField] private float airMultiplier;
    private bool canJump = true;
    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchY;
    private float originY;
    [Header("GroundCheck")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask isGround;
    private bool grounded;
    [Header("ForSlope")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeRay;
    //MoveValues
    private float inputHor;
    private float inputVer;
    private Vector3 moveDir;
    private Vector3 veloToSet;
    [Header("References")]
    [SerializeField] private Slider hpBar;
    public Transform orientation;
    private Rigidbody rigid;
    private AudioSource audioSource;
    [SerializeField] private AudioSource secondAudio;
    private KeyBindings key;
    private Swinging SW;
    private Grappling GR;
    [SerializeField] private KatanaSwing KS;
    [Header("Flags")]
    public bool wallRunning = false;
    public bool freeze = false;
    public bool activeGrapple = false;
    public bool setMoveEnable;
    public bool swinging = false;

    private MoveState mState;

    public enum MoveState
    {
        freeze,
        walking,
        running,
        swing,
        wall,
        grappling,
        crouching,
        air
    }

    void Start()
    {
        Init();
    }
    void Update()
    {
        CheckOnGround();
        ManageInput();
        SpeedControl();
        ManageState();
        hpBar.value = (float)hp / (float)maxHp;
    }
    void FixedUpdate()
    {
        Move();
    }
    private void Init()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.freezeRotation = true;
        originY = transform.localScale.y;
        audioSource = GetComponent<AudioSource>();
        key = GetComponent<KeyBindings>();
        SW = GetComponent<Swinging>();
        GR = GetComponent<Grappling>();
        KS.damage = attack;
    }
    private void ManageInput()
    {
        inputHor = Input.GetAxisRaw("Horizontal");
        inputVer = Input.GetAxisRaw("Vertical");

        if (grounded && (inputHor != 0 || inputVer != 0))
        {
            string _clipKey = "WALK";
            if (Input.GetKeyDown(key.run) || Input.GetKeyUp(key.run))
                audioSource.Stop();
            if (Input.GetKey(key.run))
                _clipKey = "RUN";
            else
                _clipKey = "WALK";
            if (!audioSource.isPlaying)
                SoundManager.instance.PlayClip(audioSource, _clipKey);
        }
        else if (audioSource.clip != null && (audioSource.clip.name == "Walk" || audioSource.clip.name == "Run"))
            audioSource.Stop();

        if (Input.GetKey(key.jump) && canJump && grounded)
        {
            canJump = false;
            Jump();
            Invoke("ResetJump", jumpCool);
        }

        if (Input.GetKeyDown(key.crouch))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchY, transform.localScale.z);
            rigid.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(key.crouch))
            transform.localScale = new Vector3(transform.localScale.x, originY, transform.localScale.z);

        if (SW.enabled)
        {
            if (Input.GetKeyDown(key.swing))
                SW.StartSwing();
            if (Input.GetKey(key.swing))
                SW.DrawRope();
            if (Input.GetKeyUp(key.swing))
                SW.StopSwing();
        }

        if (GR.enabled && Input.GetKeyDown(key.grapple))
            GR.StartGrap();
    }
    private void CheckOnGround()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isGround);
        if (grounded && !activeGrapple)
            rigid.drag = groundDrag;
        else
            rigid.drag = 0;
    }
    private void ManageState()
    {
        if (wallRunning)
        {
            mState = MoveState.wall;
            moveSpeed = wallRunSpeed;
        }
        if (freeze)
        {
            mState = MoveState.freeze;
            moveSpeed = 0;
            rigid.velocity = Vector3.zero;
        }
        else if (activeGrapple)
        {
            mState = MoveState.grappling;
            moveSpeed = runSpeed;
        }
        else if (swinging)
        {
            mState = MoveState.swing;
            moveSpeed = swingSpeed;
        }
        else if (Input.GetKey(key.crouch))
        {
            mState = MoveState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(key.run))
        {
            mState = MoveState.running;
            moveSpeed = runSpeed;
        }
        else if (grounded)
        {
            if (mState == MoveState.air)
                SoundManager.instance.PlayClip(audioSource, "JUMPEN");
            mState = MoveState.walking;
            moveSpeed = walkSpeed;
        }
        else
            mState = MoveState.air;
    }
    private void Move()
    {
        if (activeGrapple || swinging)
            return;

        moveDir = orientation.forward * inputVer + orientation.right * inputHor;

        if (OnSlope())
        {
            rigid.AddForce(GetSlopeMoveDir() * moveSpeed * 10f, ForceMode.Force);

            if (rigid.velocity.y > 0)
                rigid.AddForce(Vector3.down * 10f, ForceMode.Force);
        }
        else if (grounded)
            rigid.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rigid.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        if (!wallRunning)
            rigid.useGravity = !OnSlope();
    }
    private void SpeedControl()
    {
        if (activeGrapple)
            return;

        if (OnSlope())
        {
            if (rigid.velocity.magnitude > moveSpeed)
                rigid.velocity = rigid.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 _flatVelocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
            if (_flatVelocity.magnitude > moveSpeed)
            {
                Vector3 _limitedVelocity = _flatVelocity.normalized * moveSpeed;
                rigid.velocity = new Vector3(_limitedVelocity.x, rigid.velocity.y, _limitedVelocity.z);
            }
        }
    }
    private void Jump()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
        rigid.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        SoundManager.instance.PlayClip(audioSource, "JUMPST");
    }
    private void ResetJump()
    {
        canJump = true;
    }
    public void JumpToPos(Vector3 _targetPos, float _height)
    {
        activeGrapple = true;

        veloToSet = CalcJumpVelocity(transform.position, _targetPos, _height);
        Invoke("SetVelocity", 0.1f);
        Invoke("ResetLimit", 3f);
    }
    private void SetVelocity()
    {
        setMoveEnable = true;
        rigid.velocity = veloToSet;
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeRay, playerHeight * 0.5f + 0.3f))
        {
            float _angle = Vector3.Angle(Vector3.up, slopeRay.normal);
            return _angle < maxSlopeAngle && _angle != 0;
        }
        return false;
    }
    private Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeRay.normal).normalized;
    }
    public Vector3 CalcJumpVelocity(Vector3 _startPos, Vector3 _endPos, float _height)
    {
        float _gravity = Physics.gravity.y;
        float _distY = _endPos.y - _startPos.y;
        Vector3 _distXZ = new Vector3(_endPos.x - _startPos.x, 0, _endPos.z - _startPos.z);

        Vector3 _velocityY = Vector3.up * Mathf.Sqrt(-2 * _gravity * _height);
        Vector3 _velocityXZ = _distXZ / (Mathf.Sqrt(-2 * _height / _gravity) + Mathf.Sqrt(2 * (_distY - _height) / _gravity));

        return _velocityY + _velocityXZ;
    }
    public void ResetLimit()
    {
        activeGrapple = false;
    }
    public void PlayDamageSound()
    {
        SoundManager.instance.PlayClip(secondAudio, "DAMAGE");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (setMoveEnable)
        {
            setMoveEnable = false;
            ResetLimit();
            GetComponent<Grappling>().StopGrap();
        }
    }
}
