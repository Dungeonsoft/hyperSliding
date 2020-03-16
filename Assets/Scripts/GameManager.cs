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

[System.Serializable]
public class DelayTouch
{
    public Transform ht;
    public bool m;
}

[System.Serializable]
public class SpeedList
{
    public float manualNodeSpeed;
    public float delaySpeed;
    public int speedScoreBonus;
    public float decreaseTime;
}

public class GameManager : MonoBehaviour
{
    public List<SpeedList> speedList;
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
    public List<Transform> tFxKids01 = new List<Transform>();

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


    /// <summary>
    /// 플레이 기본시간을 정의한다. 기준은 초단위.
    /// </summary>
    public int defaultTime = 300;
    public int remainingTime;
    
    /// <summary>
    /// 시간을 표시하는 유아이를 연결한다.
    /// </summary>
    public Text timeUI;

    /// <summary>
    /// 라인점수 계산을 이미 했는지 기록하는 어레이변수.
    /// </summary>
    bool[] LineMatching = { false, false, false, false, false };


    /// <summary>
    /// 남은 시간을 보여주는 기능에 필요한 변수(분)
    /// </summary>
    int min;
    /// <summary>
    /// 남은 시간을 보여주는 기능에 필요한 변수(초)
    /// </summary>
    int sec;


    /// <summary>
    /// 게임 완료시 초당 주어지는 가산점수.
    /// </summary>
    public int perSecScore = 100;

    /// <summary>
    /// 스코어를 저장하는 변수.
    /// </summary>
    int score = 0;

    /// <summary>
    /// 스코어를 텍스트롤 변환하여 화면에 보여주는 유아이.
    /// </summary>
    public Text scoreUI;

    /// <summary>
    /// 실제 남은 시간을 나타내주기 위해 만든 int 변수
    /// </summary>
    int spendTime;

    //Action DelayTouch;
    public List<DelayTouch> dTouch = new List<DelayTouch>();
    Transform hitTransform;

    public Text speedDigit;

    public float speedStepTimeDefault;
    float speedLevelTime;

    int speedLevel = 0;

    public int comboLevelBase = 0;
    public int comboLevel = 1;
    public int comboMaxMoving = 30;
    public int ComboMovingCount = 0;
    public bool isComboLvDn = false;

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


    /// <summary>
    /// 게임의 스피드를 세팅한다.
    /// </summary>
    /// <param name="step"></param>
    void SpeedSetting(int step)
    {
        Debug.Log("Speed : " + step);

        manualNodeSpeed = speedList[step].manualNodeSpeed;
        delaySpeed = speedList[step].delaySpeed;

        speedDigit.text = "X" + (step + 1);

        speedLevelTime = speedList[step].decreaseTime;
        Debug.Log("Speed : " + step+ "::: Speed Level Time : "+ speedLevelTime);
    }

    private void OnEnable()
    {
        mixRnd = Random.Range( rMixV.x, rMixV.y);

        // 점수 준 것을 체크하는 불린 변수들을 모두 초기화 한다.
        #region 불린 변수 초기화.
        foreach (var n in nScript)
        {
            n.alreadyScoreCount = false;
        }
        LineMatching = new bool[] { false, false, false, false, false };
        #endregion

        remainingTime = defaultTime;

        // 시간의 효과(더하거나 빼는 것) 있으면 이 곳에 기술하여 처음에 더하게 해준다.

        TimeAdd();

        TimeCal();
        timeDigit.text = "00:00:00";
        timer = 0;
        uAction = MixCount;

        speedLevel = 0;

        comboLevel = comboLevelBase;

        SpeedSetting(speedLevel);

    }


    /// <summary>
    /// 플레이 시간이 더해지거나 빠졌을 때 계산
    /// </summary>
    /// <param name="at">더해지거나 빠지는 시간(초)를 입력</param>
    void TimeAdd(int at =0)
    {
        remainingTime += at;
    }

