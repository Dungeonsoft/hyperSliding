using System;
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


    /// <summary>
    /// 터치 이펙트의 부모 폴더 지정.
    /// </summary>
    public Transform tFX01;
    public Transform tFX02;

    /// <summary>
    /// 터치 이펙트들을 미리 모아놓는 폴더 역할 리스트 배열.
    /// </summary>
    List<Transform> tFxKids01 = new List<Transform>();

    /// <summary>
    /// 터치가 발생하면 터치 이펙트를 모아놓는 폴더역할 리스트 배열.
    /// </summary>
    List<Transform> tFxKids02 = new List<Transform>();

    public AnimationCurve ac;

    public Text timeDigit;
    float timer;

    int hideX, hideY;
    int ori1X, ori1Y;
    int ori2X, ori2Y;

    /// <summary>
    /// 숨겨진 노드는 일반적으로 마지막 번호인 25번이다.
    /// </summary>
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

    List<Vector2> clickedNode = new List<Vector2>();

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


        //tFX.SetActive(false);

        //여기서 미리 터치 에펙트를 소환하기(찾기) 좋게 리스트 배열로 묶어둠
        for (int i = 0; i < tFX01.childCount; i++)
        {
            //터치 이펙트는 처음에 보이면 안되니 처음 시작시 무조건 감추어 놓는다.
            tFxKids01.Add(tFX01.GetChild(i));
            tFX01.GetChild(i).gameObject.SetActive(false);
        }
        //여기서 미리 터치 에펙트를 소환하기(찾기) 좋게 리스트 배열로 묶어둠
        for (int i = 0; i < tFX02.childCount; i++)
        {
            //터치 이펙트는 처음에 보이면 안되니 처음 시작시 무조건 감추어 놓는다.
            tFxKids02.Add(tFX02.GetChild(i));
            tFX02.GetChild(i).gameObject.SetActive(false);
        }



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
            
            // CheckClick이 최초로 들어가는 부분 //  그러니 다른 함수가 델리게이트에 있으면 안되는 그냥 단독으로 넣어준다.
            uAction = CheckClick;
            uActionTimer = CheckTime;
        }
    }

    void CalcurateMovableNode()
    {
        //Debug.Log("움직일 노드를 찾는 계산을 함");
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


        // 처음에 섞을때.
        #region
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
        #endregion

        //이미 섞고 인게임중에.
        #region
        else
        {
            //Debug.Log("Clicked Node!!! __ 01 :: clickedNode.Count: "+ clickedNode.Count);
            for (int i = 0; i < 10; i++)
            {
                if (clickedNode.Count>0 && posN[i] == clickedNode[0])
                {
                    //Debug.Log("움직일 수 있는 노트 선택!");
                    //Debug.Log("Clicked Node!!! __ 02");
                    r = i;

                    int xx = (int)(posN[r].x);
                    int yy = (int)(posN[r].y);

                    hideX = xx;
                    hideY = yy;

                    clickedNode.RemoveAt(0);
                    break;
                }
            }
        }
        #endregion

        mNodes = new List<Transform>();
        if (r < 5)
        {
            int interVal = Mathf.Abs(hideX - ori1X);
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
        foreach (var node in mNodes)
        {
            mNodesPos.Add(node.position);
            isFX.Add(node.GetComponent<NodeFX>().isFX);
        }


        #region 노드를 옮기고 노드의 현재 위치값을 교정하고 비어있는 노드의(hideNode) 현재 위치 값을 교정한다.

        if (isMixed)
        {
            List<TouchFxCon> htTfc = new List<TouchFxCon>();
            Debug.Log("HT Name: " + mNodes[0].name +" :: "+ mNodes[0].GetComponent<NodeFX>().tfc.Count);
            htTfc = mNodes[0].GetComponent<NodeFX>().tfc;

            foreach (var v in htTfc)
            {
                v.NodeMoving();
            }
            mNodes[0].GetComponent<NodeFX>().tfc = new List<TouchFxCon>();
        }

        selNodePosition = mNodes[0].position;
        hideNodePosition = hideNode.position;
        uAction += MovingNodes;
        uAction -= CalcurateMovableNode;
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

    bool isMovingNode = false;

    void MovingNodes()
    {
        if (isMovingNode == false)
        {
            //Debug.Log("MovingNodes");
            isMovingNode = true;
        }

        int cnt = mNodes.Count - 1;
        //Debug.Log("mNodes.Count: "+ mNodes.Count);
        lVal += Time.deltaTime * nodeSpeed;
        for (int i = 0; i < cnt; i++)
        {
            float v = ac.Evaluate(lVal - (delaySpeed * (cnt - i) * Time.deltaTime));
            //if(i == 0)
            //{
            //    Debug.Log("V Value: " + v);
            //}
            mNodes[i].position = Vector3.Lerp(mNodesPos[i], mNodesPos[i + 1], v);

            //벽 또는 다른 노드와 부딪힐때 이펙트를 발동시키는 부분. 
            if (isMixed)
            {
                if (isFX[i] == false && (lVal - (delaySpeed * (cnt-i) * Time.deltaTime)) >= 0.7088259f)
                {
                    isFX[i] = true;
                    mNodes[i].GetComponent<NodeFX>().ActionCrashFX(i);
                    //Debug.Log("Crash FX :: " + mNodes[i].name);
                }
            }
        }
        
        if (lVal >= 1.0f + (delaySpeed * (cnt-1) * Time.deltaTime))
        {
            //if (isMixed) Debug.Log("Lerp 끝난후 노드 위치 정리 : mNodes.Count : " + mNodes.Count);
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
                }
                bool isCorrect = CheckCorrect();

                if (isCorrect == true)
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
                //Debug.Log("터치후 블럭이동 완료");
                //위치가 다 옮겨진 노드 정보는 삭제한다.
                //if (clickedNode.Count > 0)
                //    clickedNode.RemoveAt(0);

                //Debug.Log("dTouch.RemoveAt(0)");
                if (dTouch.Count > 0)
                    dTouch.RemoveAt(0);

                isMovingNode = false;
                if (dTouch.Count == 0)
                {
                    uAction -= CalcurateMovableNode;
                    uAction -= MovingNodes;
                }
                else
                {

                    uAction -= CalcurateMovableNode;

                    TouchAction(dTouch[0].ht, dTouch[0].m);
                    uAction -= MovingNodes;
                }
            }
        }
    }

    [System.Serializable]
    public class DelayTouch
    {
        public Transform ht;
        public bool m;
    }

   //Action DelayTouch;
    public List<DelayTouch> dTouch = new List<DelayTouch>();
    Transform hitTransform;
    // 터치를 하면 노드를 터치했는지 감지한다.
    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("터치 함");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit);

            hitTransform = hit.transform;
            if (hitTransform.CompareTag("Node"))
            {

                //터치 이펙트 함수 실행.
                //Debug.Log("터치 이펙트 함수 실행__1");


                bool isMovable = false;

                if (dTouch.Count > 0)
                {
                    Debug.Log("두개이상");
                    TouchFX();
                    Debug.Log("등록이 되었나 : " + hitTransform.name + " :: " + hitTransform.GetComponent<NodeFX>().tfc.Count);
                    dTouch.Add(new DelayTouch { ht = hitTransform, m = isMovable });
                }
                else
                {
                    Debug.Log("하나");
                    TouchFX();
                    Debug.Log("등록이 되었나 : "+ hitTransform.name+" :: "+hitTransform.GetComponent<NodeFX>().tfc.Count);
                    dTouch.Add(new DelayTouch { ht = hitTransform, m = isMovable });
                    TouchAction(hitTransform, isMovable);
                }
            }
        }
    }

    void TouchAction(Transform ht, bool isMovable)
    {


        string tn = ht.name;
        //Debug.Log("Transform Name: "+tn);

        NodeScript hideN = hideNode.GetComponent<NodeScript>();
        NodeScript hitN = ht.GetComponent<NodeScript>();

        // 이곳에 클릭한 노드들의 값을 저장.
        //clickedNode.Add(new Vector2(hitN.poxNowX, hitN.poxNowY));

        //Debug.Log("25번 노드 위치: " + hideN.poxNowX + " -- " + hideN.poxNowY);
        //Debug.Log("터치 노드 위치: " + hitN.poxNowX + " -- " + hitN.poxNowY);
        if ((hideN.poxNowX == hitN.poxNowX) || (hideN.poxNowY == hitN.poxNowY))
        {
            //Debug.Log("Movable!!");

            // 이곳에 클릭한 노드들의 값을 저장.
            clickedNode.Add(new Vector2(hitN.poxNowX, hitN.poxNowY));

            //RunDelayed(2f, () =>
            //{
            //    Debug.Log("Delayed!!");
            //});

            // 아직 아무 동작도 하지 않을 경우(움직이는 노드가 없는 상태일경우)
            //uAction += CalcurateMovableNode;

            //if (clickedNode.Count == 1) uAction += CalcurateMovableNode;

            uAction += CalcurateMovableNode;
            // 터치 이펙트 발동과 노드 이동 이펙트도 발동 할수 있도록 true로 변경한다.
            isMovable = true;
        }
        else
        {
            // 잘못된 터치로 이동이 불가한 부분에서는 밝게 해주는 이펙트를 중지시킨다.
            List<TouchFxCon> cc = ht.GetComponent<NodeFX>().tfc;
            foreach (var v in cc)
            {
                v.NodeMoving();
            }


            // 잘못된 터치(이동불가) 일때는 터치 정보를 바로 삭제한다.
            dTouch.RemoveAt(0);

            // 삭제 후 미리 들어와 있는 터치 정보가 있으면 다음 터치액션을 실행한다.
            if (dTouch.Count > 0)
                TouchAction(dTouch[0].ht, dTouch[0].m);
            // 멀티 터치는 구성 완료.
            // 멀티 터치 중 중간에 잘못 눌렀을 경우 어떻게 처리를 할 것인지
            // 논의가 필요함. 멈추게 할 것인지 아니면 그 다음 누른 것을 처리하게 할 것인지.
        }
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

    void TouchFX()
    {
        //Debug.Log("터치 이펙트 함수 실행__2");

        Vector3 pos = hitTransform.position;

        // 파동형 이펙트.
        foreach (var v in tFxKids01)
        {
            if (v.gameObject.activeSelf == false)
            {
                //Debug.Log("터치 이펙트01 함수 실행__3 :: 이름: " + v.name);
                v.gameObject.SetActive(true);
                v.GetComponent<TouchFxCon>().FxCon();
                v.position = new Vector3(pos.x, 0.3f, pos.z);

                //이펙트를 부모 역할을 하는 노드에 변수로 넣어 놓는다.
                hitTransform.GetComponent<NodeFX>().tfc.Add(v.GetComponent<TouchFxCon>());

                break;
            }
        }

        // 점멸형 이펙트.
        foreach (var v in tFxKids02)
        {
            if (v.gameObject.activeSelf == false)
            {
                //Debug.Log("터치 이펙트02 함수 실행__3 :: 이름: " + v.name);
                v.gameObject.SetActive(true);
                v.GetComponent<TouchFxCon>().FxCon();
                v.position = new Vector3(pos.x, 0.3f, pos.z);

                //이펙트를 부모 역할을 하는 노드에 변수로 넣어 놓는다.
                hitTransform.GetComponent<NodeFX>().tfc.Add(v.GetComponent<TouchFxCon>());
                break;
            }
        }
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
