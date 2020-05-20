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
    public GameObject buttonLeaderBoard;
    public GameObject buttonCloseLeaderBoard;
    public GameObject inputScore;
    public RectTransform webPanel;
    public Text textResult;

    private InputField _inputScoreField;

    private static void CheckAvailable(string value)
    {
        Sample sample = GameObject.Find("AdmondPlugin").GetComponent<Sample>();
        if (!AmondPlugin.GetInstance().Available) return;

        sample.textResult.text = "Connect Amond is available";
        // Connect Amond
        sample.ConnectAmond();
    }

    /// <summary>
    /// 1. Init: SDK 초기화, 콜백 등록
    /// </summary>
    public void Init()
    {
        // IsAvailable Callback
        AmondPlugin.GetInstance().CallbackAvailable = Sample.CheckAvailable;
        // Init
        AmondPlugin.GetInstance().Init(EnvironmentType.Stage, "game-hyper-sliding");
        buttonInit.SetActive(false);
    }

    /// <summary>
    /// 2. ConnectAmond
    /// </summary>
    public void ConnectAmond()
    {
        GameScoreDto result = AmondPlugin.GetInstance().ConnectAmond();
        textResult.text = "2. ConnectAmond: " + result;
        buttonConnectAmond.SetActive(false);
        buttonStartWatchAd.SetActive(true);
        buttonLeaderBoard.SetActive(true);
    }

    /// <summary>
    /// 3. StartWatchingAd
    /// </summary>
    public void StartWatchingAd()
    {
        bool result = AmondPlugin.GetInstance().StartWatchingAd(AdType.GameItem);
        if (result)
        {
            textResult.text = "3. StartWatchingAd: Success";
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
    public void EndWatchingAd()
    {
        bool result = AmondPlugin.GetInstance().EndWatchingAd();
        if (result)
        {
            textResult.text = "4. EndWatchingAd: Success";
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
    public void StartGame()
    {
        bool result = AmondPlugin.GetInstance().StartGame();
        if (result)
        {
            textResult.text = "5. StartGame: Success";
            buttonStartGame.SetActive(false);
            inputScore.SetActive(true);
            buttonEndGame.SetActive(true);
            _inputScoreField = inputScore.GetComponent<InputField>();
            _inputScoreField.Select();
        }
        else
        {
            textResult.text = "Failed StartGame()";
        }
    }

    /// <summary>
    /// 6. EndGame
    /// </summary>
    public void EndGame()
    {
        if (_inputScoreField.text == "") _inputScoreField.text = "1004";
        int score = int.Parse(_inputScoreField.text);

        GameScoreDto result = AmondPlugin.GetInstance().EndGame(score);
        if (result != null)
        {
            textResult.text = "6. Rank: " + result.rank +
                              "\n Best score: " + result.score;
            inputScore.SetActive(false);
            buttonEndGame.SetActive(false);

            buttonStartWatchAd.SetActive(true);
        }
        else
        {
            textResult.text = "Failed EndGame()";
        }
    }

    /// <summary>
    /// 7. OpenLeaderBoard
    /// </summary>
    public void OpenLeaderBoard()
    {
        AmondPlugin.GetInstance().OpenLeaderBoard();
    }
}