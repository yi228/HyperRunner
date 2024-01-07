using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClear : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            if(GameManager.instance.enemyCount == 0)
                GameManager.instance.LoadNextStage();
    }
}
