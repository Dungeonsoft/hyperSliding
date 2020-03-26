using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Kinds
{
    scoreAddPercent10,
    timeAdd10,
    scoreFinalIncrease10,
    speedDownLimit,
    speedAdd,
    itemFinder
}

public class ItemProperty : MonoBehaviour
{
    public string itemName;
    public Kinds kind;
}
