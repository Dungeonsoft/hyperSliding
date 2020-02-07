﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class NodeY
{
    public List<Transform> nodeX = new List<Transform>();
}

public class GameManager : MonoBehaviour
{
    #region Variables
    public Vector2Int rMixV = new Vector2Int(1,2);
    public Transform puzzle;
    [SerializeField]
    public List<NodeY> blocks = new List<NodeY>();
    public NodeScript[] nScript = new NodeScript[25];

    float nodeSpeed = 100.0f;
    public float baseNodeSpeed = 100.0f;
    public float manualNodeSpeed = 5.0f;
    public float delaySpeed = 20.0f;

    public AnimationCurve ac;

    public Text timeDigit;
    float timer;

    int hideX, hideY;
    int ori1X, ori1Y;
    int ori2X, ori2Y;

    Transform hideNode;

    Transform selNode;



    int mixRnd;
    int mixCnt;

    List<Transform> mNodes;
    List<Vector3> mNodesPos;
    List<bool> isFX;

    Vector3 selNodePosition;
    Vector3 hideNodePosition;
    float lVal;

    bool isMixed;

    Vector2 clickedNode;

    Action uAction = null;
    Action uActionTimer = null;

    // 이펙트 발동 여부 체크//
    //bool isFX = false;
    #endregion


    private void Awake()
    {
        nodeSpeed = baseNodeSpeed;
        puzzle = transform.parent.Find("Puzzle");

        nScript = puzzle.GetComponentsInChildren<NodeScript>();

        //hideNode = puzzle.GetChild(Random.Range(0, 25));
        hideNode = puzzle.GetChild(24);
        hideX = hideNode.GetComponent<NodeScript>().poxNowX;
        hideY = hideNode.GetComponent<NodeScript>().poxNowY;

        hideNode.gameObject.SetActive(false);
    }

    private void Start()
    {

        mixRnd = Random.Range( rMixV.x, rMixV.y);
        Debug.Log("Random Mix Counter : " + mixRnd);

        timeDigit.text = "00:00:00";
        timer = 0;
        uAction = MixCount;
    }

    void MixCount()
    {
        //Debug.Log("Mix Count 01 = " + mixCnt);
        if(mixRnd>mixCnt)
        {
            mixCnt++;
            uAction = CalcurateMovableNode;
        }
        else
        {
            isMixed = true;
            nodeSpeed = manualNodeSpeed;
            Debug.Log("=====Now You can click nodes!=====");
            uAction = CheckClick;
            uActionTimer = CheckTime;
        }
    }

    void CalcurateMovableNode()
    {
        ori2X = ori1X;
        ori2Y = ori1Y;
        ori1X = hideX;
        ori1Y = hideY;

        Vector2[] posN = new Vector2[]
        {
            new Vector2(0,ori1Y),
            new Vector2(1,ori1Y),
            new Vector2(2,ori1Y),
            new Vector2(3,ori1Y),
            new Vector2(4,ori1Y),
            new Vector2(ori1X,0),
            new Vector2(ori1X,1),
            new Vector2(ori1X,2),
            new Vector2(ori1X,3),
            new Vector2(ori1X,4),
        };

        int r = 0; //랜덤 선택용 변수.

        if (isMixed != true)
        {
            bool wBool = true;
            while (wBool)
            {
                r = Random.Range(0, 10);

                int xx = (int)(posN[r].x);
                int yy = (int)(posN[r].y);

                hideX = xx;
                hideY = yy;

                if ((hideX == ori1X && hideY == ori1Y) || (hideX == ori2X && hideY == ori2Y))
                {
                    //Debug.Log("---------------------같은 위치!!!");
                    wBool = true;
                }
                else
                {
                    wBool = false;
                }
            }
        }
        else
        {
            //Debug.Log("Clicked Node!!! __ 01");
            for(int i =0; i< 10; i++)
            {
                if(posN[i] == clickedNode)
                {
                    //Debug.Log("Clicked Node!!! __ 02");
                    r = i;

                    int xx = (int)(posN[r].x);
                    int yy = (int)(posN[r].y);

                    hideX = xx;
                    hideY = yy;

                    break;
                }
            }
        }

        mNodes = new List<Transform>();
        if(r<5)
        {
            int interVal =Mathf.Abs( hideX - ori1X);
            if (hideX > ori1X)
            {
                for (var i = 0; i <= interVal; i++)
                {
                    mNodes.Add(AddNode(posN[r - i]));
                }
            }
            else
            {
                for (var i = 0; i <= interVal; i++)
                {
                    mNodes.Add(AddNode(posN[r + i]));
                }
            }
        }
        else
        {
            int interVal = Mathf.Abs(hideY - ori1Y);
            if (hideY > ori1Y)
            {
                for (var i = 0; i <= interVal; i++)
                {
                    mNodes.Add(AddNode(posN[r - i]));
                }
            }
            else
            {
                for (var i = 0; i <= interVal; i++)
                {
                    mNodes.Add(AddNode(posN[r + i]));
                }
            }
        }

        isFX = new List<bool>();

    mNodesPos = new List<Vector3>();
        foreach(var node in mNodes)
        {
            mNodesPos.Add(node.position);
            isFX.Add(node.GetComponent<NodeFX>().isFX);
        }


        #region 노드를 옮기고 노드의 현재 위치값을 교정하고 비어있는 노드의(hideNode) 현재 위치 값을 교정한다.
        selNodePosition = mNodes[0].position;
        hideNodePosition = hideNode.position;
        uAction = MovingNodes;
        #endregion
    }

