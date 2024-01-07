using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreatureController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int hp;
    [SerializeField] protected int attack;
    [SerializeField] protected int maxHp;

    public void OnDamaged(int _damage)
    {
        if (hp > 0)
        {
            Debug.Log(gameObject.name + " on damaged");
            hp -= _damage;
            if (GetComponent<PlayerController>() != null)
                GetComponent<PlayerController>().PlayDamageSound();
            if (hp <= 0)
                OnDead();
        }
    }
    private void OnDead()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            GetComponent<Animator>().SetTrigger("TriDie");
            GameManager.instance.enemyCount--;
            Destroy(gameObject,2f);
        }
        else if (gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
