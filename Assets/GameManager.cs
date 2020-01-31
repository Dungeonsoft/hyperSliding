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
    public Transform puzzle;
    [SerializeField]
    public List<NodeY> blocks = new List<NodeY>();
    public NodeScript[] nScript = new NodeScript[25];

    int hideX, hideY;
    int ori1X, ori1Y;
    int ori2X, ori2Y;

    Transform hideNode;

    Transform selNode;



    int mixRnd;
    int mixCnt;

    Vector3 selNodePosition;
    Vector3 hideNodePosition;
    float lVal;

    Action uAction = null;

    private void Awake()
    {
        puzzle = transform.parent.Find("Puzzle");

        nScript = puzzle.GetComponentsInChildren<NodeScript>();

        hideNode = puzzle.GetChild(Random.Range(0, 25));
        hideX = hideNode.GetComponent<NodeScript>().poxNowX;
        hideY = hideNode.GetComponent<NodeScript>().poxNowY;

        hideNode.gameObject.SetActive(false);
    }



    private void Start()
    {

        mixRnd = Random.Range(150, 200);
        Debug.Log("Random Mix Counter : " + mixRnd);


        uAction = MixCount;
    }

    void MixCount()
    {
        Debug.Log("Mix Count 01 = " + mixCnt);
        if(mixRnd>mixCnt)
        {
            mixCnt++;
            uAction = CalcurateMovableNode;
        }
        else
        {
            uAction = null;
        }
    }

    void CalcurateMovableNode()
    {
        ori2X = ori1X;
        ori2Y = ori1Y;
        ori1X = hideX;
        ori1Y = hideY;

        Vector2[] pos = new Vector2[]
        {
            new Vector2(ori1X-1,ori1Y),
            new Vector2(ori1X+1,ori1Y),
            new Vector2(ori1X,ori1Y-1),
            new Vector2(ori1X,ori1Y+1),
        };

        bool wBool = true;
        while (wBool)
        {
            int r = Random.Range(0, 4);

            int xx = (int)(pos[r].x);
            int yy = (int)(pos[r].y);


            hideX = (xx > 4 || xx < 0) ? ori1X : xx;
            hideY = (yy > 4 || yy < 0) ? ori1Y : yy;

            if((hideX == ori1X && hideY == ori1Y) || (hideX == ori2X && hideY == ori2Y))
            {
                Debug.Log("---------------------같은 위치!!!");
                wBool = true;
            }
            else
            {
                wBool = false;
            }
        }
        #region  옮겨질 노드 찾기
        for (var i = 0; i < 25; i++)
        {
            //Debug.Log("I Value: " + i);
            NodeScript ns = nScript[i];
            //Debug.Log("pX: " + ns.poxNowX + " __ pY: " + ns.poxNowY);
            //Debug.Log("hX: " + hideX + " __ hY: " + hideY);
            if (hideX == ns.poxNowX && hideY == ns.poxNowY)
            {
                Debug.Log("_______________Found Block!!!!");
                Debug.Log("Sel Node Pos: " + ns.poxNowX + " :: " + ns.poxNowY);
                Debug.Log("Hide Node Pos: " + ori1X + " :: " + ori1Y);
                selNode = ns.transform;
                break;
            }
        }
        #endregion

        #region 노드를 옮기고 노드의 현재 위치값을 교정하고 비어있는 노드의(hideNode) 현재 위치 값을 교정한다.
        selNodePosition = selNode.position;
        hideNodePosition = hideNode.position;
        uAction = MovingNodes;
        #endregion
    }

    void MovingNodes()
    {
        lVal += Time.deltaTime * 20.0f;
        selNode.position = Vector3.Lerp(selNodePosition, hideNodePosition, lVal);
        hideNode.position = Vector3.Lerp(hideNodePosition, selNodePosition, lVal);

        if (lVal >= 1.0f)
        {
            selNode.GetComponent<NodeScript>().poxNowX = ori1X;
            selNode.GetComponent<NodeScript>().poxNowY = ori1Y;
            hideNode.GetComponent<NodeScript>().poxNowX = hideX;
            hideNode.GetComponent<NodeScript>().poxNowY = hideY;

            lVal = 0;
            uAction = MixCount;
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