    bool isSpeedLvlDn = false;
    /// <summary>
    /// 유아이 상에 남은 시간을 보여주기 위한 메소드
    /// </summary>
    void TimeCal()
    {
        speedLevelTime -= Time.deltaTime;

        if(speedLevelTime<= 0 && speedLevel > 0)
        {
            isSpeedLvlDn = true;
             speedLevel -= 1;
            SpeedSetting(speedLevel);
        }
        spendTime = remainingTime - Mathf.RoundToInt(timer);
        min = spendTime / 60;
        sec = spendTime % 60;
        timeUI.text = min.ToString("00") + ":" + sec.ToString("00");
    }

    void MixCount()
    {
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

    //bool isMovingNode = false;

    void MovingNodes()
    {
        //if (isMovingNode == false)
        //{
        //    //Debug.Log("MovingNodes");
        //    isMovingNode = true;
        //}

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
                var movedNode = mNodes[i].GetComponent<NodeScript>();
                //Debug.Log("노드 이름 : "+i+" : "+ mNodes[i].name);
                movedNode.poxNowX = mNodes[i + 1].GetComponent<NodeScript>().poxNowX;
                movedNode.poxNowY = mNodes[i + 1].GetComponent<NodeScript>().poxNowY;

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
            // 점수 계산도 이곳에서 이루어진다.
            else
            {
                #region 점수계산
                //노드 하나하나의 위치가 맞을 때 점수를 계산하기 위한 for문.
                for (int i = 0; i < cnt; i++)
                {
                    var movedNode = mNodes[i].GetComponent<NodeScript>();
                    // 각각 노드 점수계산하는 메소드 호출//
                    CalScore(movedNode);
                }

                //줄이 맞았을때 점수를 계산하는 메소드 호출.
                CalLineScore();
                #endregion

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

                    Debug.Log("Score: " + score);
                    Debug.Log("Remain Time Bonus: " + spendTime * perSecScore);
                    // 남은 시간을 기준으로 추가 점수(초당00점)를 지급한다.
                    AddScore(spendTime * perSecScore);

                    Debug.Log("Score: "+score);
                    Debug.Log("Speed Level: " + speedLevel);
                    Debug.Log("Add Score Percent: " + (speedList[speedLevel].speedScoreBonus * 0.01f));
                    Debug.Log("Speed Bonus: " + Mathf.RoundToInt(score * (speedList[speedLevel].speedScoreBonus * 0.01f)));

                    // 남은 시간을 최종 스피드 레벨에 따른 보너스를 지급한다.
                    AddScore(Mathf.RoundToInt(score * (speedList[speedLevel].speedScoreBonus * 0.01f)));
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

                //isMovingNode = false;
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

    /// <summary>
    /// 점수계산하는 메소드 인자는 노드에 붙어있는 노드 스크립트이며 여기서 정보를 호출하여 계산한다.
    /// </summary>
    /// <param name="ns"></param>
    void CalScore(NodeScript ns)
    {
        if (ns.oriPosX == ns.poxNowX && ns.oriPosY == ns.poxNowY && ns.alreadyScoreCount == false)
        {
            ns.alreadyScoreCount = true;
            score += 10 * comboLevel;

            // 다음번부터 콤보 보너스를 적용하기 위해 콤보레벨을 올려준다.
            comboLevel++;
            
            scoreUI.text = score.ToString("000000000");
            Debug.Log("스코어 10점 추가");

            

            //스피드 레벨이 다운된 적이 없으니 스피드레벨을 1 올려준다.
            //시간은 초기화 된다.
            if (isSpeedLvlDn == false)
            {
                // 스피드 레벨을 올려준다.
                speedLevel++;
            }
            else isSpeedLvlDn = false;
            if (speedLevel > 9) speedLevel = 9;


            // 스피드 레벨을 실제 적용한다.
            SpeedSetting(speedLevel);
        }
    }

    /// <summary>
    /// 점수를 추가하기 위한 메소드. 메소드에 추가될 점수를 인자로 불러온다.
    /// </summary>
    /// <param name="s"></param>
    void AddScore(int s)
    {
        score += s;
        scoreUI.text = score.ToString("000000000");
        Debug.Log("스코어 지정된 점수 추가");
    }

    /// <summary>
    /// 라인 점수 계산하는 메소드.
    /// </summary>
    void CalLineScore()
    {
        // 줄별 맟춤을 계산.
        for (int i = 0; i < 5; i++)
        {
            bool isBreak = false;
            for (int j = 0; j < 5; j++)
            {
                NodeScript ns = nScript[i + j];

                if (ns.oriPosX != ns.poxNowX || ns.oriPosY != ns.poxNowY)
                {
                    isBreak = true;
                    break;
                }
            }

            if(isBreak ==false && LineMatching[i] == false)
            {
                LineMatching[i] = true;
                Debug.Log("점수 백점 추가");
                score += 100;
                scoreUI.text = score.ToString("000000000");

                //스피드 레벨이 다운된 적이 없으니 스피드레벨을 1 올려준다.
                //시간은 초기화 된다.
                if (isSpeedLvlDn == false) speedLevel += 2;
                else isSpeedLvlDn = false;
                if (speedLevel > 9) speedLevel = 9;
                SpeedSetting(speedLevel);
            }
        }
    }

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
                // 콤보 횟수를 계산한다.
                ComboMovingCount++;
                if(ComboMovingCount>=comboMaxMoving)
                {
                    Debug.Log("콤보 초기화!");
                    comboLevel = 1;
                    ComboMovingCount = 0;
                }

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
                Debug.Log("이차 파동형 이펙트 안생김");
                v.NodeMovingNo();
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



    /// <summary>
    /// 모든 노드가 제위치로 가 맞았는지 검사하는 메소드.
    /// </summary>
    /// <returns></returns>
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
    int s = 0;
    void CheckTime()
    {
        timer += Time.deltaTime;
        m = Mathf.RoundToInt(timer / 60f);
        h = m / 60;
        s = Mathf.RoundToInt(timer) % 60;
        //if (timer >= 60.0f)
        //{
        //    m++; 
        //    timer -= 60.0f; ;
        //}
        //if(m>= 60)
        //{
        //    h++;
        //    m -= 60;
        //}
        timeDigit.text =h.ToString("00")+":"+ m.ToString("00") + ":"+s.ToString("00");

        TimeCal();
    }

    void TouchFX()
    {
        //Debug.Log("터치 이펙트 함수 실행__2");

        Vector3 pos = hitTransform.position;

        // 파동형 이펙트.
        //Debug.Log("파동형 이펙트");
        //foreach (var v in tFxKids01)
        //{
        //    if (v.gameObject.activeSelf == false)
        //    {
        //        //Debug.Log("터치 이펙트01 함수 실행__3 :: 이름: " + v.name);
        //        //v.gameObject.SetActive(true);
        //        v.GetComponent<TouchFxCon>().FxCon();
        //        v.position = new Vector3(pos.x, 0.3f, pos.z);

        //        //이펙트를 부모 역할을 하는 노드에 변수로 넣어 놓는다.
        //        hitTransform.GetComponent<NodeFX>().tfc.Add(v.GetComponent<TouchFxCon>());

        //        break;
        //    }
        //}

        // 점멸형 이펙트.
        foreach (var v in tFxKids02)
        {
            if (v.gameObject.activeSelf == false)
            {
                Debug.Log("터치 이펙트02 함수 실행 1:: 이름: " + v.name);
                v.gameObject.SetActive(true);
                v.GetComponent<TouchFxCon>().FxCon();
                v.position = new Vector3(pos.x, 0.3f, pos.z);
                v.GetComponent<TouchFxCon>().PoseMoving(hitTransform);
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
