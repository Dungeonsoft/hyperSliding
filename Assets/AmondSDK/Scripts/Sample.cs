using AmondSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AmondSDK.AmondSDKPlugin;

public class Sample : MonoBehaviour
{
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject button4;
    public GameObject button5;

    public Text message;

    public void Init()
    {
        bool returnValue = AmondPlugin.GetInstance().Init("game-hyper-sliding", EnvironmentType.STAGE);

        if (returnValue)
        {
            message.text = "초기화 성공";
            button1.SetActive(false);
            button2.SetActive(true);
        }
        else
        {
            message.text = "초기화 실패";
        }
    }

    public void IsAvailable()
    {
        bool returnValue = AmondPlugin.GetInstance().IsAvailable();
        if (returnValue)
        {
            message.text = "사용가능";
            button2.SetActive(false);
            button3.SetActive(true);
        }
        else
        {
            message.text = "사용불능";
        }
    }
    

    public void IsAuthenticated()
    {
        bool returnValue = AmondPlugin.GetInstance().IsAuthenticated();

        if (returnValue)
        {
            message.text = "인증됨";
            button3.SetActive(false);
            button4.SetActive(true);
        }
        else
        {
            long ID = AmondPlugin.GetInstance().ConnectAmond();
            if(ID ==0)
            {
                message.text = "연결 못함";
            }
            else
            {
                message.text = "연결 됨";

                button3.SetActive(false);
                button4.SetActive(true);
            }

        }
    }


    public void GetAccessToken()
    {
        bool returnValue = AmondPlugin.GetInstance().GetAccessToken();

        if (returnValue)
        {
            message.text = "인증 토큰 있음";
            button4.SetActive(false);
            button5.SetActive(true);
        }
        else
        {
            message.text = "인증 토큰 없음";
        }

        Debug.Log(returnValue);
    }

    public void GetUser()
    {
        string returnValue = AmondPlugin.GetInstance().GetUser();
        if(returnValue == null)
        {
            message.text = "유저 정보 없음";
        }
        else
        {
            //JSON정보를 JSON String으로 변경
            message.text = JsonUtility.ToJson(AmondPlugin.GetInstance().GetUserData());
        }
    }
}
