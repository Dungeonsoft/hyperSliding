using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchFxCon : MonoBehaviour
{

    public AnimationCurve acScale;
    public AnimationCurve acOpaque;
    public Color baseColor;
    public float speedFX;
    public float emissivePower =1;
    
    Transform thisTrans;
    Material thisMat;
    float timeStart;
    float timeNow;

    bool isMovale;

    Action uAction;

    private void Awake()
    {
        thisTrans = this.transform;
        thisMat = GetComponent<Renderer>().material;
        this.gameObject.SetActive(false);
    }

    public void FxCon()
    {
        Debug.Log("터치 이펙트 함수 실행__4 :: 이름 :"+this.name+" 액티브 상태: "+gameObject.activeSelf);
        timeStart = Time.time;
        uAction = StartFX;
    }

    public void MoveNodeCheck(bool b)
    {
        isMovale = b;
    }

    void StartFX()
    {
        timeNow = Time.time - timeStart;
        var eVal = timeNow * speedFX;
        thisMat.SetFloat("_EmisPower", acOpaque.Evaluate(eVal)* emissivePower);
        thisTrans.localScale = Vector3.one * acScale.Evaluate(eVal);


        if(eVal>=1)
        {
            Debug.Log("End FX");
            uAction = EndFX;
        }
    }

    void EndFX()
    {
        uAction = null;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(uAction != null)
        {
            uAction();
        }
    }
}
