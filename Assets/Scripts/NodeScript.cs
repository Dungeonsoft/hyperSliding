﻿using System;
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

    private void Awake()
    {
        nf = GetComponent<NodeFX>();
    }


    public void CheckPosition()
    {
        if (oriPosX == poxNowX && oriPosY == poxNowY)
        {
            nf.NodeType(nType.CorrectPos);

            //if (isWaitingRefractFx == true)
            //{
            //    GameManager gm = Transform.FindObjectOfType<GameManager>();
            //    gm.ShowFxRefract(this.transform);
            //    isWaitingRefractFx = false;
            //}
        }
        else
        {
            nf.NodeType(nType.Normal);
        }
    }

    public void ChangeNodeType(nType t)
    {
        //Debug.Log(name+" :: "+t.ToString());
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

    public IngameItems AddIngameItem(int randomRate = 15)
    {
        int r = RandomRange.Range(0, 10000);
        randomRate *= 100;
        //Debug.Log("r value: "+r);
        if (r < randomRate)
        {
            IngameItems igItem = RandomEnum<IngameItems>();

            return igItem;
        }
        else
        {
            return IngameItems.None;
        }
    }


    public static T RandomEnum<T>()
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(new System.Random().Next(1, values.Length));
    }
}
