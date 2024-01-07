using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OnClickStageButton()
    {
        SceneManager.LoadScene("StageSelect");
    }
    public void OnClickExitButton()
    {
        Application.Quit();
    }
}
