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

    private void OnEnable()
    {
        Debug.Log("Blur Control Start!");
        spendTime = delayTime;
        blurValue = defaultBlurValue;
        Debug.Log("blurValue :   " + blurValue);
        thisBurImg.material.SetFloat(matPropertyName, blurValue);

        uAction = BlurCon1;

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

