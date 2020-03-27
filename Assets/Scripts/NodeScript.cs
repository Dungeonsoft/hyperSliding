using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    public int oriPosX;
    public int oriPosY;

    public int poxNowX;
    public int poxNowY;

    int firstPosX;
    int firstPosY;

    public bool alreadyScoreCount = false;



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
        }
        else
        {
            nf.NodeType(nType.Normal);
        }
    }

    public void ChangeNodeType(nType t)
    {
        Debug.Log(name+" :: "+t.ToString());
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

}
