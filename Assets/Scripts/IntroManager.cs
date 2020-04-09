using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 가상의 아몬드서버를 만든다.
/// 차후에 서버를 지원 받으면 이부분을 연결한다.
/// </summary>
public class AmondServer
{
    /// <summary>
    /// 서버에서 광고가 가능한지를 체크하여 boolean 값으로 받아온다.
    /// </summary>
    /// <param name="ck"></param>
    public bool CheckActivateCM_Server()
    {
        return true;
    }


    /// <summary>
    /// 서버에서(정확히는 구글리더보드)에서 정보를 가지고 오게 한다.
    /// 연결된 경우가 아니면(구글리더보드로그인을 안했거나 온라인이 아니면) 
    /// 폰 내부에서 최고 점수를 가지고 온다.
    /// </summary>
    /// <returns></returns>
    public int CheckBestScore_Server()
    {
        // 현재는 서버와 연결되지 않았으니 로컬에서 정볼ㄹ 가지고 오게 하거나.
        // 한번도 실행한 적이 없다면 페이크 점수를 가지고 오게 한다.(테스트 상태)
        // 페이크 점수는 차후 서버와 연동하여 점수를 가지고 오게 되면 삭제한다.

        var hScore = PlayerPrefs.GetInt("HighScore", 0);

        return hScore;
    }


    /// <summary>
    ///  구글 리더보드 연동이 되었는지 체크하여 boolean 값을 돌려준다.
    ///  현재는 구현되어 있지 않으니 false 를 리턴한다.
    /// </summary>
    /// <returns></returns>
    public bool CheckleaderBoardOn_Server()
    {
        return false;
    }

    /// <summary>
    /// 구글플레이에 연동하는 코드를 실행한다.
    /// 이미 연동 되어 있으면 바로 리더보드를 열도록 한다.
    /// </summary>
    public void ConnectGoogle_Server()
    {
        Debug.Log("구글플레이 연동");
    }
}

public class IntroManager : MonoBehaviour
{
    #region variables

    /// <summary>
    /// 게임매니저 등록하기.
    /// </summary>
    public GameManager gm;

    /// <summary>
    /// 텍스트메쉬 프로의 텍스트를 이용하여 최고 점수를 표현한다.
    /// </summary>
    public TMPro.TextMeshProUGUI bestScore;

    AmondServer aServer = new AmondServer();


    public Image AmondPlay;
    /// <summary>
    /// 두개의 이미지를 넣는다.
    /// 첫번째는 구글리더보드가 연결되지 않았다는 이미지.
    /// 두번째는 구글 리더보드가 연결되었다는 이미지.
    /// </summary>
    public Sprite[] amondLeaderBoard = new Sprite[2];


    /// <summary>
    /// 아이템 활성 버튼의 역할을 하고, 이곳에 발동하는 아이템 아이콘이 보인다. 
    /// </summary>
    public Transform item_Btn;

    public GameObject Equiped;

    /// <summary>
    /// 게임에서 사용되는 아이템이미지를 모아놓는다.
    /// </summary>
    public Object[] items;

    /// <summary>
    /// 광고 가능여부를 보여는 스프라이트를 넣는다.
    /// 첫번째는 광고 불가 상태
    /// 두번째는 광고 가능 상태
    /// </summary>
    public Sprite[] connectCM_Sprite;
    /// <summary>
    /// 스프라이트가 들어가는 UI Image 컴포넌트를 연결한다.
    /// </summary>
    public Image connectCM;
    public GameObject cmEquiped;
    public bool isCmAllow = false;

    /// <summary>
    /// 광고가 가능한 상태인지를 받아올때 사용되는 boolean 변수.
    /// </summary>
    bool isAct = false;

    int savedScore;
    string KeyString;



    #endregion

    void OnEnable()
    {
        SetNodePosOrigin();

        CheckBestScore();
        CheckleaderBoardOn();
        CheckItems();
        CheckActivateCM();

        isCmAllow = false;
        cmEquiped.SetActive(false);

        //새로운 비지엠 작동.
        BG_Introdution bgi = GameObject.FindObjectOfType<BG_Introdution>();
        bgi.SetNewBgmIntro();
    }


   public List<Transform> nodes;

    void SetNodePosOrigin()
    {
        foreach(var v in nodes)
        {
            NodeScript ns = v.GetComponent<NodeScript>();
            ns.poxNowX = ns.oriPosX;
            ns.poxNowY = ns.oriPosY;
            v.localPosition = new Vector3(-256 + ns.oriPosY * 128, 256 - ns.oriPosX * 128, 0);
        }

        Debug.Log("Set Node Pos Orozin");
    }

    private void OnDisable()
    {
    }

