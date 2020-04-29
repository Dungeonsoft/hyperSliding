using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlurControl : MonoBehaviour
{

    public Image thisBurImg;
    public string matPropertyName;
    float spendTime;
    public float delayTime = 2f;
    public float defaultBlurValue = 10;
    float blurValue;

    Action uAction;

    public GameObject bg;

    private void OnEnable()
    {
        Debug.Log("Blur Control Start!");
        spendTime = delayTime;
        blurValue = defaultBlurValue;
        Debug.Log("blurValue :   " + blurValue);
        thisBurImg.material.SetFloat(matPropertyName, blurValue);

        uAction = BlurCon1;

        // 처음 시작할 때는 BG를 감춰준다.
        // BG를 감추는 것은 속도가 느려지기 때문.
        bg.SetActive(false);


    }

    /// <summary>
    /// 레디 셋 고가 끝나면 비지를 보여준다.
    /// </summary>
    public void ShowBg()
    {
        bg.SetActive(true);
    }

    void BlurCon1()
    {
        if (spendTime > 0)
            spendTime -= Time.deltaTime;
        else
        {
            uAction = BlurCon2;
        }

    }
    void BlurCon2()
    {
        //Debug.Log("blurValue1 :   " + blurValue);
        blurValue -= Time.deltaTime * defaultBlurValue;
        //Debug.Log("blurValue2 :   " + blurValue);
        //Debug.Log("blurValue3 :   ");

        thisBurImg.material.SetFloat(matPropertyName, blurValue);
        if (blurValue < 0)
            uAction = null;
    }


    public void Update()
    {
        if (uAction != null)
        {
            uAction();
        }
    }
}

