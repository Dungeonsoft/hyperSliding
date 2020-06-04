using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRolling : MonoBehaviour
{
    public GameObject[] showObj;
    public GameObject[] hideObj;

public void ActShowHide()
    {
        foreach(var v in  showObj)
        {
            v.SetActive(true);
        }

        foreach (var v in hideObj)
        {
            v.SetActive(false);
        }

    }
}
