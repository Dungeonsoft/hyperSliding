using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextOn : MonoBehaviour
{

    public List<GameObject> ls;


    public void NestOnObject()
    {
        foreach (var o in ls)
        {
            o.SetActive(true);
        }
    }

    private void OnEnable()
    {
        foreach (var o in ls)
        {
            o.SetActive(false);
        }
    }
}
