using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectScene : MonoBehaviour
{
    public void OnClickIngameButton(int _stageNum)
    {
        SceneManager.LoadScene("Stage " +  _stageNum);
    }
}
