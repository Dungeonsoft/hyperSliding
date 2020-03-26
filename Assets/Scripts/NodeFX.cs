using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum nType {Normal,Move,CorrectPos,CorrectLine};




public class NodeFX : MonoBehaviour
{

    public nType SetType = nType.Normal; 

    public Transform nodeObj;
    Material nodeMat;
    public Color crashClr = new Color(1,1,1,1);
    public float strengthFxBase=1; 
    float strengthFx;
    public AnimationCurve ac;

    public Color emisColor;
    public Color emisColorMax;
    public Color emisColorBase;
    public float emisPower;
    float emisPowerBase;
    public float emisPowerMax =10f;

    Action uAction;

    float manualNodeSpeed;

    float lerpTime;

    public bool isFX;

    public List<TouchFxCon> tfc = new List<TouchFxCon>();

    private void Awake()
    {
        nodeMat = nodeObj.GetComponent<Renderer>().material;
        nodeMat.SetFloat("_EmisPower", emisPower);
        emisPowerBase = emisPower;
        nodeMat.SetColor("_EmisColor", emisColorBase);
    }

    private void Start()
    {
        manualNodeSpeed = GameObject.Find("GameManager").GetComponent<GameManager>().manualNodeSpeed;
    }

    private void OnEnable()
    {
        /// 처음에 들어오면 이 것부터 한다.
        /// 타입의 기본은 항상 normal이다.
        SetType = nType.Normal;
        NodeType(SetType);
    }

    float strengthFxColor;
    public void ActionCrashFX(float v)
    {
        //touchFxCon().NodeMoving();
        strengthFx = (v+1.0f)* strengthFxBase;

        strengthFxColor = strengthFx / 4f;
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
        emisColor = Color.Lerp(emisColorBase, emisColorMax* strengthFxColor, ac.Evaluate(lerpTime));
        nodeMat.SetColor("_EmisColor",emisColor);
        nodeMat.SetFloat("_EmisPower", emisPower);

        if (lerpTime >= 1)
        {
            //Debug.Log("Crash Clear!!");
            lerpTime = 0.0f;
            nodeMat.SetFloat("_EmisPower", emisPowerBase);
            nodeMat.SetColor("_EmisColor", emisColorBase);
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

    Define getDefine;
    public void NodeType(nType nt)
    {

        NodeTypeDefine ntd= GameObject.FindObjectOfType<NodeTypeDefine>();

        Define getDefine = ntd.nDefine[(int)nt];
    }


    void ChangeNodeType(Define d)
    {
        // 여기서 노드 칼라의 형태를 변형해주는 작업을 한다.(베이스컬러, 라인컬러, 숫자컬러)
    }
}