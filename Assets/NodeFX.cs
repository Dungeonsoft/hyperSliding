using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeFX : MonoBehaviour
{
    public Transform nodeObj;
    Material nodeMat;
    public Color crashClr = new Color(1,1,1,1);
    public float strengthFxBase=1; 
    float strengthFx;
    public AnimationCurve ac;
    
    Color emisColor;
    public float emisPower;
    float emisPowerBase;
    public float emisPowerMax =10f;


    Action uAction;

    float manualNodeSpeed;

    float lerpTime;

    public bool isFX;

    private void Awake()
    {
        nodeMat = nodeObj.GetComponent<Renderer>().material;
        emisColor = nodeMat.GetColor("_EmisColor");
        nodeMat.SetFloat("_EmisPower", emisPower);
        emisPowerBase = emisPower;
    }

    private void Start()
    {
        manualNodeSpeed = GameObject.Find("GameManager").GetComponent<GameManager>().manualNodeSpeed;
    }

    public void ActionCrashFX(float v)
    {
        strengthFx = (v+1.0f)* strengthFxBase;
        uAction += CrashFX;

        //RunDelayed(2f, () =>
        //{
        //    Debug.Log("Delayed!!");
        //});
    }



    Coroutine RunDelayed(float v, Action a)
    {
        return StartCoroutine(DelayedCoroutine(v, a));
    }

    IEnumerator DelayedCoroutine(float v, Action a)
    {
        yield return new WaitForSeconds(v);
        a();
    }

    void CrashFX()
    {
        emisPower = Mathf.Lerp(emisPowerBase, emisPowerBase * emisPowerMax* strengthFx, ac.Evaluate(lerpTime));
        nodeMat.SetFloat("_EmisPower", emisPower);

        if (lerpTime >= 1)
        {
            //Debug.Log("Crash Clear!!");
            lerpTime = 0.0f;
            nodeMat.SetFloat("_EmisPower", emisPowerBase);
            uAction -= CrashFX;
        }
        else
        {
            lerpTime += Time.deltaTime * 5.0f;
        }
    }



    private void Update()
    {
        if(uAction != null)
        {
            uAction();
        }
    }



}
