using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using static Amond.Plugins.AmondSdkPlugin;

public class UnityAdsManager : MonoBehaviour
{
    void Awake()
    {
        Advertisement.Initialize("3565741", true);
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

    // 광고가 종료된 후 자동으로 호출되는 콜백 함수 
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                // 광고를 성공적으로 시청한 경우 보상 지급 
                Debug.Log("광고보기==완료!");
                amdPlugin.EndWatchingAd(atManager);
                //DoSomeRewardAction();
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