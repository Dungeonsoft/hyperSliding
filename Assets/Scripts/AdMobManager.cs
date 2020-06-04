using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Amond.Plugins.AmondSdkPlugin;
//using GoogleMobileAds.Api;



public class AdMobManager : MonoBehaviour
{
    /*
    private RewardedAd rewardedAd;
    public string adUnitId;

    void Awake()
    {
        AdmobInitialize();
    }

    public void AdmobInitialize()
    {

        // Test ID: ca-app-pub-3940256099942544~3347511713
        // Test 광고 단위 ID: ca-app-pub-3940256099942544/5224354917

        // HyperSlide AdMob ID : ca-app-pub-6271565023912435~3631049944
        // HyperSlide AdMob 광고 단위 ID : ca-app-pub-6271565023912435/6604983051

        #if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/5224354917";                       
            Debug.Log("Android");
        #elif UNITY_IPHONE
            adUnitId = "";
            Debug.Log("iOS ID 입력");
        #else
            adUnitId = "unexpected_platform";
        #endif
            MobileAds.Initialize(initStatus => { });

        RequestInit();
    }

    public void RequestInit()                       //초기화 메소드 RewardedAd 객체는 사용후 바로 소멸되니 매번 사용할때마다 재할당해야함
    {
        this.rewardedAd = new RewardedAd(adUnitId);
        rewardedAd.OnAdClosed += HandleOnAdLoaded;

        AdRequest request = new AdRequest.Builder().Build();

        rewardedAd.LoadAd(request);
    }


    AdType atManager;
    public Amondplugin amdPlugin;


    public void ShowRewardedAd_Admob(AdType at)
    {
        //RequestInit();
        if (rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
            Debug.Log("Ad Mob Show");
        }
        else
        {
            RequestInit();
        }
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)                 //광고 로드가 완료될 때 실행됩니다.
    {
        Debug.Log("애드몹광고보기==완료!");
        amdPlugin.EndWatchingAd(atManager);

        // 무조건 초기화.
        RequestInit();
    }

    public void HandleOnAdFailedToLoad(object sender, EventArgs args)           //광고 로드에 실패할 때 실행됩니다. 제공된 AdErrorEventArgs의 Message 속성은 발생한 실패의 유형을 설명합니다.
    {

    }

    public void HandleOnAdOpening(object sender, EventArgs args)                //광고가 표시될 때 실행되며 기기 화면을 덮습니다. 이때 필요한 경우 앱의 오디오 출력 또는 게임 루프를 일시 중지하는 것이 좋습니다.
    {

    }
    public void HandleOnFailedToShow(object sender, EventArgs args)             //광고 표시에 실패할 때 실행됩니다. 제공된 AdErrorEventArgs의 Message 속성은 발생한 실패의 유형을 설명합니다.
    {

    }

    public void HandlEarnedReward(object sender, EventArgs args)                //사용자가 동영상 시청에 대한 보상을 받아야 할 때 실행됩니다. Reward 매개변수는 사용자에게 제공되는 보상을 설명합니다.
    {

    }
    public void HandleOnAdClosed(object sender, EventArgs args)                 //사용자가 닫기 아이콘을 탭하거나 뒤로 버튼을 사용하여 보상형 동영상 광고를 닫을 때 실행됩니다. 앱에서 오디오 출력 또는 게임 루프를 일시중지했을 때 이 메소드로 재개하면 편리합니다.
    {

    }

    //광고 재생 상황 별 Delegate 문서 https://developers.google.com/admob/unity/rewarded-ads?hl=ko - 사용 가능한 광고 이벤트 탭

    */
    
}
