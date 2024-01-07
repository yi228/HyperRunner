using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaSwing : MonoBehaviour
{
    //References
    [SerializeField] private Transform playerCam;
    [SerializeField] private GameObject melee;
    [SerializeField] private KeyBindings keys;
    private new AudioSource audio;
    private Animator anim;
    //Flags
    private bool hitboxActivated = false;
    private bool canAttack = true;
    private bool checkCancel = false;
    private bool audioPlayed = false;
    //Timer
    private float cancelTime = 1f;
    private float cTimer = 0f;
    //Value
    [SerializeField] private float attackDist;
    private int attackStep = 1;
    public int damage;

    void Start()
    {
        Init();
    }
    void Update()
    {
        TryAttack();

        if(hitboxActivated)
        {
            melee.SetActive(true);
            melee.GetComponent<MeleeZone>().damage = damage;
            if (!audio.isPlaying && !audioPlayed)
            {
                audioPlayed = true;
                int _clipPick = Random.Range(0, 2);
                if (_clipPick == 0)
                    SoundManager.instance.PlayClip(audio, "KATSW1");
                else
                    SoundManager.instance.PlayClip(audio, "KATSW2");
            }
            //RaycastHit _hit;
            //if(Physics.Raycast(playerCam.position, playerCam.forward, out _hit, attackDist, enemy))
            //{
            //    hitboxActivated = false;
            //    _hit.transform.GetComponent<EnemyRobotController>().OnDamaged(damage);
            //    SoundManager.instance.PlayClip(audio, "KATHIT");
            //}
            //else if(!audioPlayed)
            //{
            //    audioPlayed = true;
            //    int _clipPick = Random.Range(0, 2);
            //    if (_clipPick == 0)
            //        SoundManager.instance.PlayClip(audio, "KATSW1");
            //    else
            //        SoundManager.instance.PlayClip(audio, "KATSW2"); 
            //}
        }
        else
            melee.SetActive(false);

        if (checkCancel)
        {
            cTimer += Time.deltaTime;
            if(cTimer >= cancelTime)
            {
                cTimer = 0f;
                checkCancel = false;
                CancelAttack();
            }
        }
    }
    private void Init()
    {
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }
    private void TryAttack()
    {
        if(Input.GetKeyDown(keys.KatAttack) && canAttack)
        {
            canAttack = false; 
            cTimer = 0f;
            checkCancel = false;
            DoAttack(attackStep);
            attackStep = attackStep % 3 + 1;
        }
    }
    private void DoAttack(int _step)
    {
        anim.SetBool("FlaFirst", (_step == 1) ? true : false);
        anim.SetBool("FlaSecond", (_step == 2) ? true : false);
        anim.SetBool("FlaThird", (_step == 3) ? true : false);
        anim.SetTrigger("TriAttack");
    }
    private void CancelAttack()
    {
        attackStep = 1;
        anim.SetTrigger("TriCancel");
        anim.SetBool("FlaFirst", false);
        anim.SetBool("FlaSecond", false);
        anim.SetBool("FlaThird", false);
    }
    public void ActivateHitbox()
    {
        hitboxActivated = true;
    }
    public void DeactivateHitbox()
    {
        hitboxActivated = false;
        checkCancel = true;
    }
    public void ResetFlag()
    {
        canAttack = true;
        audioPlayed = false;
    }
    public void PlayHitSound()
    {
        SoundManager.instance.PlayClip(audio, "KATHIT");
    }
}
