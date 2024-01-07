using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeZone : MonoBehaviour
{
    public int damage;
    [SerializeField] private KatanaSwing katana;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyRobotController>().OnDamaged(damage);
            katana.PlayHitSound();
        }
    }
}
