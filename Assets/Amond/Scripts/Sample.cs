using Amond.Plugins;
using UnityEngine;
using UnityEngine.UI;
using static Amond.Plugins.AmondSdkPlugin;

    public class Sample : MonoBehaviour
    {
        public GameObject buttonInit;
        public GameObject buttonConnectAmond;
        public GameObject buttonStartWatchAd;
        public GameObject buttonEndWatchAd;
        public GameObject buttonStartGame;
        public GameObject buttonEndGame;
        public GameObject buttonLeaderBoardUrl;
        public Text textResult;

        private static void CheckAvailable(string value)
        {
            var sample = GameObject.Find("AdmondPlugin").GetComponent<Sample>();
            if (AmondPlugin.GetInstance().IsAvailable)
            {
                sample.textResult.text = "Connect Amond is available";
                // Connect Amond
                sample.ConnectAmond();
            }
            else
            {
                sample.textResult.text = "Connect Amond is not available";
            }
        }

        /// <summary>
        /// 1. Init: SDK 초기화, 콜백 등록
        /// </summary>
        private void Init()
        {
            // IsAvailable Callback
            AmondPlugin.GetInstance().SetCallbackIsAvailable = Sample.CheckAvailable;
            // Init
            AmondPlugin.GetInstance().Init(EnvironmentType.Stage, "game-hyper-sliding");
            buttonInit.SetActive(false);
        }

        /// <summary>
        /// 2. ConnectAmond
        /// </summary>
        private void ConnectAmond()
        {
            var result = AmondPlugin.GetInstance().ConnectAmond();
            if (result != null)
            {
                textResult.text = "2. ConnectAmond User ID: " + result.userId +
                                  "\n" + result;
                buttonConnectAmond.SetActive(false);
                buttonStartWatchAd.SetActive(true);
            }
            else
            {
                textResult.text = "Failed ConnectAmond()";
            }
        }

        /// <summary>
        /// 3. StartWatchingAd
        /// </summary>
        private void StartWatchingAd()
        {
            var transactionId = AmondPlugin.GetInstance().StartWatchingAd(AdType.Reward);
            if (transactionId > 0)
            {
                textResult.text = "3. Transaction ID: " + transactionId;
                buttonStartWatchAd.SetActive(false);
                buttonEndWatchAd.SetActive(true);
            }
            else
            {
                textResult.text = "Failed StartWatchingAd()";
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
                textResult.text = "4. " + result;
                buttonEndWatchAd.SetActive(false);
                buttonStartGame.SetActive(true);
            }
            else
            {
                textResult.text = "Failed EndWatchingAd()";
            }
        }

        /// <summary>
        /// 5. StartGame
        /// </summary>
        private void StartGame()
        {
            var result = AmondPlugin.GetInstance().StartGame();
            if (result != null)
            {
                textResult.text = "5. Game ticket: " + result;
                buttonStartGame.SetActive(false);
                buttonEndGame.SetActive(true);
            }
            else
            {
                textResult.text = "Failed StartGame()";
            }
        }

        /// <summary>
        /// 6. EndGame
        /// </summary>
        private void EndGame()
        {
            var result = AmondPlugin.GetInstance().EndGame(1000);
            if (result != null)
            {
                textResult.text = "6. Rank: " + result.rank +
                                  "\n Best score: " + result.score;
                buttonEndGame.SetActive(false);
                buttonLeaderBoardUrl.SetActive(true);
            }
            else
            {
                textResult.text = "Failed EndGame()";
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
                textResult.text = "7. LeaderBoardUrl: " + result;
                buttonLeaderBoardUrl.SetActive(false);
            }
            else
            {
                textResult.text = "Failed LeaderBoardUrl()";
            }
        }
    }