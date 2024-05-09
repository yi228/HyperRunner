using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private TextMeshProUGUI stageText;
    public int enemyCount = 0;
    private int curStageNum;

    void Start()
    {
        instance = this;
        string _temp = SceneManager.GetActiveScene().name;
        _temp = _temp.Substring(_temp.Length - 1);
        curStageNum = int.Parse(_temp);
        EnemyRobotController[] _enemyList = FindObjectsByType<EnemyRobotController>(FindObjectsSortMode.None);
        enemyCount = _enemyList.Length;
    }
    void Update()
    {
        SetTextUI();
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Start");
    }
    private void SetTextUI()
    {
        StringBuilder _enemyTmp = new StringBuilder();
        _enemyTmp.Append("남은 적: ");
        _enemyTmp.Append(enemyCount.ToString());
        enemyCountText.text = _enemyTmp.ToString();
        StringBuilder _stageTmp = new StringBuilder();
        _stageTmp.Append("스테이지 ");
        _stageTmp.Append(curStageNum.ToString());
        stageText.text = _stageTmp.ToString();
    }
    public void LoadNextStage()
    {
        if(curStageNum < 3)
        {
            StringBuilder _nextSceneName = new StringBuilder();
            _nextSceneName.Append("Stage ");
            _nextSceneName.Append(++curStageNum);
            SceneManager.LoadScene(_nextSceneName.ToString());
        }
        else
        {
            SceneManager.LoadScene("Ending");
        }
    }
}
