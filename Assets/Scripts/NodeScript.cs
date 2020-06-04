using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomRange = UnityEngine.Random;

public class NodeScript : MonoBehaviour
{
    public int oriPosX;
    public int oriPosY;

    public int poxNowX;
    public int poxNowY;

    int firstPosX;
    int firstPosY;

    public bool alreadyScoreCount;

    //public bool isWaitingRefractFx = false;

    public NodeFX nf;

    public GameManager gm;
    public AudioSource aSource;


    private void Awake()
    {
        nf = GetComponent<NodeFX>();
    }


    public void CheckPosition()
    {
        //Debug.Log("Crash Clear! === 2: "+ this.name);
        if (oriPosX == poxNowX && oriPosY == poxNowY)
        {
            nf.NodeType(nType.CorrectPos);
        }
        else
        {
            nf.NodeType(nType.Normal);
        }
    }

    public void ChangeNodeType(nType t)
    {
        Debug.Log(name+" :: "+t);
        nf.NodeType(t);
    }

    
    /// <summary>
    /// 노드의 형태를 노멀로 변형.
    /// </summary>
    /// <param name="t"></param>
    public void ChangeNodeToNormal(nType t)
    {
        //Debug.Log(name + " :: " + t.ToString());
        nf.NodeToNormal(t);
    }

    public IngameItems AddIngameItem(/*int randomRate = 15*/)
    {

        if(gm.ingameItemShowCount >= 4) return IngameItems.None;

        int randomRate = Mathf.RoundToInt((gm.rateCount / 6f) * 10000f);

        int r = RandomRange.Range(0, 10000);
        
        //Debug.Log("r value: "+r);
        if (r < randomRate)
        {
            Debug.Log("인게임 아이템 생성");
            IngameItems igItem = RandomEnum<IngameItems>();

            gm.ingameItemShowCount++;
            if(gm.ingameItemShowCount>= 4)
            {
                gm.rateCount = 0;
            }
            gm.rateCount = 1;
            return igItem;
        }
        else
        {
            Debug.Log("인게임 아이템 없음");
            gm.rateCount++;
            return IngameItems.None;
        }
    }


    public static T RandomEnum<T>()
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(new System.Random().Next(1, values.Length));
    }



}
