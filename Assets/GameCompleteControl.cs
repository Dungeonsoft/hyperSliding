using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCompleteControl : MonoBehaviour
{

    public CanvasGroup cg;
    public GameObject scoreBoard;
    public AnimationCurve ac;
    public float alphaSpeed =0.3f;
    float spendTime = 0;
    Action uAction;
    private void OnEnable()
    {
        spendTime = 0;
        uAction = ShowCavas;
        scoreBoard.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        uAction?.Invoke();
    }

    void ShowCavas()
    {
        cg.alpha = ac.Evaluate(spendTime);

        if(spendTime>=1.0f)
        {
            Debug.Log("스코어보드 보이기");
            spendTime = 0.0f;
            scoreBoard.SetActive(true);
            uAction = null;
        }
        spendTime += Time.deltaTime*alphaSpeed;
    }
}
