using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public NodeTypeDefine ntd;
    Define getDefine;

    public Transform digitBase;
    public Transform lineGlow;
    public Transform line;
    public Transform digit;

    bool isMoveFx = false;

    // 최초 무브를 했는지 체크하는 불린 변수.
    public bool isFirstMove = false;



    private void Awake()
    {
        //nodeMat = nodeObj.GetComponent<Renderer>().material;
        //nodeMat.SetFloat("_EmisPower", emisPower);
        //emisPowerBase = emisPower;
        //nodeMat.SetColor("_EmisColor", emisColorBase);

        SetChild();
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
        isFirstMove = false;
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
        isFirstMove = true;
        //emisPower = Mathf.Lerp(emisPowerBase, emisPowerBase * emisPowerMax* strengthFx, ac.Evaluate(lerpTime));
        //emisColor = Color.Lerp(emisColorBase, emisColorMax* strengthFxColor, ac.Evaluate(lerpTime));
        //nodeMat.SetColor("_EmisColor",emisColor);
        //nodeMat.SetFloat("_EmisPower", emisPower);

        if (isMoveFx == false)
        {
            isMoveFx = true;
            NodeType(nType.Move);
        }
        if (lerpTime >= 1)
        {
            //Debug.Log("Crash Clear!!");
            lerpTime = 0.0f;
            //nodeMat.SetFloat("_EmisPower", emisPowerBase);
            //nodeMat.SetColor("_EmisColor", emisColorBase);
            isMoveFx = false;
            uAction -= CrashFX;
            
            //NodeType(nType.Normal);
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

    public void NodeType(nType nt)
    {

        if (isFirstMove == false) return;
        ntd= GameObject.FindObjectOfType<NodeTypeDefine>();

        //Debug.Log("NT"+name+ "Number: "+ nt.ToString());
        getDefine = ntd.nDefine[(int)nt];
        
        
        ChangeNodeType(getDefine);
    }

    public void NodeToNormal(nType nt)
    {
        ntd = GameObject.FindObjectOfType<NodeTypeDefine>();

        //Debug.Log("NodeToNormal :: " + name + "Number: " + nt.ToString());
        getDefine = ntd.nDefine[(int)nt];
        ChangeNodeType(getDefine);
    }


    /// <summary>
    /// 기 지정된 노트를 형태에 관련된 정보를 가지고 와서 각각의 노드의 상태에 맞게 적용한다.
    /// </summary>
    /// <param name="d"></param>
    void ChangeNodeType(Define d)
    {
        // 여기서 노드 칼라의 형태를 변형해주는 작업을 한다.(베이스컬러, 라인컬러, 숫자컬러)

        //Debug.Log("d.digitColor:: " + d.digitColor);
        //Debug.Log("d.lineColor:: " + d.lineColor);
        //Debug.Log("d.lineGlowColor:: " + d.lineGlowColor);
        //Debug.Log("d.baseColor:: " + d.baseColor);
        if (digit == null) SetChild();


        digit.GetComponent<Image>().color = d.digitColor;
        line.GetComponent<Image>().color = d.lineColor;
        lineGlow.GetComponent<Image>().color = d.lineGlowColor;
        digitBase.GetComponent<Image>().color = d.baseColor;


        //digit.GetComponent<Renderer>().material.SetColor("_TintColor", d.digitColor);
        //line.GetComponent<Renderer>().material.SetColor("_TintColor", d.lineColor);
        //lineGlow.GetComponent<Renderer>().material.SetColor("_TintColor", d.lineGlowColor);
        //digitBase.GetComponent<Renderer>().material.SetColor("_TintColor", d.baseColor);
        
    }

    void SetChild()
    {
        var thisT = this.transform;
        digit = thisT.Find("Block");
        line = thisT.Find("Line");
        lineGlow = thisT.Find("Line_Glow");
        digitBase = thisT.Find("Base");
    }
}