    // 옮겨질 노드 찾는 메소드;
    Transform AddNode(Vector2 nPos)
    {
        Transform sNode = null;
        for (var i = 0; i < 25; i++)
        {
            NodeScript ns = nScript[i];
            if (nPos.x == ns.poxNowX && nPos.y == ns.poxNowY)
            {
                sNode = ns.transform;
                break;
            }
        }
        return sNode;
    }



    void MovingNodes()
    {
        int cnt = mNodes.Count - 1;
        lVal += Time.deltaTime * nodeSpeed;
        for (int i = 0; i < cnt; i++)
        {
            float v = ac.Evaluate(lVal - (delaySpeed * (cnt - i) * Time.deltaTime));
            mNodes[i].position = Vector3.Lerp(mNodesPos[i], mNodesPos[i + 1], v);

            //벽 또는 다른 노드와 부딪힐때 이펙트를 발동시키는 부분. 
            if (isMixed)
            {
                if (isFX[i] == false && (lVal - (delaySpeed * (cnt-i) * Time.deltaTime)) >= 0.7088259f)
                {
                    isFX[i] = true;
                    mNodes[i].GetComponent<NodeFX>().ActionCrashFX(i);
                    Debug.Log("Crash FX :: " + mNodes[i].name);
                }
            }
        }
        
        if (lVal >= 1.0f + (delaySpeed * (cnt-1) * Time.deltaTime))
        {
            for (int i = 0; i < cnt; i++)
            {
                mNodes[i].GetComponent<NodeScript>().poxNowX = mNodes[i + 1].GetComponent<NodeScript>().poxNowX;
                mNodes[i].GetComponent<NodeScript>().poxNowY = mNodes[i + 1].GetComponent<NodeScript>().poxNowY;

                mNodes[i].position = mNodesPos[i + 1];
            }

            lVal = 0.0f;

            hideNode.position = selNodePosition;
            hideNode.GetComponent<NodeScript>().poxNowX = hideX;
            hideNode.GetComponent<NodeScript>().poxNowY = hideY;

            // 최초 인게임 작동시 셔플이 다 되지 않았을때 오는 곳.
            if (isMixed != true)
            {
                uAction = MixCount;
            }
            // 셔플이 다 되면 모든 블럭이 이동후 이곳으로 온다.
            else
            {
                // 크래쉬 이펙트가 끝났으니 다시 크래쉬 이펙트를 사용할 수 있게 초기화한다.
                // 초기화 하는 방법은 NodeFX에 있는 isFX를 false로 바꾸는 것이다.
                for (int i = 0; i < cnt; i++)
                {
                    mNodes[i].GetComponent<NodeFX>().isFX = false;
                    //isFX[i] = false;
                }
                bool isCorrect = CheckCorrect();

                if(isCorrect == true)
                {
                    Debug.Log("YOU WIN!!!");
                    uActionTimer = null;
                    timeDigit.color = Color.yellow;
                }
                else
                {
                    //Debug.Log("NOT YET!!!");
                }

                nodeSpeed = manualNodeSpeed;

                uAction = CheckClick;
            }
        }
    }

    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit);

            Transform hitTransform = hit.transform;
            if(hitTransform.CompareTag("Node"))
            {
                string tn = hitTransform.name;
                //Debug.Log("Transform Name: "+tn);

                NodeScript hideN = hideNode.GetComponent<NodeScript>();
                NodeScript hitN = hitTransform.GetComponent<NodeScript>();

                clickedNode = new Vector2(hitN.poxNowX, hitN.poxNowY);

                if ((hideN.poxNowX == hitN.poxNowX) || (hideN.poxNowY == hitN.poxNowY))
                {
                    //Debug.Log("Movable!!");

                    uAction = CalcurateMovableNode;
                }
            }
        }
    }

    bool CheckCorrect()
    {
        //Debug.Log(nScript.Length);
        for (var i=0; i< nScript.Length; i++)
        {
            NodeScript s = nScript[i];
            if (s.oriPosX != s.poxNowX || s.oriPosY != s.poxNowY)
            {
                //Debug.Log(s.name);
                //Debug.Log("Corrected Node :: "+ i);
                return false;
            }
        }
        return true;
    }

    int h = 0;
    int m = 0;
    void CheckTime()
    {
        timer += Time.deltaTime;
        if (timer >= 60.0f)
        {
            m++; 
            timer -= 60.0f; ;
        }
        if(m>= 60)
        {
            h++;
            m -= 60;
        }
        timeDigit.text =h.ToString("00")+":"+ m.ToString("00") + ":"+timer.ToString();
    }

    private void Update()
    {
        if (uAction != null)
        {
            uAction();
        }

        if(uActionTimer != null)
        {
            uActionTimer();
        }
    }

}
