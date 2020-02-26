using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ot = OpaqueType;

public enum OpaqueType {normal, reverse }
public class TouchFxCon : MonoBehaviour
{

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
    bool isNoMoving;
    public OpaqueType oType = ot.normal;
    

    Transform thisTrans;
    Material thisMat;
    float timeStart;
    float timeNow;

    bool isMovale;

    Action uAction;

    GameManager gb;

    private void Awake()
    {
        thisTrans = this.transform;
        thisMat = GetComponent<Renderer>().material;
        this.gameObject.SetActive(false);
        gb = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void FxCon()
    {
        gameObject.SetActive(true);
        if (oType == ot.reverse)
        {
            Debug.Log("터치 이펙트 함수 실행 2 :: 이름 :" + this.name + " 액티브 상태: " + gameObject.activeSelf);
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
        thisMat.SetFloat("_EmisPower", acOpaque.Evaluate(eVal)* emissivePower);



        if (isUntilMoving == false)
        {
            thisTrans.localScale = Vector3.one * acScale.Evaluate(eVal);

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

            #region 파동형 이펙트 후처리로 삽입
            Debug.Log("파동형 이펙트");
            if (isNoMoving == false)
            {
                foreach (var v in gb.tFxKids01)
                {
                    if (v.gameObject.activeSelf == false)
                    {
                        //Debug.Log("터치 이펙트01 함수 실행__3 :: 이름: " + v.name);
                        //v.gameObject.SetActive(true);
                        v.GetComponent<TouchFxCon>().FxCon();
                        v.position = new Vector3(h.position.x, 0.3f, h.position.z);

                        //이펙트를 부모 역할을 하는 노드에 변수로 넣어 놓는다.
                        //hitTransform.GetComponent<NodeFX>().tfc.Add(v.GetComponent<TouchFxCon>());

                        break;
                    }
                }
            }
            #endregion

        }
    }

    public void NodeMoving()
    {
        isNodeMoving = true;
    }
    public void NodeMovingNo()
    {
        isNodeMoving = true;
        isNoMoving = true;
    }

    void EndFX01()
    {
        gameObject.SetActive(false);
        isNodeMoving = false;
        isNoMoving = false;
        uAction = null;
    }

    void EndFX02()
    {

        timeNow = Time.time - timeStart;
        var eVal = timeNow * speedFX;
        thisMat.SetFloat("_EmisPower", acOpaque01.Evaluate(eVal) * emissivePower);

        if (eVal >= 1)
        {
            gameObject.SetActive(false);
            isNodeMoving = false;
            isNoMoving = false;
            uAction = null;
        }
    }

    Transform h;
    public void PoseMoving(Transform ht)
    {
        h=ht;
        uAction += PoseMovingDel;
    }

    void PoseMovingDel()
    {
        thisTrans.position = new Vector3(h.position.x, 0.3f, h.position.z);
        Debug.Log("thisTrans    : " + thisTrans.position);
        Debug.Log("hitTransform : " + h.position);

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