    void ShowItem(int r)
    {
        //Debug.Log("Random Value: " + r);
        //Debug.Log("Item Count: "+ item_Btn.childCount);
        //Debug.Log("Item Child Name: " + item_Btn.GetChild(0).name);
        if (item_Btn.childCount > 0)
        {
            //Debug.Log("Will Destroy Item: "+ item_Btn.GetChild(0).name);
            Destroy(item_Btn.GetChild(0).gameObject);
        }
        //int r = Random.Range(0, 3);
        GameObject GO = Instantiate(items[r]) as GameObject;
        GO.name = "Item_RandomSel";
        GO.transform.parent = item_Btn;
        GO.transform.localPosition = Vector3.zero;
        GO.transform.localEulerAngles = Vector3.zero ;
        GO.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// BestScore를 체크하여 가지고 온다.
    /// </summary>
    void CheckBestScore()
    {
        bestScore.text = aServer.CheckBestScore_Server().ToString("000000000");
    }

    /// <summary>
    /// 리더보드 연동상태를 체크하여 가지고 온다.
    /// </summary>
    void CheckleaderBoardOn()
    {
        bool isLbOn = false;
        isLbOn = aServer.CheckleaderBoardOn_Server();

        // 구글 리더보드가 연결이 안되어 있으면 구글플레이 가입 유도아이콘을 보여주고
        // 가입되어 있으면 바로 리더보드를 보여주도록 한다.
        if (isLbOn == false)
            AmondPlay.sprite = amondLeaderBoard[0];
        else
            AmondPlay.sprite = amondLeaderBoard[1];
    }

    /// <summary>
    /// 구글 리더보드를 호출하는 메소드를 작성한다.
    /// </summary>
    public void ConnectGoogle()
    {
        aServer.ConnectGoogle_Server();
    }

    /// <summary>
    /// 활성 아이템을 가지고 온다.
    /// </summary>
    void CheckItems()
    {
        // 아이템의 갯수를 가지고 온다.
        var iCount = items.Length;
        //Debug.Log("items Count: "+ iCount);


        //CheckItemRange를 통하여 가지고 온 값을 이용해 몇개의 아이템이 보이게 할 것인지 정한다.
        var showCount =0;
        // 아이템은 기지정된 것들 중에서 하나가 나오게 만들어준다.
        // 레벨이나 점수에 따른 범위의 제약을 파악하는 메소드를 먼저 실행하게 만들어 주고 그다음에
        // 범위의 제약이 걸린 상태로 아이템이 나오게 해준다.
        var iRange = CheckItemRange();

        if(iRange<1000)
        {
            Debug.Log("Range: 1");
            showCount = iCount;
        }
        else if (iRange < 10000)
        {
            Debug.Log("Range: 2");
            showCount = iCount;
        }
        else if(iRange < 100000)
        {
            Debug.Log("Range: 3");
            showCount = iCount;
        }
        else if(iRange < 1000000)
        {
            Debug.Log("Range: 4");
            showCount = iCount;
        }
        var getItemNum = Random.Range(0,showCount);

        ShowItem(getItemNum);
    }

    public void ChangeItem()
    {
        Debug.Log("체인지 아이템");
    }

    int CheckItemRange()
    {
        //아이템이 나오는 범위를 측정하는 코드를 이곳에 작성한다.
        return 1000;
    }

    /// <summary>
    /// 광고 활성 여부를 체크하여 가지고 온다.
    /// </summary>
    void CheckActivateCM()
    {
        isAct = aServer.CheckActivateCM_Server();


        Debug.Log("isAct :: "+isAct);
        //활성화가 되어있지 않으면 터치가 되면 안되기에 레이캐스트를 끊는다.
        connectCM.raycastTarget = isAct;
        //if (isAct == true)
        //    connectCM.sprite = connectCM_Sprite[0];
        //else
        //    connectCM.sprite = connectCM_Sprite[1];
    }


    /// <summary>
    /// 광고보기를 누르면 이 메소드를 실행한다.
    /// 광고 활성여부를 체크하며 비활성시 광고를 보여주고
    /// 광고보기를 통한 인게임 보상 아이템을 발동시킨다(액티브)
    /// </summary>
    public void ShowCM()
    {
        if (isCmAllow == false) {
            //광고 실행.
            Debug.Log("광고 실행: 광고와 관련된 코드를 실행한다.");
            isCmAllow = true;
            cmEquiped.SetActive(true);
            Kinds itemKind = item_Btn.GetChild(0).GetComponent<ItemProperty>().kind;
            gm.ApplyItem(itemKind);
        }
        else
        {
            cmEquiped.SetActive(false);
        }
    }
}
