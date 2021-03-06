﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using static Amond.Plugins.AmondSdkPlugin;

public class UnityAdsManager : MonoBehaviour
{
    private string placementId;

    void Awake()
    {
        UnityAdsInitialize();
    }

    public void UnityAdsInitialize()                                                // Unity Ads 초기화 메소드
    {
#if UNITY_ANDROID
        placementId = "3565741";                                                     // 안드로이드 ID값 입력 
        Debug.Log("Android");                                                       // iOS ID값 입력
#elif UNITY_IPHONE
        placemenId = "xxxxxxx";
        Debug.Log("Iphone");
#else
#endif

        Advertisement.Initialize(placementId, false);                                // true값일 경우 Test mode, false 시 실제 광고 재생
    }



    ////////////SimpleAdsManger///////////
    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show("video");
        }
    }
    ////////////SimpleAdsManger///////////


    ////////////RewardedAdsManger///////////
    public void ShowRewardedAd(AdType at)
    {
        atManager = at;
        if (Advertisement.IsReady("rewardedVideo"))
        {
            // 광고가 끝난 뒤 콜백함수 "HandleShowResult" 호출 
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    AdType atManager;

    public  Amondplugin amdPlugin;

    public GameObject gameFailWin;
    // 광고가 종료된 후 자동으로 호출되는 콜백 함수 
    private void HandleShowResult(ShowResult result)
    {
        gameFailWin.GetComponent<FailWin>().ShowCmToggleOn(false);
        switch (result)
        {
            case ShowResult.Finished:
                // 광고를 성공적으로 시청한 경우 보상 지급 
                Debug.Log("유니티애즈광고보기==완료!");
                amdPlugin.EndWatchingAd(atManager);
                break;


            case ShowResult.Skipped:
                // 스킵 되었다면 뭔가 그런짓을 하면 보상을 줄 수 없잖아! 같은 창을 띄워야곘죠?
                Debug.Log("광고보기==스킵!");
                //DoSomeSkippedAction();
                break;
            case ShowResult.Failed:
                Debug.Log("광고보기==실패!");
                //DoSomeSkippedFailed();
                break;
        }
    }

}
////////////RewardedAdsManger///////////