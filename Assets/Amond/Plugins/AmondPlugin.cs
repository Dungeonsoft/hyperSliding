using System;
using JetBrains.Annotations;
using UnityEngine;
using static Amond.Plugins.AmondSdkPlugin;

namespace Amond.Plugins
{
    public class AmondPlugin
    {
        public bool Available;
        private static AmondPlugin _instance;
        private readonly AmondSdkPlugin _amondSdkPlugin;

        public delegate void CallbackIsAvailable(string value);
        public CallbackIsAvailable CallbackAvailable;

        private AmondPlugin()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    _amondSdkPlugin = new AndroidPlugin();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    _amondSdkPlugin = new IPhonePlugin();
                    break;
                default:
                    _amondSdkPlugin = new EditorPlugin();
                    break;
            }
        }

        public static AmondPlugin GetInstance()
        {
            return _instance ?? (_instance = new AmondPlugin());
        }

        /// <summary>
        /// Android SDK Callback 값 변경
        /// </summary>
        /// <param name="value"></param>
        public void SetAvailable(string value)
        {
            Debug.Log("SetAvailable: " + value);
            if (value.ToLower().Equals("true"))
            {
                Available = true;
            }
        }

        /// <summary>
        /// 1. Init: SDK 초기화
        /// </summary>
        /// <param name="environment">개발 환경</param>
        /// <param name="gameId">게임 ID</param>
        /// <returns>bool</returns>
        public void Init(EnvironmentType environment, string gameId)
        {
            _amondSdkPlugin.Init(environment, gameId);
        }

        /// <summary>
        /// 2. ConnectAmond
        /// </summary>
        /// <returns>GameScoreDto</returns>
        public GameScoreDto ConnectAmond()
        {
            string result = _amondSdkPlugin.ConnectAmond();
            return result == null ? DefaultGameScore() : GetGameScore();
        }

        private static GameScoreDto DefaultGameScore()
        {
            return new GameScoreDto {nickname = "Unnamed", profileImageUrl = "", rank = 0, score = 0, totalPoint = 0};
        }

        private GameScoreDto GetGameScore()
        {
            if (Available == false) return null;

            string result = _amondSdkPlugin.GetGameScore();
            if (result == null) return null;
            Debug.Log("GetGameScore: " + result);
            return JsonUtility.FromJson<GameScoreDto>(result);
        }

        /// <summary>
        /// 3. StartWatchingAd
        /// </summary>
        /// <param name="adType"></param>
        /// <returns></returns>
        public bool StartWatchingAd(AdType adType)
        {
            if (Available == false) return false;

            bool result = _amondSdkPlugin.StartWatchingAd(adType);
            Debug.Log("StartWatchingAd: " + result);
            return result;
        }

        /// <summary>
        /// 4. EndWatchingAd
        /// </summary>
        /// <returns></returns>
        public bool EndWatchingAd()
        {
            if (Available == false) return false;

            bool result = _amondSdkPlugin.EndWatchingAd();
            Debug.Log("EndWatchingAd: " + result);
            return result;
        }

        /// <summary>
        /// 5. StartGame
        /// </summary>
        /// <returns></returns>
        public bool StartGame()
        {
            if (Available == false) return false;

            bool result = _amondSdkPlugin.StartGame();
            Debug.Log("StartGame: " + result);
            return result;
        }

        /// <summary>
        /// 6. EndGame
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public GameScoreDto EndGame(int score)
        {
            if (Available == false) return null;

            string result = _amondSdkPlugin.EndGame(score);
            if (result == null) return null;
            Debug.Log("EndGame: " + result);
            return JsonUtility.FromJson<GameScoreDto>(result);
        }

        /// <summary>
        /// 7. GetLeaderBoardUrl
        /// </summary>
        /// <returns></returns>
        public string GetLeaderBoardUrl()
        {
            if (Available == false) return null;

            string result = _amondSdkPlugin.GetLeaderBoardUrl();
            if (result == null) return null;
            Debug.Log("GetLeaderBoardUrl: " + result);
            return result;
        }

        public void OpenLeaderBoard()
        {
            _amondSdkPlugin.OpenLeaderBoard();
        }
    }

    public class AndroidPlugin : AmondSdkPlugin
    {
        //1. Init
        //2. ConnectAmond
        //3. StartWatchingAd
        //4. EndWatchingAd
        //5. StartGame
        //6. EndGame
        //7. OpenLeaderBoard

        private const string PluginName = "io.amond.sdk.unity.PluginHelper";

        private static AndroidJavaClass _pluginClass;
        private static AndroidJavaObject _pluginInstance;

        private static AndroidJavaClass PluginClass
        {
            get
            {
                if (_pluginClass != null) return _pluginClass;
                _pluginClass = new AndroidJavaClass(PluginName);
                AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                _pluginClass.SetStatic("mainActivity", activity);
                return _pluginClass;
            }
        }

        private static AndroidJavaObject PluginInstance
        {
            get
            {
                if (_pluginInstance != null) return _pluginInstance;
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
                return _pluginInstance;
            }
        }

        public override void Init(EnvironmentType environment, string gameId)
        {
            //Android Callback insert
            GameObject amondCallback = GameObject.Find("AmondCallback");
            if (amondCallback == null)
            {
                Debug.Log("Make AmondCallback");
                amondCallback = new GameObject {name = "AmondCallback"};
                amondCallback.AddComponent<AmondCallback>();
            }

            Debug.Log("Call SDK init()");
            PluginInstance.Call<bool>("init", (int) environment, gameId);
        }

        public override string ConnectAmond()
        {
            Debug.Log("Call SDK connectAmond()");
            return PluginInstance.Call<string>("connectAmond");
        }

        public override string GetGameScore()
        {
            Debug.Log("Call SDK getGameScore()");
            return PluginInstance.Call<string>("getGameScore");
        }

        public override bool StartWatchingAd(AdType adType)
        {
            return PluginInstance.Call<bool>("startWatchingAd", (int) adType);
        }

        public override bool EndWatchingAd()
        {
            return PluginInstance.Call<bool>("endWatchingAd");
        }

        public override bool StartGame()
        {
            return PluginInstance.Call<bool>("startGame");
        }

        public override string EndGame(int score)
        {
            return PluginInstance.Call<string>("endGame", score);
        }

        public override string GetLeaderBoardUrl()
        {
            return PluginInstance.Call<string>("getLeaderBoardUrl");
        }

        public override void OpenLeaderBoard()
        {
            PluginInstance.Call("openLeaderBoard");
        }
    }

    public class IPhonePlugin : AmondSdkPlugin
    {
        public override void Init(EnvironmentType environment, string gameId)
        {
            throw new NotImplementedException();
        }

        public override string ConnectAmond()
        {
            throw new NotImplementedException();
        }

        public override string GetGameScore()
        {
            throw new NotImplementedException();
        }

        public override bool StartWatchingAd(AdType adType)
        {
            throw new NotImplementedException();
        }

        public override bool EndWatchingAd()
        {
            throw new NotImplementedException();
        }

        public override bool StartGame()
        {
            throw new NotImplementedException();
        }

        public override string EndGame(int score)
        {
            throw new NotImplementedException();
        }

        public override string GetLeaderBoardUrl()
        {
            throw new NotImplementedException();
        }

        public override void OpenLeaderBoard()
        {
            throw new NotImplementedException();
        }
    }

    public class EditorPlugin : AmondSdkPlugin
    {
        private const string DefaultImageUrl = "http://game.stage.amond.io/images/drawable-xhdpi/avatar_01.png";

        public override void Init(EnvironmentType environment, string gameId)
        {
            Debug.Log("Call Init()");
            AmondPlugin.GetInstance().SetAvailable("true");
            AmondPlugin.GetInstance().CallbackAvailable("true");
        }

        public override string ConnectAmond()
        {
            return "{\"userId\":1004,\"nickname\":\"Unnamed\"}";
        }

        public override string GetGameScore()
        {
            return "{\"userId\":1004,\"nickname\":\"Unnamed\",\"profileImageUrl\":\"" + DefaultImageUrl + "\"" +
                   ",\"rank\":1,\"totalPoint\":100,\"score\":1000}";
        }

        public override bool StartWatchingAd(AdType adType)
        {
            return true;
        }

        public override bool EndWatchingAd()
        {
            return true;
        }

        public override bool StartGame()
        {
            return true;
        }

        public override string EndGame(int score)
        {
            return "{\"userId\":1004,\"nickname\":\"Unnamed\",\"profileImageUrl\":\"" + DefaultImageUrl + "\"" +
                   ",\"rank\":1,\"totalPoint\":100,\"score\":" + score + "}";
        }

        public override string GetLeaderBoardUrl()
        {
            return "http://stage.ranker24.com/ranking?token=eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiMTAwNSIsInVzZXJfbmFtZSI6IiIsInNjb3BlIjpbIm9wZW5pZCJdLCJleHAiOjE1ODg5MDgyNTEsImF1dGhvcml0aWVzIjpbIlJPTEVfVVNFUiJdLCJqdGkiOiJkOTE3YmU2YS01OWE5LTQ0NTYtOTRhYy05NGU0MjM3Y2RlYjQiLCJjbGllbnRfaWQiOiJnYW1lLWh5cGVyLXNsaWRpbmcifQ.PNsWNRga_qGS6sX0tQM7uJT0fKloq9SFm1Rc5-t5a1nA_EciSFiJq4gs4AHqMY4XUK73LT1rdmhBYEnjXt2AdNELMfTWwLuhz6emClv2BZVH3Z8SVEDdwExXy4mXtvgSlGbDlFNBh1YCzj1AiLiRUMmnvmxi4wkYeTq2rLbmcWtCtd-ymPc0sfMDlSPcFwp0NolDoO98eSgHzh8wBsiskm7FtVUt8z_f5XmYhR12CiCtQMeOYl55VZkrtueha9rW6Vvioum2Qe2JtzVSQ_Z4nN2_DjFoWjjWS44QCS3YxG9wDB9efUh0ApPru7zGy4olQUxu0OvP6q8mZOG3dlT_2g";
        }

        public override void OpenLeaderBoard()
        {
            Application.OpenURL(GetLeaderBoardUrl());
        }
    }

    public abstract class AmondSdkPlugin
    {
        public enum EnvironmentType
        {
            Dev,
            Stage,
            Prod
        }

        public enum AdType
        {
            Forced,
            Reward,
            GameItem,
            GameContinue
        }

        public abstract void Init(EnvironmentType environment, string gameId);

        public abstract string ConnectAmond();

        public abstract string GetGameScore();

        public abstract bool StartWatchingAd(AdType adType);

        public abstract bool EndWatchingAd();

        public abstract bool StartGame();

        public abstract string EndGame(int score);

        public abstract string GetLeaderBoardUrl();

        public abstract void OpenLeaderBoard();
    }

    [Serializable]
    public class GameScoreDto
    {
        public long userId;
        public string nickname;
        public string profileImageUrl;
        public long rank;
        public int score;
        public int prizeLowScore = 0;
        public int prizeHighScore = 0;
        public long totalPoint;
        public string gameTicket;

        public override string ToString()
        {
            return "GameScoreDto{" +
                   "userId=" + userId +
                   ", nickname='" + nickname + '\'' +
                   ", profileImageUrl='" + profileImageUrl + '\'' +
                   ", rank=" + rank +
                   ", score=" + score +
                   ", prizeLowScore=" + prizeLowScore +
                   ", prizeHighScore=" + prizeHighScore +
                   ", totalPoint=" + totalPoint +
                   ", gameTicket='" + gameTicket + '\'' +
                   '}';
        }
    }
}