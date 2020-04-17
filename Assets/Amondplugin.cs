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

    private void Awake()
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
        Debug.Log("Set User Data 진입");
        
        // 프로필 이미지 표시.
        StartCoroutine(GetProfileImage(uData.profileImageUrl));
        // 닉네임 표시.
        nickName.text = uData.nickname;
        // 아몬드 포인트 표시.
        amondPoint.text = uData.totalPoint.ToString();
        //랭크 표시.
        uRank.text = uData.rank.ToString();
        //베스트 스코어 표시.
        bestScore.text = uData.score.ToString();
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
        if (AmondPlugin.GetInstance().IsAvailable)
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
        AmondPlugin.GetInstance().SetCallbackIsAvailable = Amondplugin.CheckAvailable;
        // Init
        AmondPlugin.GetInstance().Init(EnvironmentType.Stage, "game-hyper-sliding");
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

    /// <summary>
    /// 3. StartWatchingAd
    /// 광고보기.
    /// </summary>
    private void StartWatchingAd()
    {
        var transactionId = AmondPlugin.GetInstance().StartWatchingAd(AdType.Reward);
        if (transactionId > 0)
        {
            Debug.Log("3. Transaction ID: " + transactionId);
        }
        else
        {
            Debug.Log("Failed StartWatchingAd()");
        }
    }

    /// <summary>
    /// 4. EndWatchingAd
    /// </summary>
    private void EndWatchingAd()
    {
        var result = AmondPlugin.GetInstance().EndWatchingAd(AdType.Reward);
        if (result != null)
        {
            Debug.Log("4. " + result);
        }
        else
        {
            Debug.Log("Failed EndWatchingAd()");
        }
    }

    /// <summary>
    /// 5. StartGame
    /// </summary>
    public void StartGame()
    {
        var result = AmondPlugin.GetInstance().StartGame();
        if (result != null)
        {
            Debug.Log("5. Game ticket: " + result);
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

}

public delegate void GetEndData(GameScoreDto result);