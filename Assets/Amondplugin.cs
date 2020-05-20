using Amond.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Amond.Plugins.AmondSdkPlugin;

public class Amondplugin : MonoBehaviour
{
    public Image profileImage;
    public TextMeshProUGUI nickName;
    public TextMeshProUGUI amondPoint;
    public TextMeshProUGUI uRank;
    public TextMeshProUGUI bestScore;
    public TextMeshProUGUI goalScore;


    private void Awake()
    {
        Init();

        youGotAmondWin.SetActive(false);
    }

    public void InitForced()
    {
        Init();
    }
    /// <summary>
    /// //////////////////////////
    /// GameScoreDto uData
    ///     long: userId
    ///     string: nickname
    ///     string: profileImageUrl
    ///     long: rank
    ///     int: score
    ///     long: totalPoint
    ///     string: gameTicket
    /// //////////////////////////
    /// </summary>
    /// <param name="uData"></param>
    void SetUserData(GameScoreDto uData)
    {
        Debug.Log("Set User Data 진입: "+ uData);
        
        // 프로필 이미지 표시.
        StartCoroutine(GetProfileImage(uData.profileImageUrl));
        // 닉네임 표시.
        nickName.text = uData.nickname;
        // 아몬드 포인트 표시.
        amondPoint.text = uData.totalPoint.ToString()+"P";
        // 랭크 표시.
        uRank.text = uData.rank.ToString();
        // 베스트 스코어 표시.
        bestScore.text = uData.score.ToString();
        // 목표 스코어 범위 표시.
        goalScore.text = uData.prizeLowScore + " ~ " + uData.prizeHighScore;

    }

    IEnumerator GetProfileImage(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        profileImage.sprite = Sprite.Create(
            www.texture, 
            new Rect(0, 0, www.texture.width, www.texture.height), 
            new Vector2(0.5f, 0.5f)); 
    }


    private static void CheckAvailable(string value)
    {
        var Amondplugin = GameObject.Find("AmondPluginGO").GetComponent<Amondplugin>();
        if (AmondPlugin.GetInstance().Available)
        {
            Debug.Log("Connect Amond is available");
            // Connect Amond
            Amondplugin.ConnectAmond();
        }
        else
        {
            Debug.Log("Connect Amond is not available");
        }
    }

    /// <summary>
    /// 1. Init: SDK 초기화, 콜백 등록
    /// </summary>
    private void Init()
    {
        // IsAvailable Callback
        AmondPlugin.GetInstance().CallbackAvailable = Amondplugin.CheckAvailable;
        // Init
        AmondPlugin.GetInstance().Init(EnvironmentType.Prod, "game-hyper-sliding");
        //buttonInit.SetActive(false);
    }

    /// <summary>
    /// 2. ConnectAmond
    /// </summary>
    private void ConnectAmond()
    {
        var result = AmondPlugin.GetInstance().ConnectAmond();
        if (result != null)
        {
            Debug.Log("2. ConnectAmond User ID: " + result.userId + "\n" + result);

            // 연결이 완료되었으니 불러온 유저데이터를 이용하여 화면을 구성한다.
            SetUserData(result);
        }
        else
        {
            Debug.Log("Failed ConnectAmond()");
        }
    }

    public GameManager gManager;
    public IntroManager iManager;
    public UnityAdsManager uaManager;


    public void StartWatchingAdNum(int atNum)
    {
        if (gManager.isShowContinueCm == true) return;
        gManager.isShowContinueCm = true;
        Debug.Log("atNum: "+ atNum);
        AdType at = (AdType)atNum;
        Debug.Log("atName: " + at);

        StartWatchingAd(at);
    }
    /// <summary>
    /// 3. StartWatchingAd
    /// 광고보기.
    /// </summary>
    public void StartWatchingAd(AdType at)
    {
        bool result = AmondPlugin.GetInstance().StartWatchingAd(at);
        if (result)
        {
            Debug.Log("3. result: " + result);

            uaManager.ShowRewardedAd(at);
        }
        else
        {
            Debug.Log("Failed StartWatchingAd()");
        }
    }


    /// <summary>
    /// 4. EndWatchingAd
    /// </summary>
    public void EndWatchingAd(AdType at)
    {
        var result = AmondPlugin.GetInstance().EndWatchingAd();
        if (result)
        {
            Debug.Log("4. " + result);




            switch (at)
            {
                case AdType.GameItem:
                    Debug.Log("광고보기 완료: 게임 아이템");
                    iManager.SuccessCM_GmaeItem();
                    break;

                case AdType.GameContinue:
                    Debug.Log("광고보기 완료: 게임 컨티뉴");
                    gManager.ContinueCM();
                    break;

                case AdType.Reward:
                    Debug.Log("광고보기 완료: 게임 리워드");
                    ShowYouGotAmondWin();
                    break;
            }


        }
        else
        {
            Debug.Log("Failed EndWatchingAd()");
        }
    }


    public GameObject youGotAmondWin;
    void ShowYouGotAmondWin()
    {
        youGotAmondWin.SetActive(true);
    }


    /// <summary>
    /// 5. StartGame
    /// </summary>
    public void StartGame()
    {
        var result = AmondPlugin.GetInstance().StartGame();
        if (result)
        {
            Debug.Log("5. Game start: " + result);
        }
        else
        {
            Debug.Log("Failed StartGame()");
        }
    }

    /// <summary>
    /// 6. EndGame
    /// </summary>
    public void EndGame(int endScore, GetEndData aDel)
    {
        var result = AmondPlugin.GetInstance().EndGame(endScore);
        if (result != null)
        {
            Debug.Log("6. Rank: " + result.rank + "\n Best score: " + result.score);
            
            // 결과값 화면에 출력
            aDel(result);

        }
        else
        {
            Debug.Log("Failed EndGame() ==  결과값을 못 받았을 때");
        }
    }

    /// <summary>
    /// 7. GetLeaderBoardUrl
    /// </summary>
    private void GetLeaderBoardUrl()
    {
        var result = AmondPlugin.GetInstance().GetLeaderBoardUrl();
        if (result != null)
        {
            Debug.Log("7. LeaderBoardUrl: " + result);
        }
        else
        {
            Debug.Log("Failed LeaderBoardUrl()");
        }
    }

    public GameObject webPanel_Canvas;

    public void OpenLeaderBoard()
    {
        AmondPlugin.GetInstance().OpenLeaderBoard();
    }

    public void CloseLeaderBoard()
    {
        //Debug.Log("Close Leader Board!!!");
        //AmondPlugin.GetInstance().CloseLeaderBoard(CloseWebPanel);
    }

    public void CloseWebPanel(int num)
    {
        //if (num == 1)
        //    webPanel_Canvas.SetActive(false);
    }

}

public delegate void GetEndData(GameScoreDto result);