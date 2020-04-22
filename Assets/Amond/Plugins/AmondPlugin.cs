using System;
using UnityEngine;
using static Amond.Plugins.AmondSdkPlugin;

namespace Amond.Plugins
{
    public class AmondPlugin
    {
        public bool IsAvailable;
        private static AmondPlugin _instance;
        private readonly AmondSdkPlugin _amondSdkPlugin;
        
        // 2020-04-21 추가.
        private AdType _adType = AdType.Forced;
        private long _transactionId;
        private string _gameTicket = "";

        public delegate void CallbackIsAvailable(string value);

        public CallbackIsAvailable SetCallbackIsAvailable;

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
                IsAvailable = true;
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
            return new GameScoreDto { nickname = "Unnamed", profileImageUrl = "", rank = 0, score = 0, totalPoint = 0 };
        }


        private GameScoreDto GetGameScore()
        {
            if (IsAvailable == false) return null;

            var result = _amondSdkPlugin.GetGameScore();
            if (result == null) return null;
            Debug.Log("GetGameScore: " + result);
            return JsonUtility.FromJson<GameScoreDto>(result);
        }

        /// <summary>
        /// 3. StartWatchingAd
        /// </summary>
        /// <param name="adType"></param>
        /// <returns></returns>
        public long StartWatchingAd(AdType adType)
        {
            if (IsAvailable == false) return 0;

            string result = _amondSdkPlugin.StartWatchingAd(adType);
            if (result == null) return 0;
            Debug.Log("StartWatchingAd: " + result);
            AdWatchDto adWatchDto = JsonUtility.FromJson<AdWatchDto>(result);
            _adType = adType;
            _transactionId = adWatchDto.transactionId;
            return adWatchDto.transactionId;
        }


        /// <summary>
        /// 4. EndWatchingAd
        /// </summary>
        /// <returns></returns>
        public AdWatchDto EndWatchingAd()
        {
            if (IsAvailable == false) return null;

            string result = _amondSdkPlugin.EndWatchingAd(_adType, _transactionId);
            if (result == null) return null;
            Debug.Log("EndWatchingAd: " + result);
            AdWatchDto adWatchDto = JsonUtility.FromJson<AdWatchDto>(result);
            return adWatchDto;
        }

        /// <summary>
        /// 5. StartGame
        /// </summary>
        /// <returns></returns>
        public string StartGame()
        {
            if (IsAvailable == false) return null;

            string result = _amondSdkPlugin.StartGame();
            if (result == null) return null;
            Debug.Log("StartGame: " + result);
            GameScoreDto gameScoreDto = JsonUtility.FromJson<GameScoreDto>(result);
            _gameTicket = gameScoreDto.gameTicket;
            return gameScoreDto.gameTicket;
        }


        /// <summary>
        /// 6. EndGame
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public GameScoreDto EndGame(int score)
        {
            if (IsAvailable == false) return null;

            var result = _amondSdkPlugin.EndGame(_gameTicket, score);
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
            if (IsAvailable == false) return null;

            var result = _amondSdkPlugin.GetLeaderBoardUrl();
            if (result == null) return null;
            Debug.Log("GetLeaderBoardUrl: " + result);
            return result;
        }

        public void OpenLeaderBoard()
        {
            _amondSdkPlugin.OpenLeaderBoard();
        }

        public void CloseLeaderBoard(Action<int> closeComplete)
        {
            _amondSdkPlugin.CloseLeaderBoard(closeComplete);
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
        //7. GetLeaderBoardUrl

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

        private class ReturnCallback : AndroidJavaProxy
        {
            private readonly Action<int> _returnHandler;

            public ReturnCallback(Action<int> returnHandlerIn) : base(PluginName + "$ReturnCallback")
            {
                _returnHandler = returnHandlerIn;
            }

            // ReSharper disable once InconsistentNaming
            public void onComplete(int result)
            {
                Debug.Log("onComplete:" + result);
                if (_returnHandler != null)
                    _returnHandler(result);
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

        public override string StartWatchingAd(AdType adType)
        {
            return PluginInstance.Call<string>("startWatchingAd", (int) adType);
        }

        public override string EndWatchingAd(AdType adType, long transactionId)
        {
            return PluginInstance.Call<string>("endWatchingAd", (int) adType, transactionId);
        }

        public override string StartGame()
        {
            return PluginInstance.Call<string>("startGame");
        }

        public override string EndGame(string gameTicket, int score)
        {
            return PluginInstance.Call<string>("endGame", gameTicket, score);
        }

        public override string GetLeaderBoardUrl()
        {
            return PluginInstance.Call<string>("getLeaderBoardUrl");
        }

        public override void OpenLeaderBoard()
        {
            PluginInstance.Call("openLeaderBoard");
        }

        public override void CloseLeaderBoard(Action<int> closeComplete)
        {
            PluginInstance.Call("closeLeaderBoard", new ReturnCallback(closeComplete));
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

        public override string StartWatchingAd(AdType adType)
        {
            throw new NotImplementedException();
        }

        public override string EndWatchingAd(AdType adType, long transactionId)
        {
            throw new NotImplementedException();
        }

        public override string StartGame()
        {
            throw new NotImplementedException();
        }

        public override string EndGame(string gameTicket, int score)
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

        public override void CloseLeaderBoard(Action<int> closeComplete)
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
            AmondPlugin.GetInstance().SetCallbackIsAvailable("true");
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

        public override string StartWatchingAd(AdType adType)
        {
            return "{\"transactionId\":1004}";
        }

        public override string EndWatchingAd(AdType adType, long transactionId)
        {
            return "{\"adType\":\"" + adType + "\",\"transactionId\":1004}";
        }

        public override string StartGame()
        {
            return "{\"userId\":1004,\"gameTicket\":\"TEST-TICKET\"}";
        }

        public override string EndGame(string gameTicket, int score)
        {
            return "{\"userId\":1004,\"nickname\":\"Unnamed\",\"profileImageUrl\":\"" + DefaultImageUrl + "\"" +
                   ",\"rank\":1,\"totalPoint\":100,\"score\":" + score + "}";
        }

        public override string GetLeaderBoardUrl()
        {
            return "http://game.stage.amond.io/ranking?userId=1004&deviceId=2d8b2dd0&clientId=clientId";
        }

        public override void OpenLeaderBoard()
        {
            throw new NotImplementedException();
        }

        public override void CloseLeaderBoard(Action<int> closeComplete)
        {
            throw new NotImplementedException();
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

        public abstract string StartWatchingAd(AdType adType);

        public abstract string EndWatchingAd(AdType adType, long transactionId);

        public abstract string StartGame();

        public abstract string EndGame(string gameTicket, int score);

        public abstract string GetLeaderBoardUrl();

        public abstract void OpenLeaderBoard();

        public abstract void CloseLeaderBoard(Action<int> closeComplete);
    }

    [Serializable]
    public class AdWatchDto
    {
        public long transactionId;

        public override string ToString()
        {
            return "AdWatchDto{" +
                   "transactionId=" + transactionId +
                   '}';
        }
    }

    [Serializable]
    public class GameScoreDto
    {
        public long userId;
        public string nickname;
        public string profileImageUrl;
        public long rank;
        public int score;
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
                   ", totalPoint=" + totalPoint +
                   ", gameTicket='" + gameTicket + '\'' +
                   '}';
        }
    }
}