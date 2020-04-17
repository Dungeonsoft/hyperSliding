using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailWin : MonoBehaviour
{
    public List<GameObject> counts;
    int countOfCount;
    float spandTime;
    int oldNowTime;



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

        // 모든 숫자를 꺼준다.
        foreach(var v in counts)
        {
            v.SetActive(false);
        }
        // 먼저 숫자 10을 켜준다.
        counts[0].SetActive(true);

        countOfCount = counts.Count;

        oldNowTime = 0;
        // 초기화 할것 이곳에 먼저 정리.

        uAction = Count;
    }
   
    void Count()
    {
        spandTime += Time.deltaTime;
        var nowTime = Mathf.RoundToInt(spandTime);
        if (nowTime == oldNowTime)
        {
            return;
        }
        if (nowTime >= 11)
        {
            CloseWin();
        }
        else
        {
            oldNowTime = nowTime;
            for (int i = 0; i < countOfCount; i++)
            {
                if (i == nowTime) counts[i].SetActive(true);
                else counts[i].SetActive(false);
            }
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
