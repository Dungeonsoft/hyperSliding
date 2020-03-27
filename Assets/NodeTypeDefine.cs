using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum nType { Normal, Move, CorrectPos, CorrectLine };



[System.Serializable]
public class Define
{
    public Color digitColor;

    public Color lineColor;

    public Color lineGlowColor;

    public Color baseColor;

    //public float baseAlpha;
}

[System.Serializable]
public class NodeTypeDefine : MonoBehaviour
{
    public List<Define> nDefine;

}
