using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum IngameItems
{
    None,IncreaseTime10,IncreaseScore50
}


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
    public AudioSource audioClick;
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
    public TMPro.TextMeshProUGUI timeUI;

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
    public TMPro.TextMeshProUGUI scoreUI;

    /// <summary>
    /// 실제 남은 시간을 나타내주기 위해 만든 int 변수
    /// </summary>
    int spendTime;

    //Action DelayTouch;
    public List<DelayTouch> dTouch = new List<DelayTouch>();
    Transform hitTransform;

    public TMPro.TextMeshProUGUI speedDigit;

    public float speedStepTimeDefault;
    float speedLevelTime;

    int speedLevel = 0;

    public int comboLevelBase = 0;
    public int comboLevel = 1;
    public int comboMaxMoving = 30;
    public int ComboMovingCount = 0;
    public bool isComboLvDn = false;

    public bool isPause = false;

    public int lineCorrectScore = 100;
    public int comboDefaultScore = 10;

    public List<TMPro.TextMeshProUGUI> scoreTextFailComple;
    public GameObject gameFailWin;
    public GameObject gameCompleWin;

    int scoreIncreasePercent = 0;

    #endregion


    private void Awake()
    {


        isPause = false;
        nodeSpeed = baseNodeSpeed;
        //puzzle = transform.parent.Find("Puzzle");

        //nScript = puzzle.GetComponentsInChildren<NodeScript>();

        //hideNode = puzzle.GetChild(Random.Range(0, 25));
        hideNode = nScript[24].transform;
        hideNode.gameObject.SetActive(true);
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
        //Debug.Log("Speed : " + step);

        manualNodeSpeed = speedList[step].manualNodeSpeed;
        delaySpeed = speedList[step].delaySpeed;

        speedDigit.text = "X" + (step + 1);

        speedLevelTime = speedList[step].decreaseTime;
        //Debug.Log("Speed : " + step+ "::: Speed Level Time : "+ speedLevelTime);
    }

    private void OnDisable()
    {
        Debug.Log("InGame OnDisable End!");
        uAction = null;
        uActionTimer = null;

        // 일회성 아이템으로 인한 스코어 증가 정도를 초기화 한다.
        scoreIncreasePercent = 0;

    }

    public BG_Introdution bg_Introdution;
    private void OnEnable()
    {
        // BGM을 새로 세팅해준다.
        bg_Introdution.SetNewBgmIngame();


        //Debug.Log("InGame OnEnable Start!");
        // 델리게이트에 있는 것들을 다 지운다.
        //uAction = null;
        //uActionTimer = null;

        isPause = false;

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

        //Debug.Log("시간 계산 시작.");
        timeDigit.text = "00:00:00";
        timer = 0;
        TimeCal();


        //Debug.Log("섞기 횟수를 초기화 함");
        mixCnt = 0;
        isMixed = false;

        /// 스코어 초기화.
        score = 0;
        scoreUI.text = "000000000";

        nodeSpeed = baseNodeSpeed;


        //모든 노드를 노멀상태로 초기화.
        for (int i = 0; i < nScript.Length; i++)
        {
            nScript[i].ChangeNodeToNormal(nType.Normal);
        }

        //Debug.LogError("Hang on a second! I gotta see Them!");


        hideNode = nScript[24].transform;
        hideNode.gameObject.SetActive(true);
        hideX = hideNode.GetComponent<NodeScript>().poxNowX;
        hideY = hideNode.GetComponent<NodeScript>().poxNowY;

        hideNode.gameObject.SetActive(false);

        Debug.Log("Do Mix Count A");
        uAction = MixCount;
        
        //if(uAction != null)
        //{
        //    Debug.Log("====uAction is not NULL");
        //}
        //else
        //{
        //    Debug.Log("=====uAction is NULL");
        //}

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
        //Debug.Log("시간 계산 들어옴");
        speedLevelTime -= Time.deltaTime;

        if(speedLevelTime<= 0 && speedLevel > 0)
        {
            isSpeedLvlDn = true;
             speedLevel -= 1;
            SpeedSetting(speedLevel);
        }

        spendTime = remainingTime - Mathf.RoundToInt(timer);
        if (spendTime <= 0)
        {
            spendTime = 0;
            Debug.Log("게임시간종료");
            ShowGameFailWin();
        }

        min = spendTime / 60;
        sec = spendTime % 60;
        timeUI.text = min.ToString("00") + ":" + sec.ToString("00");
    }


    void ShowGameFailWin()
    {
        TouchPause();
        ScoreSaveToLocal();
       gameFailWin.SetActive(true);
    }
     void ShowCompleWin()
    {
        TouchPause();
        ScoreSaveToLocal();
        gameCompleWin.SetActive(true);

    }

    void MixCount()
    {
        //Debug.Log("MixCount1: " + mixCnt);
        if (mixRnd>mixCnt)
        {
            //Debug.Log("MixCount2: "+ mixCnt);
            mixCnt++;
            uAction = CalcurateMovableNode;
        }
        else
        {
            isMixed = true;
            nodeSpeed = manualNodeSpeed;
            Debug.Log("=====Now You can click nodes!=====");

            uAction = null;

            // CheckClick이 최초로 들어가는 부분 //  그러니 다른 함수가 델리게이트에 있으면 안되는 그냥 단독으로 넣어준다.
            //uAction = CheckClick;
            uActionTimer = CheckTime;
        }
    }

    void CalcurateMovableNode()
    {
        //Debug.Log("움직일 노드를 찾는 계산을 함 ori1X: "+ ori1X+" = ori1Y: "+ ori1Y);
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
            //Debug.Log("R Num: " + r);
            int interVal = Mathf.Abs(hideX - ori1X);
            if (hideX > ori1X)
            {
                for (var i = 0; i <= interVal; i++)
                {
                    if (AddNode(posN[r - i]) != null)
                    {
                        mNodes.Add(AddNode(posN[r - i]));
                    }
                    else
                    {
                        Debug.LogWarning("There is nothing to add :: " + posN[r - i]);
                    }
                }
            }
            else
            {
                for (var i = 0; i <= interVal; i++)
                {
                    if (AddNode(posN[r + i]) != null)
                    {
                        mNodes.Add(AddNode(posN[r + i]));
                    }
                    else
                    {
                        Debug.LogWarning("There is nothing to add :: " + posN[r + i]);
                    }
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
                    if (AddNode(posN[r - i]) != null)
                    {
                        mNodes.Add(AddNode(posN[r - i]));
                    }
                    else
                    {
                        Debug.LogWarning("There is nothing to add :: " + posN[r - i]);
                    }
                }
            }
            else
            {
                for (var i = 0; i <= interVal; i++)
                {
                    if (AddNode(posN[r + i]) != null)
                    {
                        mNodes.Add(AddNode(posN[r + i]));
                    }
                    else
                    {
                        Debug.LogWarning("There is nothing to add :: " + posN[r + i]);
                    }
                }
            }
        }

        isFX = new List<bool>();

        mNodesPos = new List<Vector3>();
        foreach (var node in mNodes)
        {
            if (node == null) Debug.Log("Node is NULL");
            //Debug.Log("NODE name: " + node.name+" ::: Avtive: "+ node.gameObject.activeSelf);
            mNodesPos.Add(node.localPosition);
            isFX.Add(node.GetComponent<NodeFX>().isFX);
        }


        #region 노드를 옮기고 노드의 현재 위치값을 교정하고 비어있는 노드의(hideNode) 현재 위치 값을 교정한다.

        
        if (isMixed)
        {
            List<TouchFxCon> htTfc = new List<TouchFxCon>();
            //Debug.Log("HT Name: " + mNodes[0].name +" :: "+ mNodes[0].GetComponent<NodeFX>().tfc.Count);
            htTfc = mNodes[0].GetComponent<NodeFX>().tfc;

            foreach (var v in htTfc)
            {
                v.NodeMoving();
            }
            mNodes[0].GetComponent<NodeFX>().tfc = new List<TouchFxCon>();
        }
        

        selNodePosition = mNodes[0].localPosition;
        hideNodePosition = hideNode.localPosition;
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
        if (sNode == null) Debug.Log("nPos Error:" + nPos.x+" ::: " +nPos.y);
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
            mNodes[i].localPosition = Vector3.Lerp(mNodesPos[i], mNodesPos[i + 1], v);

            //벽 또는 다른 노드와 부딪힐때 이펙트를 발동시키는 부분. 
            if (isMixed)
            {
                if (isFX[i] == false && (lVal - (delaySpeed * (cnt-i) * Time.deltaTime)) >= 0.7088259f)
                {
                    isFX[i] = true;
                    mNodes[i].GetComponent<NodeFX>().ActionCrashFX(i);

                    /// 정확한 위치로 들어간 노드에 왜곡이펙트가 만들어질수 있도록 불변수에 true 값을 입력한다.
                    
                    //mNodes[i].GetComponent<NodeFX>()
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

                mNodes[i].localPosition = mNodesPos[i + 1];

                if (isMixed) StartCoroutine(IeShowFxRefract(mNodes));
            }


            lVal = 0.0f;

            hideNode.localPosition = selNodePosition;
            hideNode.GetComponent<NodeScript>().poxNowX = hideX;
            hideNode.GetComponent<NodeScript>().poxNowY = hideY;

            // 최초 인게임 작동시 셔플이 다 되지 않았을때 오는 곳.
            if (isMixed != true)
            {
                //Debug.Log("Do Mix Count B");
                uAction = MixCount;
            }



            // 셔플이 다 되면 모든 블럭이 이동후 이곳으로 온다.
            // 점수 계산도 이곳에서 이루어진다.
            else
            {
                //Debug.Log("점수 계산");
                #region 점수계산
                //노드 하나하나의 위치가 맞을 때 점수를 계산하기 위한 for문.
                for (int i = 0; i < cnt; i++)
                {
                    //Debug.Log("노드별 점수 계산");
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
                    timeUI.color = Color.yellow;

                    Debug.Log("Score: " + score);
                    Debug.Log("Remain Time Bonus: " + spendTime * perSecScore);
                    // 남은 시간을 기준으로 추가 점수(초당00점)를 지급한다.
                    AddScore(spendTime * perSecScore * comboLevel);
                    AddScoreIcreaseItem();

                    Debug.Log("Score: "+score);
                    Debug.Log("Speed Level: " + speedLevel);
                    Debug.Log("Add Score Percent: " + (speedList[speedLevel].speedScoreBonus * 0.01f));
                    Debug.Log("Speed Bonus: " + Mathf.RoundToInt(score * (speedList[speedLevel].speedScoreBonus * 0.01f)));

                    // 남은 시간을 최종 스피드 레벨에 따른 보너스를 지급한다.
                    AddScore(Mathf.RoundToInt(score * (speedList[speedLevel].speedScoreBonus * 0.01f)));

                    ScoreSaveToLocal();
                    ShowCompleWin();
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

                // 노드의 컬러(형태)를 지정하기 위한 부분이다.
                checkNodeColor();

            }
        }
    }

    IEnumerator IeShowFxRefract(List<Transform> mn)
    {
        //Debug.Log("굴절 이펙트 발생");
        for (int i = 0; i < mn.Count; i++)
        {
            NodeScript ns = mn[i].GetComponent<NodeScript>();
            if (ns.oriPosX == ns.poxNowX && ns.oriPosY == ns.poxNowY)
            {
                ShowFxRefract(mn[i]);
                yield return new WaitForSecondsRealtime(0.2f);
            }
        }
    }

    void RunFx(Transform trans, Transform posT)
    {
        Debug.Log("======================= 아이템 이펙트 발생 ::: "+ trans.name);
        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject GO = trans.GetChild(i).gameObject;
            if (GO.activeSelf == false)
            {
                Debug.Log("pos 위치: " + posT.position);

                GO.SetActive(true);
                GO.transform.position = new Vector3(posT.position.x,3f ,posT.position.z);
                Debug.Log(GO.name + " ::: 위치: "+ GO.transform.position);
                break;
            }
        }
    }

    public Transform timeItemFx01;
    public Transform comboItemFx01;

    /// <summary>
    /// 점수계산하는 메소드 인자는 노드에 붙어있는 노드 스크립트이며 여기서 정보를 호출하여 계산한다.
    /// </summary>
    /// <param name="ns"></param>
    void CalScore(NodeScript ns)
    {
        //Debug.Log("선택된 노드 이름: "+ns.name+" :: "+ ns.oriPosX+"="+ ns.poxNowX+"==="+ ns.oriPosY+"="+ns.oriPosY);
        if (ns.oriPosX == ns.poxNowX && ns.oriPosY == ns.poxNowY && ns.alreadyScoreCount == false)
        {
            IngameItems getItem =  ns.AddIngameItem();

            //Debug.Log("getItem :: "+ getItem);
            switch (getItem)
                {
                case IngameItems.None:
                    // 얻은 아이템이 없음.
                    break;
                case IngameItems.IncreaseTime10:
                    // 시간추가.
                    TimeAdd(10);
                    // 이펙트 실행.
                    RunFx(timeItemFx01, ns.transform);
                    break;
                case IngameItems.IncreaseScore50:
                    // 콤보점수 추가.
                    comboDefaultScore = 50;
                    // 이펙트 실행.
                    RunFx(comboItemFx01, ns.transform);
                    break;
            }

            // 여기서 계산을 해주고 이펙트도 추가해준다.
            // 이펙트 발생위치.

            ns.alreadyScoreCount = true;
            //score += 10 * comboLevel;
            int addComboScore = comboDefaultScore * comboLevel;
            AddScore(addComboScore);
            comboDefaultScore = 10;
            // 다음번부터 콤보 보너스를 적용하기 위해 콤보레벨을 올려준다.
            comboLevel++;
            
            //Debug.Log("스코어 10점 추가");

            

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
        //Debug.Log("스코어 지정된 점수 추가");
    }

    /// <summary>
    /// 라인 점수 계산하는 메소드.
    /// </summary>
    void CalLineScore()
    {
        NodeScript ns;
        // 줄별 맟춤을 계산.
        for (int i = 0; i < 5; i++)
        {
            bool isBreak = false;
            for (int j = 0; j < 5; j++)
            {
                ns = nScript[i*5 + j];

                // 라인 체크를 하면서 동시에 정확한 위치에 있는지 확인한다.
                // 라인이 맞게 되면 이부분이 먼저 실행이 되니 무시되는 것과 같게 된다.

                if (ns.oriPosX != ns.poxNowX || ns.oriPosY != ns.poxNowY)
                {
                    isBreak = true;
                    break;
                }
            }

            if(isBreak ==false)
            {

                for (int j = 0; j < 5; j++)
                {
                    ns = nScript[i*5 + j];
                }

                if (LineMatching[i])
                {
                    LineMatching[i] = true;
                    Debug.Log("점수 백점 추가");
                    AddScore(lineCorrectScore * comboLevel);

                    //스피드 레벨이 다운된 적이 없으니 스피드레벨을 1 올려준다.
                    //시간은 초기화 된다.
                    if (isSpeedLvlDn == false) speedLevel += 2;
                    else isSpeedLvlDn = false;
                    if (speedLevel > 9) speedLevel = 9;
                    SpeedSetting(speedLevel);
                }
            }
        }

    }

    void checkNodeColor()
    {
        NodeScript ns;
        for (int i = 0; i < 25; i++)
        {
                ns = nScript[i];
                ns.CheckPosition();
        }

        CheckLineColor();
    }

    void CheckLineColor()
    {
        bool isLine = true;
        NodeScript ns;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                ns = nScript[i*5 + j];
                if (ns.oriPosX == ns.poxNowX && ns.oriPosY == ns.poxNowY)
                {
                    isLine = true;
                }
                else
                {
                    isLine = false;
                    break;
                }
            }
            
            if (isLine == true)
            {
                Debug.Log("Value I: "+i);
                for (int j = 0; j < 5; j++)
                {
                    ns = nScript[i*5 + j];
                    ns.ChangeNodeType(nType.CorrectLine);
                }
            }
        }

    }

    void ScoreSaveToLocal()
    {
        var showScore = PlayerPrefs.GetInt("HighScore");
        if (score > showScore) showScore = score;
        PlayerPrefs.SetInt("HighScore", showScore);

        foreach (var v in scoreTextFailComple)
        {
            v.text = showScore.ToString("000000000");
        }
    }

    /// <summary>
    /// 터치를 하면 노드를 터치했는지 감지한다. 
    /// </summary>
    public void CheckClick(Transform thisT)
    {
        if (isPause == true) return;
        // 터치 사운드 발생.
        audioClick.Play();
        //Debug.Log("터치 함");

        hitTransform = thisT;

        //hitTransform = hit.transform;
        if (hitTransform.CompareTag("Node"))
        {
            //Debug.Log("노드 클릭");
            // 콤보 횟수를 계산한다.
            ComboMovingCount++;
            if (ComboMovingCount >= comboMaxMoving)
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
                //Debug.Log("두개이상");
                TouchFX();
                //Debug.Log("등록이 되었나 : " + hitTransform.name + " :: " + hitTransform.GetComponent<NodeFX>().tfc.Count);
                dTouch.Add(new DelayTouch { ht = hitTransform, m = isMovable });
            }
            else
            {
                //Debug.Log("하나");
                TouchFX();
                //Debug.Log("등록이 되었나 : " + hitTransform.name + " :: " + hitTransform.GetComponent<NodeFX>().tfc.Count);
                dTouch.Add(new DelayTouch { ht = hitTransform, m = isMovable });
                TouchAction(hitTransform, isMovable);
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

        // 게임이 멈춰 있을 때는 작동을 하지 않는다. (계속 같은 시간을 보여줌)
        if (isPause == false) timer += Time.deltaTime;

        m = Mathf.RoundToInt(timer / 60f);
        h = m / 60;
        s =  Mathf.RoundToInt(timer) % 60;
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

        Vector3 pos = hitTransform.localPosition;

        // 점멸형 이펙트.
        foreach (var v in tFxKids02)
        {
            if (v.gameObject.activeSelf == false)
            {
                //Debug.Log("터치 이펙트02 함수 실행 1:: 이름: " + v.name);
                v.gameObject.SetActive(true);
                v.GetComponent<TouchFxCon>().FxCon(hitTransform);
                v.localPosition = new Vector3(pos.x, 0.3f, pos.z);
                v.GetComponent<TouchFxCon>().PoseMoving(hitTransform);
                //이펙트를 부모 역할을 하는 노드에 변수로 넣어 놓는다.
                hitTransform.GetComponent<NodeFX>().tfc.Add(v.GetComponent<TouchFxCon>());
                break;
            }
        }
    }


    /// <summary>
    /// TouchFX01 을 직접실행하는 메소드.
    /// 현재는 관련되어 있는 이펙트 오브젝트를 임시 위치로 옮겨놓아 검색이 안되어 작동을 하지 않음.
    /// </summary>
    /// <param name="h"></param>
    public void ShowFxRefract(Transform h)
    {
        foreach (var v in tFxKids01)
        {
            if (v.gameObject.activeSelf == false)
            {
                TouchFxCon tfc = v.GetComponent<TouchFxCon>();
                v.gameObject.SetActive(true);
                tfc.ShowFxRefract(h);
                break;
            }
        }
    }

    /// <summary>
    /// 광고를 보고 활성화 되는 아이템을 적용하는 메소드.
    /// </summary>
    /// <param name="k"></param>
    public void ApplyItem(Kinds k)
    {
        switch (k)
        {
            case Kinds.scoreFinalIncrease10:
                scoreIncreasePercent = 10;
                break;
        }
    }

    void AddScoreIcreaseItem()
    {
        var addScore = Mathf.RoundToInt(score * (scoreIncreasePercent / 100.0f));
        AddScore(addScore);
    }


    private void Update()
    {
        //if (uAction == null)
        //{
        //    Debug.Log("=== In Update, uAction is NULL!");
        //}
        //else
        //{
        //    Debug.Log("=== In Update, uAction is not NULL!" + uAction.Method.Name);
        //}
        uAction?.Invoke();

        uActionTimer?.Invoke();
    }

    public void TouchPause()
    {
        isPause = !isPause;
    }
}
