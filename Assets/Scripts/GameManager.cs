using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    int hideX, hideY;
    int ori1X, ori1Y;
    int ori2X, ori2Y;

    Transform hideNode;

    Transform selNode;



    int mixRnd;
    int mixCnt;

    List<Transform> mNodes;
    List<Vector3> mNodesPos;

    Vector3 selNodePosition;
    Vector3 hideNodePosition;
    float lVal;

    bool isMixed;

    Vector2 clickedNode;

    Action uAction = null;
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

        mNodesPos = new List<Vector3>();
        foreach(var node in mNodes)
        {
            mNodesPos.Add(node.position);
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
            mNodes[i].position = Vector3.Lerp(mNodes[i].position, mNodesPos[i + 1], lVal-(delaySpeed*(cnt-i)*Time.deltaTime));
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

            if (isMixed != true)
            {
                uAction = MixCount;
            }
            else
            {
                bool isCorrect = CheckCorrect();

                if(isCorrect == true)
                {
                    Debug.Log("YOU WIN!!!");
                }
                else
                {
                    Debug.Log("NOT YET!!!");
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
        Debug.Log(nScript.Length);
        for (var i=0; i< nScript.Length; i++)
        {
            NodeScript s = nScript[i];
            if (s.oriPosX != s.poxNowX || s.oriPosY != s.poxNowY)
            {
                Debug.Log(s.name);
                Debug.Log("Corrected Node :: "+ i);
                return false;
            }
        }
        return true;
    }

    private void Update()
    {
        if (uAction != null)
        {
            uAction();
        }
    }

}
