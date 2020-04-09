using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ot = OpaqueType;

public enum OpaqueType {normal, reverse }
public class TouchFxCon : MonoBehaviour
{
    public float scaleRange = 1;
    public bool scaleReverse = false;

    public AnimationCurve acScale;

    AnimationCurve acOpaque;
    public AnimationCurve acOpaque01;
    public AnimationCurve acOpaque02;
    public AnimationCurve acReverse01;
    public Color baseColor;
    public float speedFX;
    public float emissivePower =1;
    public bool isUntilMoving;
    bool isNodeMoving;
    public OpaqueType oType = ot.normal;
    

    Transform thisTrans;
    Material thisMat;
    float timeStart;
    float timeNow;

    bool isMovale;

    Action uAction;

    GameManager gb;


    Transform h;


    //public NodeScript

    private void Awake()
    {
        thisTrans = this.transform;
        thisMat = GetComponent<Renderer>().material;
        this.gameObject.SetActive(false);
        gb = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void FxCon(Transform ht)
    {
        h = ht;
        //gameObject.SetActive(true);
        if (oType == ot.reverse)
        {
            //Debug.Log("터치 이펙트 함수 실행 2 :: 이름 :" + this.name + " 액티브 상태: " + gameObject.activeSelf);
        }
        timeStart = Time.time;
        speedFX = GetManualNodeSpeed();
        
        switch (oType)
        {
            case ot.normal:
                acOpaque = acOpaque01;
                break;

            case ot.reverse:
                acOpaque = acOpaque02;
                break;
        }
        uAction += StartFX01;
    }

    float GetManualNodeSpeed()
    {
        return gb.manualNodeSpeed;
    }

    public void MoveNodeCheck(bool b)
    {
        isMovale = b;
    }

    void StartFX01()
    {
        timeNow = Time.time - timeStart;
        var eVal = timeNow * speedFX;
        thisMat.SetFloat("_AlphaRange", acOpaque.Evaluate(eVal));



        if (isUntilMoving == false)
        {
            if (scaleReverse == false)
            {
                thisTrans.localScale = (Vector3.one * acScale.Evaluate(eVal) * scaleRange);
            }
            else
            {
                thisTrans.localScale = Vector3.one - Vector3.one * acScale.Evaluate(eVal) * scaleRange;
            }


            thisMat.SetFloat("_TexContrast", acOpaque.Evaluate(eVal));

            if (eVal >= 1)
            {
                //Debug.Log("End FX");
                uAction = EndFX01;
            }
        }
        else if(isNodeMoving == true && eVal >= 1)
        {
            timeStart = Time.time;
            acOpaque = acReverse01;
            uAction = EndFX02;

            thisTrans.position = new Vector3(h.position.x, 0.3f, h.position.z);
        }
    }


    public void ShowFxRefract(Transform h)
    {
        gb = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (this.transform.childCount > 0)
        {
            GameObject c = this.transform.GetChild(0).gameObject;
            c.SetActive(true);
            c.GetComponent<TouchFxCon>().ShowFxRefract(h);
        }
        this.transform.position = new Vector3(h.position.x, 0.3f, h.position.z);
        acOpaque = acOpaque01;
        timeStart = Time.time;
        speedFX = GetManualNodeSpeed();

        uAction = StartFX01;


        /*
        NodeScript ns = h.GetComponent<NodeScript>();
        if (ns.oriPosX == ns.poxNowX && ns.oriPosY == ns.poxNowY)
        {
            foreach (var v in gb.tFxKids01)
            {
                if (v.gameObject.activeSelf == false)
                {
                    Debug.Log("터치 이펙트01 함수 실행__3 :: 이름: " + v.name);
                    v.gameObject.SetActive(true);
                    v.GetComponent<TouchFxCon>().FxCon(h);
                    v.position = new Vector3(h.position.x, 0.3f, h.position.z);
                    break;
                }
            }
        }
        */
    }

    public void NodeMoving()
    {
        isNodeMoving = true;
    }
    public void NodeMovingNo()
    {
        isNodeMoving = true;
    }

    void EndFX01()
    {
        gameObject.SetActive(false);
        isNodeMoving = false;
        uAction = null;
    }

    /// <summary>
    /// 이곳에서 블럭을 이동함.
    /// eVal이 1을 넘을 경우 다 이동을 한 것이기에 멈춤.
    /// 여기서 블럭이 제자리에 가 있는지 확인하는 코드를 넣어야 됨.
    /// </summary>
    void EndFX02()
    {

        timeNow = Time.time - timeStart;
        var eVal = timeNow * speedFX;
        thisMat.SetFloat("_EmisPower", acOpaque01.Evaluate(eVal) * emissivePower);

        if (eVal >= 1)
        {
            gameObject.SetActive(false);
            isNodeMoving = false;
            uAction = null;
        }
    }


    /// <summary>
    /// 노드가 제위치에 들어갔는지 확인하는 메소드.
    /// </summary>
    void CheckCorrectPosition()
    {

    }

    public void PoseMoving(Transform ht)
    {
        h=ht;
        uAction += PoseMovingDel;
    }

    void PoseMovingDel()
    {
        thisTrans.position = new Vector3(h.position.x, 0.3f, h.position.z);
        //Debug.Log("thisTrans    : " + thisTrans.position);
        //Debug.Log("hitTransform : " + h.position);

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
