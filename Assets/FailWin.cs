using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailWin : MonoBehaviour
{
    public TMPro.TextMeshProUGUI count;
    public int failCountStartDgit;

    float spandTime;


    System.Action uAction;

    public GameManager inGameManager;
    public GameObject intro;
    public GameObject inGame;
    public GameObject GameFail;


    void OnEnable()
    {
        // 초기화 할것 이곳에 먼저 정리.
        spandTime = 0;
        inGameManager.TouchPause();


        // 초기화 할것 이곳에 먼저 정리.

        uAction = Count;
    }

    void Count()
    {
        spandTime += Time.deltaTime;
        var nowTime = failCountStartDgit - Mathf.RoundToInt(spandTime);
        count.text = (nowTime-1).ToString();
        if(nowTime<=0)
        {
            CloseWin();
        }
    }

    private void Update()
    {
        if(uAction != null)
        uAction();
    }

    public void CloseWin()
    {
        inGameManager.TouchPause();
        intro.SetActive(true);
        inGame.SetActive(false);
        GameFail.SetActive(false);
        uAction = null;
    }

}
